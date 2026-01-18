#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
source "${SCRIPT_DIR}/../../lib/config.sh"
source "${SCRIPT_DIR}/../../lib/log.sh"

BACKEND_URL="${BACKEND_URL:-http://localhost:5055}"

ENTITY="${1:-}"
ID="${2:-}"

echo ""
if [ -z "$ENTITY" ] || [ -z "$ID" ]; then
  warn "Usage: ping_entity_by_id.sh <entity> <guid>"
  highlight "Examples:"
  highlight "  make ping-entity ENTITY=properties ID=<guid>"
  highlight "  make ping-entity ENTITY=brokers   ID=<guid>"
  highlight "  make ping-entity ENTITY=leads     ID=<guid>"
  exit 1
fi

info "Fetching ${ENTITY} by id..."

OUT="$(mktemp)"
trap 'rm -f "$OUT"' EXIT

STATUS=""
if ! STATUS="$(curl -sS -o "$OUT" -w "%{http_code}" "$BACKEND_URL/api/${ENTITY}/$ID")"; then
  error "Request failed: cannot reach backend at ${BACKEND_URL}"
  exit 1
fi

if [ "$STATUS" = "404" ]; then
  warn "$(tr '[:lower:]' '[:upper:]' <<< "${ENTITY:0:1}")${ENTITY:1} not found"
  exit 0
fi

if [ "$STATUS" -ge 400 ]; then
  error "Request failed (HTTP ${STATUS})"
  cat "$OUT"
  exit 1
fi

cat "$OUT"
