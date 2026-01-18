#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
source "${SCRIPT_DIR}/../../lib/config.sh"
source "${SCRIPT_DIR}/../../lib/log.sh"

BACKEND_URL="${BACKEND_URL:-http://localhost:5055}"
HEALTH_PATH="${HEALTH_PATH:-/api/health}"
PAGE_SIZE="${SEED_PAGE_SIZE:-100}"
AGENCY_ID="${SEED_AGENCY_ID:-aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa}"

require_cmd() {
  command -v "$1" >/dev/null 2>&1 || { error "Missing required command: $1"; exit 1; }
}

require_cmd curl
require_cmd python3

seed_dir="${SCRIPT_DIR}/../../.seed"
seed_env="${seed_dir}/seed.env"
mkdir -p "${seed_dir}"

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

neutral "Seeding brokers"
neutral "Checking API health: ${BACKEND_URL}${HEALTH_PATH}"
curl -fsS "${BACKEND_URL}${HEALTH_PATH}" >/dev/null
success "[seed] API health OK"

neutral "Clearing existing brokers"
brokers_json="$(curl -fsS "${BACKEND_URL}/api/brokers?page=1&pageSize=${PAGE_SIZE}" || true)"

broker_ids=""
if [[ -n "${brokers_json}" ]] && printf '%s' "$brokers_json" | python3 -c 'import sys,json; json.load(sys.stdin)' >/dev/null 2>&1; then
  broker_ids="$(printf '%s' "${brokers_json}" | extract_ids_from_paged || true)"
else
  warn "[seed] Skipping brokers cleanup (GET list returned non-JSON or failed)."
fi

if [[ -n "${broker_ids}" ]]; then
  while IFS= read -r id; do
    [[ -z "$id" ]] && continue
    curl -fsS -X DELETE "${BACKEND_URL}/api/brokers/${id}" >/dev/null || true
  done <<< "${broker_ids}"
  success "[seed] Brokers cleared: $(echo "${broker_ids}" | wc -l | tr -d ' ')"
else
  warn "[seed] No brokers to clear"
fi

create_broker_payload() {
  local first="$1" last="$2" email="$3" phone="$4"
  python3 - <<PY
import json
print(json.dumps({
  "agencyId": "${AGENCY_ID}",
  "firstName": "${first}",
  "lastName": "${last}",
  "email": "${email}",
  "phoneNumber": "${phone}",
  "photoUrl": None
}))
PY
}

neutral "Creating brokers"
b1="$(http_post_json "${BACKEND_URL}/api/brokers" "$(create_broker_payload "Ola" "Nordmann" "ola.seed@broker.no" "+47 900 00 001")")"
assert_json "POST /api/brokers #1" "$b1"
broker1_id="$(printf '%s' "$b1" | json_field "id")"

b2="$(http_post_json "${BACKEND_URL}/api/brokers" "$(create_broker_payload "Kari" "Nordmann" "kari.seed@broker.no" "+47 900 00 002")")"
assert_json "POST /api/brokers #2" "$b2"
broker2_id="$(printf '%s' "$b2" | json_field "id")"

b3="$(http_post_json "${BACKEND_URL}/api/brokers" "$(create_broker_payload "Arya" "Stark" "arya.seed@winterfell.no" "+47 900 00 003")")"
assert_json "POST /api/brokers #3" "$b3"
broker3_id="$(printf '%s' "$b3" | json_field "id")"

success "[seed] Brokers created:"
success "  broker1=${broker1_id}"
success "  broker2=${broker2_id}"
success "  broker3=${broker3_id}"

{
  echo "BROKER1_ID=${broker1_id}"
  echo "BROKER2_ID=${broker2_id}"
  echo "BROKER3_ID=${broker3_id}"
} > "${seed_env}"

success "[seed] Saved ids to ${seed_env}"
