#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
source "${SCRIPT_DIR}/../../lib/config.sh"
source "${SCRIPT_DIR}/../../lib/log.sh"

BACKEND_URL="${BACKEND_URL:-http://localhost:5055}"
HEALTH_PATH="${HEALTH_PATH:-/api/health}"
PAGE_SIZE="${SEED_PAGE_SIZE:-100}"

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

: "${BROKER1_ID:?BROKER1_ID missing}"
: "${BROKER2_ID:?BROKER2_ID missing}"
: "${BROKER3_ID:?BROKER3_ID missing}"

assert_json() {
  local label="$1"
  local body="$2"
  if [[ -z "$body" ]]; then error "[seed] ERROR: ${label} returned empty response."; exit 1; fi
  if ! printf '%s' "$body" | python3 -c 'import sys,json; json.load(sys.stdin)' >/dev/null 2>&1; then
    error "[seed] ERROR: ${label} did not return JSON. Response was:"
    printf '%s\n' "$body" | head -n 80
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
  printf '%s\n' "$payload" | head -n 200
  warn "----- response -----"
  printf '%s\n' "$resp" | head -n 120
  return 22
}

neutral "Seeding properties"
neutral "Checking API health: ${BACKEND_URL}${HEALTH_PATH}"
curl -fsS "${BACKEND_URL}${HEALTH_PATH}" >/dev/null
success "[seed] API health OK"

neutral "Clearing existing properties (best-effort)"
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
  local title="$1" city="$2" price="$3" broker_id="$4"
  python3 - <<PY
import json
print(json.dumps({
  "title": "${title}",
  "description": "Seed property description",
  "address": "Seed Street 1",
  "city": "${city}",
  "price": ${price},
  "type": 0,
  "bedrooms": 2,
  "bathrooms": 1,
  "area": 55.5,
  "status": 0,
  "mainImageUrl": None,
  "brokerId": "${broker_id}"
}))
PY
}

neutral "Creating properties"
p1="$(http_post_json "${BACKEND_URL}/api/properties" "$(create_property_payload "Seed Property #1" "Oslo" "3500000" "${BROKER1_ID}")")"
assert_json "POST /api/properties #1" "$p1"
property_id="$(printf '%s' "$p1" | json_field "id")"

# extra props - nice to have
http_post_json "${BACKEND_URL}/api/properties" "$(create_property_payload "Seed Property #2" "Trondheim" "4900000" "${BROKER2_ID}")" >/dev/null
http_post_json "${BACKEND_URL}/api/properties" "$(create_property_payload "Seed Property #3" "Bergen" "4200000" "${BROKER3_ID}")" >/dev/null

success "[seed] Properties created (propertyId for leads): ${property_id}"

# Append to seed.env (keep brokers too)
grep -v '^PROPERTY_ID=' "${seed_env}" > "${seed_env}.tmp" || true
mv "${seed_env}.tmp" "${seed_env}"
echo "PROPERTY_ID=${property_id}" >> "${seed_env}"

success "[seed] Saved PROPERTY_ID to ${seed_env}"
