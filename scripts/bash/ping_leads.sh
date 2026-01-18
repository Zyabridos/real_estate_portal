#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
source "${SCRIPT_DIR}/../lib/config.sh"
source "${SCRIPT_DIR}/../lib/log.sh"

echo ""
info "Fetching leads list..."
curl -i "${BACKEND_URL}/api/leads?page=1&pageSize=10"


