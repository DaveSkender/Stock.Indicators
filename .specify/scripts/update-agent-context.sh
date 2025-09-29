#!/bin/bash

# Update agent context script for Stock Indicators
# Updates GitHub Copilot instructions with current project state

set -e

AGENT_TYPE=${1:-copilot}
REPO_ROOT=$(git rev-parse --show-toplevel)

echo "Updating ${AGENT_TYPE} agent context..."

if [ "$AGENT_TYPE" = "copilot" ]; then
    # Update copilot instructions with current tech stack from specs
    TARGET_FILE="$REPO_ROOT/.github/copilot-instructions.md"
    SOURCE_FILE="$REPO_ROOT/.specify/.github/copilot-instructions.md"
    
    if [ -f "$SOURCE_FILE" ]; then
        cp "$SOURCE_FILE" "$TARGET_FILE"
        echo "✅ Updated $TARGET_FILE"
    else
        echo "⚠️  Source file not found: $SOURCE_FILE"
    fi
else
    echo "⚠️  Unknown agent type: $AGENT_TYPE"
    exit 1
fi

echo "Agent context update complete."