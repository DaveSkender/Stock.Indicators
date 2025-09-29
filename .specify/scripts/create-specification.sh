#!/bin/bash

# Create specification script for Stock Indicators
# Generates a new specification directory and initializes spec.md

set -e

# Parse arguments
FEATURE_DESCRIPTION="$*"
REPO_ROOT=$(git rev-parse --show-toplevel)

if [ -z "$FEATURE_DESCRIPTION" ]; then
    echo "Error: Feature description required"
    exit 1
fi

# Generate spec ID (find next available number)
NEXT_ID=$(find "$REPO_ROOT/specs" -maxdepth 1 -name "[0-9]*-*" | wc -l)
NEXT_ID=$((NEXT_ID + 1))
SPEC_ID=$(printf "%03d" $NEXT_ID)

# Generate directory name from feature description
DIR_NAME=$(echo "$FEATURE_DESCRIPTION" | tr '[:upper:]' '[:lower:]' | sed 's/[^a-z0-9 ]//g' | sed 's/ /-/g' | cut -c1-50)
SPEC_DIR="$REPO_ROOT/specs/$SPEC_ID-$DIR_NAME"

# Create specification directory
mkdir -p "$SPEC_DIR"

# Initialize spec.md file
cat > "$SPEC_DIR/spec.md" << EOF
# Specification: $FEATURE_DESCRIPTION

## Overview

$FEATURE_DESCRIPTION

## Mathematical Foundation

[TODO: Add mathematical formulation]

## Technical Requirements

[TODO: Add technical requirements]

## API Design

[TODO: Add API design details]

## Validation Criteria

[TODO: Add validation criteria]

---
Created: $(date -I)
Specification ID: $SPEC_ID
EOF

echo "Created specification: $SPEC_DIR"
echo "Spec file: $SPEC_DIR/spec.md"
echo "Ready for /plan command"