#!/usr/bin/env bash
set -euo pipefail

# shellcheck source=./colors.sh
source "$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)/colors.sh"

info() {
  echo -e "${LIGHT_BLUE}$*${RESET}"
}

success() {
  echo -e "${GREEN}$*${RESET}"
}

warn() {
  echo -e "${YELLOW}$*${RESET}"
}

error() {
  echo -e "${RED}$*${RESET}" >&2
}

highlight() {
  echo -e "${PURPLE}→ $*${RESET}"
}
