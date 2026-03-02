#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
source "${SCRIPT_DIR}/../../lib/config.sh"
source "${SCRIPT_DIR}/../../lib/log.sh"

BACKEND_URL="${BACKEND_URL:-http://localhost:5055}"
AGENCIES_ENDPOINT="/api/agencies"
HEALTH_PATH="${HEALTH_PATH:-/api/health/readiness}"
PAGE_SIZE="${SEED_PAGE_SIZE:-100}"

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

http_delete() {
  local url="$1"
  local resp status
  resp="$(curl -sS -i -X DELETE "$url" || true)"
  status="$(printf '%s' "$resp" | head -n 1 | awk '{print $2}')"
  if [[ "$status" =~ ^2 ]] || [[ "$status" == "404" ]]; then
    return 0
  fi
  warn "[seed] DELETE failed: $url (status=$status)"
  printf '%s\n' "$resp" | head -n 80
  return 0
}

neutral "Seeding agencies (3 agencies)"
neutral "Checking API health: ${BACKEND_URL}${HEALTH_PATH}"
curl -fsS "${BACKEND_URL}${HEALTH_PATH}" >/dev/null
success "[seed] API health OK"

# Чтобы можно было удалить agencies без “хвостов” — чистим brokers
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
    http_delete "${BACKEND_URL}/api/brokers/${id}" || true
  done <<< "${broker_ids}"
  success "[seed] Brokers cleared: $(echo "${broker_ids}" | wc -l | tr -d ' ')"
else
  warn "[seed] No brokers to clear"
fi

neutral "Clearing existing agencies"
agencies_json="$(curl -fsS "${BACKEND_URL}${AGENCIES_ENDPOINT}?page=1&pageSize=${PAGE_SIZE}" || true)"
agency_ids=""
if [[ -n "${agencies_json}" ]] && printf '%s' "$agencies_json" | python3 -c 'import sys,json; json.load(sys.stdin)' >/dev/null 2>&1; then
  agency_ids="$(printf '%s' "${agencies_json}" | extract_ids_from_paged || true)"
else
  warn "[seed] Skipping agencies cleanup (GET list returned non-JSON or failed)."
fi

if [[ -n "${agency_ids}" ]]; then
  while IFS= read -r id; do
    [[ -z "$id" ]] && continue
    http_delete "${BACKEND_URL}${AGENCIES_ENDPOINT}/${id}" || true
  done <<< "${agency_ids}"
  success "[seed] Agencies cleared: $(echo "${agency_ids}" | wc -l | tr -d ' ')"
else
  warn "[seed] No agencies to clear"
fi

create_agency_payload() {
  local name="$1" org="$2" phone="$3" city="$4" street="$5" zip="$6"
  python3 - <<PY
import json
print(json.dumps({
  "name": "${name}",
  "orgNumber": "${org}",
  "phoneNumber": "${phone}",
  "city": "${city}",
  "street": "${street}",
  "zipCode": "${zip}"
}))
PY
}

neutral "Creating 3 agencies"

a1_resp="$(http_post_json "${BACKEND_URL}${AGENCIES_ENDPOINT}" "$(create_agency_payload \
  "Nordic Homes AS" "912345678" "+47 73 10 00 01" "Trondheim" "Kjøpmannsgata 10" "7013")")"
assert_json "POST ${AGENCIES_ENDPOINT} #1" "${a1_resp}"
AGENCY1_ID="$(printf '%s' "${a1_resp}" | json_field "id")"

a2_resp="$(http_post_json "${BACKEND_URL}${AGENCIES_ENDPOINT}" "$(create_agency_payload \
  "Fjord Realty AS" "923456789" "+47 55 20 00 02" "Bergen" "Bryggen 5" "5003")")"
assert_json "POST ${AGENCIES_ENDPOINT} #2" "${a2_resp}"
AGENCY2_ID="$(printf '%s' "${a2_resp}" | json_field "id")"

a3_resp="$(http_post_json "${BACKEND_URL}${AGENCIES_ENDPOINT}" "$(create_agency_payload \
  "Oslo Living AS" "934567890" "+47 22 30 00 03" "Oslo" "Karl Johans gate 1" "0154")")"
assert_json "POST ${AGENCIES_ENDPOINT} #3" "${a3_resp}"
AGENCY3_ID="$(printf '%s' "${a3_resp}" | json_field "id")"

success "[seed] Agencies created"
success "  agency1=${AGENCY1_ID}"
success "  agency2=${AGENCY2_ID}"
success "  agency3=${AGENCY3_ID}"

all_agency_ids_csv="$(IFS=,; echo "${AGENCY1_ID},${AGENCY2_ID},${AGENCY3_ID}")"

{
  echo "AGENCY1_ID=${AGENCY1_ID}"
  echo "AGENCY2_ID=${AGENCY2_ID}"
  echo "AGENCY3_ID=${AGENCY3_ID}"
  echo "ALL_AGENCY_IDS=${all_agency_ids_csv}"
} > "${seed_env}"

success "[seed] Saved agency ids to ${seed_env}"