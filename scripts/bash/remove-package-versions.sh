#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="${1:-.}"

echo "Searching for .csproj files in: ${ROOT_DIR}"

find "$ROOT_DIR" -name "*.csproj" | while read -r file; do
  echo "→ Processing $file"
  
  perl -i -pe 'if (/<PackageReference\b/) { s/\s+Version="[^"]*"//g }' "$file"
done

echo "Script remove-package-versions.sh completed"
