#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
source "${SCRIPT_DIR}/../../lib/config.sh"
source "${SCRIPT_DIR}/../../lib/log.sh"
source "${SCRIPT_DIR}/../../lib/auth.sh"

BACKEND_URL="${BACKEND_URL:-http://localhost:5055}"
BROKERS_ENDPOINT="/api/brokers"
HEALTH_PATH="${HEALTH_PATH:-/api/health/readiness}"
PAGE_SIZE="${SEED_PAGE_SIZE:-100}"
SEED_BROKERS_COUNT="${SEED_BROKERS_COUNT:-22}"

require_cmd() {
  command -v "$1" >/dev/null 2>&1 || { error "Missing required command: $1"; exit 1; }
}

require_cmd curl
require_cmd python3

seed_dir="${SCRIPT_DIR}/../../.seed"
seed_env="${seed_dir}/seed.env"
mkdir -p "${seed_dir}"

source_seed_env_if_exists() {
  if [[ -f "${seed_env}" ]]; then
    set +u
    # shellcheck disable=SC1090
    source "${seed_env}"
    set -u
  fi
}

assert_json() {
  local label="$1"
  local body="$2"

  if [[ -z "${body}" ]]; then
    error "[seed] ERROR: ${label} returned empty response."
    exit 1
  fi

  if ! printf '%s' "${body}" | python3 -c 'import sys,json; json.load(sys.stdin)' >/dev/null 2>&1; then
    error "[seed] ERROR: ${label} did not return JSON. Response was:"
    printf '%s\n' "${body}" | head -n 120
    exit 1
  fi
}

extract_ids_from_paged() {
  python3 -c '
import sys, json
data = json.load(sys.stdin)
for x in (data.get("items") or []):
    _id = x.get("id")
    if _id is not None:
        print(_id)
'
}

json_field() {
  local field="$1"
  python3 -c "import sys,json; print(json.load(sys.stdin).get('${field}', ''))"
}

http_get_json() {
  local url="$1"
  curl -fsS \
    -H "Authorization: Bearer ${SEED_TOKEN}" \
    "$url"
}

http_post_json() {
  local url="$1"
  local payload="$2"
  local resp status

  resp="$(curl -sS -i -X POST "$url" \
    -H "Content-Type: application/json" \
    -H "Authorization: Bearer ${SEED_TOKEN}" \
    -d "$payload")"

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

  resp="$(curl -sS -i -X DELETE "$url" \
    -H "Authorization: Bearer ${SEED_TOKEN}" || true)"

  status="$(printf '%s' "$resp" | head -n 1 | awk '{print $2}')"

  if [[ "$status" =~ ^2 ]] || [[ "$status" == "404" ]]; then
    return 0
  fi

  warn "[seed] DELETE failed: $url (status=$status)"
  printf '%s\n' "$resp" | head -n 120
  return 0
}

create_broker_payload() {
  local agency_id="$1"
  local first="$2"
  local last="$3"
  local gender="$4"
  local email="$5"
  local phone="$6"

  python3 - "$agency_id" "$first" "$last" "$gender" "$email" "$phone" <<'PY'
import json, sys

agency_id = int(sys.argv[1])
first = sys.argv[2]
last = sys.argv[3]
gender = int(sys.argv[4])
email = sys.argv[5]
phone = sys.argv[6]

print(json.dumps({
  "agencyId": agency_id,
  "firstName": first,
  "lastName": last,
  "gender": gender,
  "email": email,
  "phoneNumber": phone,
  "photoUrl": None
}))
PY
}

neutral "Seeding brokers (distributed across 3 agencies)"
neutral "Checking API health: ${BACKEND_URL}${HEALTH_PATH}"
curl -fsS "${BACKEND_URL}${HEALTH_PATH}" >/dev/null
success "[seed] API health OK"

neutral "Logging in as seed admin"
SEED_TOKEN="$(seed_login)"
success "[seed] Admin token acquired"

source_seed_env_if_exists

: "${AGENCY1_ID:?Missing AGENCY1_ID. Run seed-agencies first.}"
: "${AGENCY2_ID:?Missing AGENCY2_ID. Run seed-agencies first.}"
: "${AGENCY3_ID:?Missing AGENCY3_ID. Run seed-agencies first.}"

neutral "Clearing existing brokers"
brokers_json="$(http_get_json "${BACKEND_URL}${BROKERS_ENDPOINT}?page=1&pageSize=${PAGE_SIZE}" || true)"

broker_ids=""
if [[ -n "${brokers_json}" ]] && printf '%s' "${brokers_json}" | python3 -c 'import sys,json; json.load(sys.stdin)' >/dev/null 2>&1; then
  broker_ids="$(printf '%s' "${brokers_json}" | extract_ids_from_paged || true)"
else
  warn "[seed] Skipping brokers cleanup (GET list returned non-JSON or failed)."
fi

if [[ -n "${broker_ids}" ]]; then
  while IFS= read -r id; do
    [[ -z "$id" ]] && continue
    http_delete "${BACKEND_URL}${BROKERS_ENDPOINT}/${id}" || true
  done <<< "${broker_ids}"
  success "[seed] Brokers cleared: $(echo "${broker_ids}" | wc -l | tr -d ' ')"
else
  warn "[seed] No brokers to clear"
fi

first_names=("Ola" "Kari" "Anders" "Ingrid" "Erik" "Nora" "Jonas" "Maja" "Lars" "Emma" "Sindre" "Hanna")
last_names=("Nordmann" "Hansen" "Johansen" "Olsen" "Larsen" "Andersen" "Nilsen" "Berg" "Haugen" "Moen" "Dahl" "Solberg")
genders=(0 1 2 3)
agencies=("${AGENCY1_ID}" "${AGENCY2_ID}" "${AGENCY3_ID}")

neutral "Creating ${SEED_BROKERS_COUNT} brokers distributed across 3 agencies"

created_ids=()
i=1

while [[ "${i}" -le "${SEED_BROKERS_COUNT}" ]]; do
  first="${first_names[$(((i-1) % ${#first_names[@]}))]}"
  last="${last_names[$(((i-1) % ${#last_names[@]}))]}"
  gender="${genders[$(((i-1) % ${#genders[@]}))]}"
  email="broker${i}.seed@broker.no"
  phone=$(printf "+47 900 %02d %03d" $(((i-1) / 100)) $(((i-1) % 1000)))
  agency_id="${agencies[$(((i-1) % ${#agencies[@]}))]}"

  resp="$(http_post_json \
    "${BACKEND_URL}${BROKERS_ENDPOINT}" \
    "$(create_broker_payload "${agency_id}" "${first}" "${last}" "${gender}" "${email}" "${phone}")"
  )"

  assert_json "POST ${BROKERS_ENDPOINT} #${i}" "${resp}"

  id="$(printf '%s' "${resp}" | json_field "id")"
  created_ids+=("${id}")

  i=$((i+1))
done

success "[seed] Brokers created: ${#created_ids[@]}"

broker1_id="${created_ids[0]:-}"
broker2_id="${created_ids[1]:-}"
broker3_id="${created_ids[2]:-}"
all_ids_csv="$(IFS=,; echo "${created_ids[*]}")"
all_agency_ids_csv="$(IFS=,; echo "${AGENCY1_ID},${AGENCY2_ID},${AGENCY3_ID}")"

{
  echo "AGENCY1_ID=${AGENCY1_ID}"
  echo "AGENCY2_ID=${AGENCY2_ID}"
  echo "AGENCY3_ID=${AGENCY3_ID}"
  echo "ALL_AGENCY_IDS=${all_agency_ids_csv}"
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