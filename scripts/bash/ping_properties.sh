#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
source "${SCRIPT_DIR}/../lib/config.sh"
source "${SCRIPT_DIR}/../lib/log.sh"

info "Fetching properties list..."
curl -i "${BACKEND_URL}/api/properties?page=1&pageSize=10"


