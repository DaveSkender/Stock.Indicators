#!/usr/bin/env bash
# Delete specific folders (supports globs and subfolder paths) and files, skipping protected ones
# then does a clean restore of packages.

DELETE_FOLDERS=(
  ".codacy"
  "bin"
  "obj"
  "TestResults"
  "vendor"
  "docs/_site"
  "BenchmarkDotNet.Artifacts"
  "node_modules"
)

DELETE_FILES=(
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
cd "$(dirname "$0")/../.." || exit 1
dotnet clean

# Delete folders (supports globs and subfolder paths; bypasses SKIPPED_FOLDERS)
for folder in "${DELETE_FOLDERS[@]}"; do
  find . $(for ignore in "${SKIPPED_FOLDERS[@]}"; do echo -n "\\( -path \"./$ignore\" -prune \\) -o "; done) \\( -path "./$folder" \\) -exec rm -rf {} +
done

# Delete files (bypasses SKIPPED_FOLDERS)
for file in "${DELETE_FILES[@]}"; do
  find . $(for ignore in "${SKIPPED_FOLDERS[@]}"; do echo -n "\\( -path \"./$ignore\" -prune \\) -o "; done) -type f -name "$file" -exec rm -f {} +
done

# restore
dotnet restore
npm install
