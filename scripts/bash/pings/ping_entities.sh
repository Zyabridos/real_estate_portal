#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
source "${SCRIPT_DIR}/../../lib/config.sh"
source "${SCRIPT_DIR}/../../lib/log.sh"

ENTITY="${1:-}"

echo ""
if [ -z "$ENTITY" ]; then
  warn "Usage: ping_entities.sh <entity>"
  highlight "Example: make ping-entities ENTITY=properties"
  exit 1
fi

info "Fetching ${ENTITY} list..."
curl -i "${BACKEND_URL}/api/${ENTITY}?page=1&pageSize=10"
