#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

source "$SCRIPT_DIR/lib/colors.sh"
source "${SCRIPT_DIR}/../lib/log.sh"

BACKEND_URL="${BACKEND_URL:-http://localhost:5055}"

info "Fetching property by id..."

if [ $# -lt 1 ] || [ -z "${1:-}" ]; then
  warn "Usage: ping_property_by_id.sh <guid>"
  highlight "Oops. Seems like you are not using this command correctly."
  highlight "Here is an example: make ping-property ID=qwertyui-aaaa-bbbb-cccc-abcdefghiqwe"
  exit 1
fi

ID="$1"
OUT="$(mktemp)"
trap 'rm -f "$OUT"' EXIT

STATUS=""
if ! STATUS="$(curl -sS -o "$OUT" -w "%{http_code}" "$BACKEND_URL/api/properties/$ID")"; then
  error "Request failed: cannot reach backend at ${BACKEND_URL}"
  exit 1
fi

if [ "$STATUS" = "404" ]; then
  warn "Property not found"
  exit 0
fi

if [ "$STATUS" -ge 400 ]; then
  error "Request failed (HTTP ${STATUS})"
  cat "$OUT"
  exit 1
fi

cat "$OUT"
