#!/bin/bash

# Create new feature script for Stock Indicators
# Generates branch name and spec file path for new indicator features

set -e

# Parse JSON arguments if provided
if [ "$1" == "--json" ] && [ -n "$2" ]; then
    ARGS="$2"
else
    ARGS="$*"
fi

# Generate branch name from feature description
# Convert to lowercase, replace spaces with hyphens, remove special chars
BRANCH_NAME="feature/$(echo "$ARGS" | tr '[:upper:]' '[:lower:]' | sed 's/[^a-z0-9 ]//g' | sed 's/ /-/g' | sed 's/--*/-/g' | sed 's/^-\|-$//g' | cut -c1-50)"

# Ensure we're in the repo root
REPO_ROOT=$(git rev-parse --show-toplevel)
cd "$REPO_ROOT"

# Create specs directory if it doesn't exist
mkdir -p spec-kit/specs

# Generate spec file path
SPEC_FILE="$REPO_ROOT/spec-kit/specs/${BRANCH_NAME#feature/}.md"

# Create and checkout new branch
git checkout -b "$BRANCH_NAME" 2>/dev/null || git checkout "$BRANCH_NAME"

# Initialize spec file if it doesn't exist
if [ ! -f "$SPEC_FILE" ]; then
    touch "$SPEC_FILE"
fi

# Output JSON for the spec-kit command
echo "{\"BRANCH_NAME\": \"$BRANCH_NAME\", \"SPEC_FILE\": \"$SPEC_FILE\"}"