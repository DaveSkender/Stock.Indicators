#!/usr/bin/env bash
# Delete specific folders (supports globs and subfolder paths) and files, skipping protected ones
# then does a clean restore of packages.

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
  "packages.lock.json"
  "package-lock.json"
  "*.tmp"
  "*.bak"
)

SKIPPED_FOLDERS=(
  ".git"
  ".vs"
  "*copilot*"
  # add more protected folders as needed
)

# start from root, basic dotnet clean
echo ""
echo "=== Deleting caches and lock files ==="
cd "$(dirname "$0")/../.." || exit 1
dotnet clean

# Delete folders (bypasses SKIPPED_FOLDERS)
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
for file in "${DELETE_FILES[@]}"; do
  # Build find argument array with exclusions
  find_args=(".")
  for ignore in "${SKIPPED_FOLDERS[@]}"; do
    find_args+=("(" "-path" "./$ignore" "-prune" ")" "-o")
  done
  find_args+=("(" "-type" "f" "-name" "$file" ")" "-exec" "rm" "-f" "{}" "+")
  find "${find_args[@]}" 2>/dev/null || true
done

# restore
echo ""
echo "=== Restoring caches ==="
dotnet restore
npm install

echo ""
echo "✓ Cleanup completed!"
