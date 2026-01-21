#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
source "${SCRIPT_DIR}/../../lib/config.sh"
source "${SCRIPT_DIR}/../../lib/log.sh"

BACKEND_URL="${BACKEND_URL:-http://localhost:5055}"
HEALTH_PATH="${HEALTH_PATH:-/api/health}"
PAGE_SIZE="${SEED_PAGE_SIZE:-100}"

SEED_BROKERS_COUNT="${SEED_BROKERS_COUNT:-22}"

AGENCY1_ID="${SEED_AGENCY1_ID:-aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa}"
AGENCY2_ID="${SEED_AGENCY2_ID:-bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb}"
AGENCY3_ID="${SEED_AGENCY3_ID:-cccccccc-cccc-cccc-cccc-cccccccccccc}"

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

neutral "Seeding brokers (3 agencies)"
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
  local agency_id="$1" first="$2" last="$3" email="$4" phone="$5"
  python3 - <<PY
import json
print(json.dumps({
  "agencyId": "${agency_id}",
  "firstName": "${first}",
  "lastName": "${last}",
  "email": "${email}",
  "phoneNumber": "${phone}",
  "photoUrl": None
}))
PY
}

first_names=("Ola" "Kari" "Anders" "Ingrid" "Erik" "Nora" "Jonas" "Maja" "Lars" "Emma" "Sindre" "Hanna")
last_names=("Nordmann" "Hansen" "Johansen" "Olsen" "Larsen" "Andersen" "Nilsen" "Berg" "Haugen" "Moen" "Dahl" "Solberg")
agencies=("${AGENCY1_ID}" "${AGENCY2_ID}" "${AGENCY3_ID}")

neutral "Creating ${SEED_BROKERS_COUNT} brokers distributed across 3 agencies"

created_ids=()
i=1

while [[ "${i}" -le "${SEED_BROKERS_COUNT}" ]]; do
  first="${first_names[$(((i-1) % ${#first_names[@]}))]}"
  last="${last_names[$(((i-1) % ${#last_names[@]}))]}"
  email="broker${i}.seed@broker.no"
  phone=$(printf "+47 900 %02d %03d" $(((i-1) / 100)) $(((i-1) % 1000)))

  # distribute across agencies by round-robin: 1,2,3,1,2,3...
  agency_id="${agencies[$(((i-1) % ${#agencies[@]}))]}"

  resp="$(http_post_json "${BACKEND_URL}/api/brokers" "$(create_broker_payload "${agency_id}" "${first}" "${last}" "${email}" "${phone}")")"
  assert_json "POST /api/brokers #${i}" "${resp}"

  id="$(printf '%s' "${resp}" | json_field "id")"
  created_ids+=("${id}")

  i=$((i+1))
done

success "[seed] Brokers created: ${#created_ids[@]}"

broker1_id="${created_ids[0]}"
broker2_id="${created_ids[1]}"
broker3_id="${created_ids[2]}"

all_ids_csv="$(IFS=,; echo "${created_ids[*]}")"

{
  echo "AGENCY1_ID=${AGENCY1_ID}"
  echo "AGENCY2_ID=${AGENCY2_ID}"
  echo "AGENCY3_ID=${AGENCY3_ID}"
  echo "BROKER1_ID=${broker1_id}"
  echo "BROKER2_ID=${broker2_id}"
  echo "BROKER3_ID=${broker3_id}"
  echo "ALL_BROKER_IDS=${all_ids_csv}"
} > "${seed_env}"

success "[seed] Saved ids to ${seed_env}"
success "  agency1=${AGENCY1_ID}"
success "  agency2=${AGENCY2_ID}"
success "  agency3=${AGENCY3_ID}"
success "  broker1=${broker1_id}"
success "  broker2=${broker2_id}"
success "  broker3=${broker3_id}"
