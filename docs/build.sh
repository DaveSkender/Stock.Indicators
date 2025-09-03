#!/bin/bash

# Build the MkDocs site
echo "Building MkDocs site..."
cd /workspaces/Stock.Indicators/docs
mkdocs build

echo "MkDocs site built successfully! Files are in /docs/_site/"
