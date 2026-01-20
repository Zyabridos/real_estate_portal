#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
source "${SCRIPT_DIR}/../../lib/config.sh"
source "${SCRIPT_DIR}/../../lib/log.sh"

BACKEND_URL="${BACKEND_URL:-http://localhost:5055}"
HEALTH_PATH="${HEALTH_PATH:-/api/health}"
PAGE_SIZE="${SEED_PAGE_SIZE:-100}"
SEED_PROPERTIES_COUNT="${SEED_PROPERTIES_COUNT:-25}"

require_cmd() {
  command -v "$1" >/dev/null 2>&1 || { error "Missing required command: $1"; exit 1; }
}

require_cmd curl
require_cmd python3

seed_dir="${SCRIPT_DIR}/../../.seed"
seed_env="${seed_dir}/seed.env"

if [[ ! -f "${seed_env}" ]]; then
  error "[seed] Missing ${seed_env}. Run: make seed-brokers"
  exit 1
fi

source "${seed_env}"

assert_json() {
  local label="$1"
  local body="$2"
  if [[ -z "$body" ]]; then error "[seed] ERROR: ${label} returned empty response."; exit 1; fi
  if ! printf '%s' "$body" | python3 -c 'import sys,json; json.load(sys.stdin)' >/dev/null 2>&1; then
    error "[seed] ERROR: ${label} did not return JSON. Response was:"
    printf '%s\n' "$body" | head -n 120
    exit 1
  fi
}

extract_ids_from_paged() {
  python3 -c '
import sys, json
data = json.load(sys.stdin)
for x in (data.get("items") or []):
    _id = x.get("id")
    if _id:
        print(_id)
'
}

json_field() {
  local field="$1"
  python3 -c "import sys,json; print(json.load(sys.stdin).get('${field}',''))"
}

http_post_json() {
  local url="$1"
  local payload="$2"
  local resp status
  resp="$(curl -sS -i -X POST "$url" -H "Content-Type: application/json" -d "$payload")"
  status="$(printf '%s' "$resp" | head -n 1 | awk '{print $2}')"
  if [[ "$status" =~ ^2 ]]; then
    printf '%s' "$resp" | awk 'BEGIN{p=0} /^\r?$/{p=1;next} {if(p) print}'
    return 0
  fi
  error "[seed] POST failed: $url"
  warn "----- payload -----"
  printf '%s\n' "$payload"
  warn "----- response (headers) -----"
  printf '%s\n' "$resp" | head -n 40
  warn "----- response (body) -----"
  printf '%s' "$resp" | awk 'BEGIN{p=0} /^\r?$/{p=1;next} {if(p) print}'
  return 22
}

neutral "Seeding properties"
neutral "Checking API health: ${BACKEND_URL}${HEALTH_PATH}"
curl -fsS "${BACKEND_URL}${HEALTH_PATH}" >/dev/null
success "[seed] API health OK"

neutral "Fetching brokers to distribute properties across"
brokers_json="$(curl -fsS "${BACKEND_URL}/api/brokers?page=1&pageSize=${PAGE_SIZE}")"
assert_json "GET /api/brokers" "${brokers_json}"

broker_ids="$(printf '%s' "${brokers_json}" | extract_ids_from_paged || true)"
broker_count="$(printf '%s\n' "${broker_ids}" | grep -c '.*' || true)"

if [[ -z "${broker_ids}" ]]; then
  error "[seed] No brokers found. Run seed-brokers first."
  exit 1
fi

if [[ "${broker_count}" -lt 22 ]]; then
  warn "[seed] Expected 22 brokers, got ${broker_count}. Will still proceed."
fi

neutral "Clearing existing properties"
props_json="$(curl -fsS "${BACKEND_URL}/api/properties?page=1&pageSize=${PAGE_SIZE}" || true)"

prop_ids=""
if [[ -n "${props_json}" ]] && printf '%s' "$props_json" | python3 -c 'import sys,json; json.load(sys.stdin)' >/dev/null 2>&1; then
  prop_ids="$(printf '%s' "${props_json}" | extract_ids_from_paged || true)"
else
  warn "[seed] Skipping properties cleanup (GET list returned non-JSON or failed)."
fi

if [[ -n "${prop_ids}" ]]; then
  while IFS= read -r id; do
    [[ -z "$id" ]] && continue
    curl -fsS -X DELETE "${BACKEND_URL}/api/properties/${id}" >/dev/null || true
  done <<< "${prop_ids}"
  success "[seed] Properties cleared: $(echo "${prop_ids}" | wc -l | tr -d ' ')"
else
  warn "[seed] No properties to clear"
fi

create_property_payload() {
  local title="$1" city="$2" price="$3" broker_id="$4" type="$5" status="$6"

  python3 - "$title" "$city" "$price" "$broker_id" "$type" "$status" <<'PY'
import json, sys

title, city, price, broker_id, ptype, status = sys.argv[1:]

print(json.dumps({
  "title": title,
  "description": "Seed property description",
  "address": "Seed Street 1",
  "city": city,
  "price": float(price),
  "type": int(ptype),
  "bedrooms": 2,
  "bathrooms": 1,
  "area": 55.5,
  "status": int(status),
  "mainImageUrl": None,
  "brokerId": broker_id
}))
PY
}

cities=("Oslo" "Bergen" "Stavanger" "Trondheim" "Drammen" "Elverum")

# Note: must be ints for current backend JSON options
types=(0 1 2)         # Apartment=0, House=1, Commercial=2
statuses=(0 1)        # Active=0, Sold=1

# Human-readable labels for title/UI
type_labels=("Apartment" "House" "Commercial")
status_labels=("Active" "Sold")

neutral "Creating ${SEED_PROPERTIES_COUNT} properties distributed across brokers"

property_id=""
created=0
i=1

while [[ "${created}" -lt "${SEED_PROPERTIES_COUNT}" ]]; do
  broker_index=$(( created % broker_count ))
  broker_id="$(printf '%s\n' "${broker_ids}" | sed -n "$((broker_index+1))p")"

  city="${cities[$((created % ${#cities[@]}))]}"
  price=$(( 2500000 + (created * 150000) ))

  type="${types[$((created % ${#types[@]}))]}"
  status="${statuses[$((created % ${#statuses[@]}))]}"

  type_label="${type_labels[$type]}"
  status_label="${status_labels[$status]}"

  resp="$(http_post_json "${BACKEND_URL}/api/properties" "$(
    create_property_payload \
      "Seed ${type_label} #${i} (${status_label})" \
      "${city}" "${price}" "${broker_id}" "${type}" "${status}"
  )")"
  assert_json "POST /api/properties #${i}" "${resp}"

  if [[ -z "${property_id}" ]]; then
    property_id="$(printf '%s' "${resp}" | json_field "id")"
  fi

  created=$((created+1))
  i=$((i+1))
done

success "[seed] Properties created: ${SEED_PROPERTIES_COUNT}"
success "[seed] propertyId for leads: ${property_id}"

# Save PROPERTY_ID to seed.env
grep -v '^PROPERTY_ID=' "${seed_env}" > "${seed_env}.tmp" || true
mv "${seed_env}.tmp" "${seed_env}"
echo "PROPERTY_ID=${property_id}" >> "${seed_env}"

success "[seed] Saved PROPERTY_ID to ${seed_env}"
