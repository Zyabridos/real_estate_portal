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
  error "[seed] Missing ${seed_env}. Run: make seed-properties"
  exit 1
fi

source "${seed_env}"

: "${PROPERTY_ID:?PROPERTY_ID missing}"

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

neutral "Seeding leads"
neutral "Checking API health: ${BACKEND_URL}${HEALTH_PATH}"
curl -fsS "${BACKEND_URL}${HEALTH_PATH}" >/dev/null
success "[seed] API health OK"

neutral "Clearing existing leads"
leads_json="$(curl -fsS "${BACKEND_URL}/api/leads?page=1&pageSize=${PAGE_SIZE}" || true)"

lead_ids=""
if [[ -n "${leads_json}" ]] && printf '%s' "$leads_json" | python3 -c 'import sys,json; json.load(sys.stdin)' >/dev/null 2>&1; then
  lead_ids="$(printf '%s' "${leads_json}" | extract_ids_from_paged || true)"
fi

if [[ -n "${lead_ids}" ]]; then
  while IFS= read -r id; do
    [[ -z "$id" ]] && continue
    curl -fsS -X DELETE "${BACKEND_URL}/api/leads/${id}" >/dev/null || true
  done <<< "${lead_ids}"
  success "[seed] Leads cleared: $(echo "${lead_ids}" | wc -l | tr -d ' ')"
else
  warn "[seed] No leads to clear"
fi

lead_payload() {
  local full_name="$1" email="$2" phone="$3" message="$4"
  python3 - <<PY
import json
email = "${email}".strip() or None
phone = "${phone}".strip() or None
print(json.dumps({
  "propertyId": "${PROPERTY_ID}",
  "fullName": "${full_name}",
  "email": email,
  "phoneNumber": phone,
  "message": "${message}"
}))
PY
}

neutral "Creating leads (linked to propertyId=${PROPERTY_ID})"
http_post_json "${BACKEND_URL}/api/leads" "$(lead_payload "Seed User EmailOnly" "seed.emailonly@example.com" "" "Seed lead (email only).")" >/dev/null
http_post_json "${BACKEND_URL}/api/leads" "$(lead_payload "Seed User PhoneOnly" "" "+47 999 88 777" "Seed lead (phone only).")" >/dev/null
http_post_json "${BACKEND_URL}/api/leads" "$(lead_payload "Seed User Both" "seed.both@example.com" "+47 111 22 333" "Seed lead (both email and phone).")" >/dev/null

success "[seed] Leads created"

neutral "Verifying leads list"
check="$(curl -fsS "${BACKEND_URL}/api/leads?page=1&pageSize=10")"
assert_json "GET /api/leads verify" "$check"
printf '%s' "$check" | python3 -c 'import sys,json; d=json.load(sys.stdin); print("[seed] leads.totalCount =", d.get("totalCount"))'

success "[seed] Seed completed"
