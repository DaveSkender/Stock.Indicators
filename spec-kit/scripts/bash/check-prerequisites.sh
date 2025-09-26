#!/bin/bash

# Check prerequisites script for Stock Indicators tasks generation
# Generates feature directory and available documents list

set -e

# Parse JSON arguments if provided
if [ "$1" == "--json" ]; then
    OUTPUT_JSON=true
else
    OUTPUT_JSON=false
fi

# Get repository root
REPO_ROOT=$(git rev-parse --show-toplevel)
cd "$REPO_ROOT"

# Feature directory (for v3 streaming indicators)
FEATURE_DIR="$REPO_ROOT/spec-kit/specs"

# Check for available design documents
AVAILABLE_DOCS=()

# Check for core specification
if [ -f "$FEATURE_DIR/v3-streaming-indicators.md" ]; then
    AVAILABLE_DOCS+=("v3-streaming-indicators.md")
fi

# Check for constitution
if [ -f "$REPO_ROOT/spec-kit/memory/constitution.md" ]; then
    AVAILABLE_DOCS+=("constitution.md")
fi

# Check for existing tasks
if [ -f "$FEATURE_DIR/v3-streaming-tasks.md" ]; then
    AVAILABLE_DOCS+=("v3-streaming-tasks.md")
fi

# Output results
if [ "$OUTPUT_JSON" = true ]; then
    # Generate JSON output for spec-kit command
    echo "{"
    echo "  \"FEATURE_DIR\": \"$FEATURE_DIR\","
    echo "  \"AVAILABLE_DOCS\": ["
    for i in "${!AVAILABLE_DOCS[@]}"; do
        if [ $i -eq 0 ]; then
            echo "    \"${AVAILABLE_DOCS[$i]}\""
        else
            echo "    ,\"${AVAILABLE_DOCS[$i]}\""
        fi
    done
    echo "  ]"
    echo "}"
else
    # Human-readable output
    echo "Feature Directory: $FEATURE_DIR"
    echo "Available Documents:"
    for doc in "${AVAILABLE_DOCS[@]}"; do
        echo "  - $doc"
    done
fi