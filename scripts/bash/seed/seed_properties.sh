#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
source "${SCRIPT_DIR}/../../lib/config.sh"
source "${SCRIPT_DIR}/../../lib/log.sh"

BACKEND_URL="${BACKEND_URL:-http://localhost:5055}"
HEALTH_PATH="${HEALTH_PATH:-/api/health/readiness}"
PAGE_SIZE="${SEED_PAGE_SIZE:-100}"
SEED_PROPERTIES_COUNT="${SEED_PROPERTIES_COUNT:-50}"

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

# shellcheck disable=SC1090
source "${seed_env}"

: "${ALL_BROKER_IDS:?Missing ALL_BROKER_IDS. Run: make seed-brokers}"

assert_json() {
  local label="$1"
  local body="$2"

  if [[ -z "$body" ]]; then
    error "[seed] ERROR: ${label} returned empty response."
    exit 1
  fi

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
    if _id is not None:
        print(_id)
'
}

json_field() {
  local field="$1"
  python3 -c "import sys,json; print(json.load(sys.stdin).get('${field}', ''))"
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

create_property_payload() {
  local title="$1"
  local city="$2"
  local price="$3"
  local broker_id="$4"
  local type="$5"
  local status="$6"
  local main_image_url="$7"
  local image_urls_json="$8"

  python3 - "$title" "$city" "$price" "$broker_id" "$type" "$status" "$main_image_url" "$image_urls_json" <<'PY'
import json
import sys

title, city, price, broker_id, ptype, status, main_image_url, image_urls_json = sys.argv[1:]

main_image_url = None if main_image_url == "" else main_image_url
image_urls = json.loads(image_urls_json)

print(json.dumps({
  "title": title,
  "description": "Seed property description",
  "address": "Seed Street 1",
  "city": city,
  "price": float(price),
  "type": ptype,
  "status": status,
  "bedrooms": 2,
  "bathrooms": 1,
  "area": 55.5,
  "mainImageUrl": main_image_url,
  "imageUrls": image_urls,
  "brokerId": int(broker_id)
}))
PY
}

neutral "Seeding properties"
neutral "Checking API health: ${BACKEND_URL}${HEALTH_PATH}"
curl -fsS "${BACKEND_URL}${HEALTH_PATH}" >/dev/null
success "[seed] API health OK"

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

IFS=',' read -r -a broker_ids <<< "${ALL_BROKER_IDS}"
broker_count="${#broker_ids[@]}"

if [[ "${broker_count}" -ne 22 ]]; then
  warn "[seed] Expected 22 brokers, got ${broker_count}. Will still proceed."
fi

if [[ "${broker_count}" -lt 22 ]]; then
  error "[seed] Need 22 brokers before seeding properties."
  exit 1
fi

cities=("Oslo" "Bergen" "Stavanger" "Trondheim" "Drammen" "Elverum")
types=("Apartment" "House" "Commercial")
statuses=("Active" "Sold")

PROPERTY1_IMAGE_1="https://commons.wikimedia.org/wiki/Special:Redirect/file/Modern_apartment_building_%28Unsplash%29.jpg"
PROPERTY1_IMAGE_2="https://commons.wikimedia.org/wiki/Special:Redirect/file/Brown_apartment_building_%28Unsplash%29.jpg"
PROPERTY1_IMAGE_3="https://commons.wikimedia.org/wiki/Special:Redirect/file/White_apartment_building_%28Unsplash%29.jpg"
PROPERTY1_IMAGE_4="https://commons.wikimedia.org/wiki/Special:Redirect/file/Apartment_Building_Yellow_Wall_%28Unsplash%29.jpg"

PROPERTY1_IMAGES_JSON="$(python3 - <<'PY'
import json
print(json.dumps([
  "https://commons.wikimedia.org/wiki/Special:Redirect/file/Modern_apartment_building_%28Unsplash%29.jpg",
  "https://commons.wikimedia.org/wiki/Special:Redirect/file/Brown_apartment_building_%28Unsplash%29.jpg",
  "https://commons.wikimedia.org/wiki/Special:Redirect/file/White_apartment_building_%28Unsplash%29.jpg",
  "https://commons.wikimedia.org/wiki/Special:Redirect/file/Apartment_Building_Yellow_Wall_%28Unsplash%29.jpg"
]))
PY
)"

PROPERTY2_MAIN_IMAGE="https://commons.wikimedia.org/wiki/Special:Redirect/file/Architecture-villa-house-building-home-construction-542165.jpg"
EMPTY_IMAGES_JSON='[]'

neutral "Creating ${SEED_PROPERTIES_COUNT} properties"

created_ids=()
i=1

for broker_index in "${!broker_ids[@]}"; do
  broker_id="${broker_ids[$broker_index]}"

  # Последний брокер без объектов
  if [[ "${broker_index}" -eq 21 ]]; then
    continue
  fi

  # Первые 8 брокеров -> по 3 объекта
  # Остальные 13 брокеров -> по 2 объекта
  if [[ "${broker_index}" -lt 8 ]]; then
    per_broker=3
  else
    per_broker=2
  fi

  for ((j=1; j<=per_broker; j++)); do
    city="${cities[$(( (i-1) % ${#cities[@]} ))]}"
    price=$((2500000 + (i * 150000)))
    type="${types[$(( (i-1) % ${#types[@]} ))]}"
    status="${statuses[$(( (i-1) % ${#statuses[@]} ))]}"

    main_image_url=""
    image_urls_json="${EMPTY_IMAGES_JSON}"

    if [[ "${i}" -eq 1 ]]; then
      main_image_url="${PROPERTY1_IMAGE_1}"
      image_urls_json="${PROPERTY1_IMAGES_JSON}"
    elif [[ "${i}" -eq 2 ]]; then
      main_image_url="${PROPERTY2_MAIN_IMAGE}"
      image_urls_json="${EMPTY_IMAGES_JSON}"
    fi

    resp="$(http_post_json "${BACKEND_URL}/api/properties" "$(
      create_property_payload \
        "Seed ${type} #${i} (${status})" \
        "${city}" \
        "${price}" \
        "${broker_id}" \
        "${type}" \
        "${status}" \
        "${main_image_url}" \
        "${image_urls_json}"
    )")"

    assert_json "POST /api/properties #${i}" "${resp}"

    id="$(printf '%s' "${resp}" | json_field "id")"
    created_ids+=("${id}")

    i=$((i + 1))
  done
done

created_count="${#created_ids[@]}"

if [[ "${created_count}" -ne 50 ]]; then
  error "[seed] Expected 50 properties, created ${created_count}"
  exit 1
fi

property1_id="${created_ids[0]:-}"
property2_id="${created_ids[1]:-}"
property3_id="${created_ids[2]:-}"
all_property_ids_csv="$(IFS=,; echo "${created_ids[*]}")"

grep -Ev '^(PROPERTY_ID|PROPERTY1_ID|PROPERTY2_ID|PROPERTY3_ID|ALL_PROPERTY_IDS)=' "${seed_env}" > "${seed_env}.tmp" || true
mv "${seed_env}.tmp" "${seed_env}"

{
  echo "PROPERTY_ID=${property1_id}"
  echo "PROPERTY1_ID=${property1_id}"
  echo "PROPERTY2_ID=${property2_id}"
  echo "PROPERTY3_ID=${property3_id}"
  echo "ALL_PROPERTY_IDS=${all_property_ids_csv}"
} >> "${seed_env}"

success "[seed] Properties created: ${created_count}"
success "[seed] property1=${property1_id}"
success "[seed] property2=${property2_id}"
success "[seed] property3=${property3_id}"
success "[seed] Saved property ids to ${seed_env}"