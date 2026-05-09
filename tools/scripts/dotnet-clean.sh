#!/usr/bin/env bash
set -euo pipefail

# Delete specific folders (supports globs and subfolder paths) and files, skipping protected ones.

DELETE_FOLDERS=(
  ".codacy"
  "bin"
  "obj"
  "TestResults"
  "test-results"
  "vendor"
  "docs/_site"
  "BenchmarkDotNet.Artifacts"
  "node_modules"
)

DELETE_FILES=(
  "*.tmp"
  "*.bak"
  "packages.lock.json"
)

SKIPPED_FOLDERS=(
  ".git"
  ".vs"
  "*copilot*"
  # add more protected folders as needed
)

# find root directory
ROOT_DIR="$(cd "$(dirname "$0")/../.." && pwd)"
cd "$ROOT_DIR" || exit 1

echo ""
echo "╔═══════════════════════════════════════╗"
echo "║  Cleaning .NET caches and lock files  ║"
echo "╚═══════════════════════════════════════╝"
echo "🧹 evaluating ${ROOT_DIR}..."
echo ""

echo "→ Basic solution cleaning..."
dotnet clean --nologo --verbosity quiet

# Delete folders (bypasses SKIPPED_FOLDERS)
echo "→ Deleting cache directories..."
for folder in "${DELETE_FOLDERS[@]}"; do
  # Build find argument array with exclusions
  find_args=(".")
  for ignore in "${SKIPPED_FOLDERS[@]}"; do
    find_args+=("(" "-path" "./$ignore" "-prune" ")" "-o")
  done
  find_args+=("(" "-path" "*/$folder" "-o" "-path" "./$folder" ")" "-type" "d" "-exec" "rm" "-rf" "{}" "+")
  find "${find_args[@]}" 2>/dev/null || true
done

# Delete files (bypasses SKIPPED_FOLDERS)
echo "→ Deleting temporary and lock files..."
for file in "${DELETE_FILES[@]}"; do
  # Build find argument array with exclusions
  find_args=(".")
  for ignore in "${SKIPPED_FOLDERS[@]}"; do
    find_args+=("(" "-path" "./$ignore" "-prune" ")" "-o")
  done
  find_args+=("(" "-type" "f" "-name" "$file" ")" "-exec" "rm" "-f" "{}" "+")
  find "${find_args[@]}" 2>/dev/null || true
done


echo ""
echo "✅ Cache deletes completed!"
