#!/bin/bash
# filepath: /.devcontainer/setup.sh

# Console banner
cat << "EOF"
========================================
Stock Indicators Dev Container Setup
========================================
EOF

# Navigate to the workspace root
cd /workspaces/Stock.Indicators

# Use ANSI escape codes for yellow text
YELLOW='\033[1;33m'
NC='\033[0m' # No color

# Update .NET workloads
dotnet workload update || echo -e "${YELLOW}WARNING: Failed to update .NET workloads${NC}"

# Restore .NET dependencies
dotnet restore || echo -e "${YELLOW}WARNING: Failed to restore .NET dependencies${NC}"

# Install MkDocs
pip install mkdocs || echo -e "${YELLOW}WARNING: Failed to install MkDocs${NC}"

# Build MkDocs documentation
cd docs && python -m mkdocs build || echo -e "${YELLOW}WARNING: Failed to build MkDocs documentation${NC}"

# Build the project
cd .. && dotnet build --no-incremental || echo -e "${YELLOW}WARNING: Failed to build the project${NC}"
