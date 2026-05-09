#!/usr/bin/env bash
# Install markdown linting tools

set -e  # Exit on error

echo "Installing markdown linting tools..."

# Install markdownlint-cli2 globally
if ! command -v markdownlint-cli2 >/dev/null 2>&1; then
  echo "Installing markdownlint-cli2..."

  # Check if pnpm is available
  if command -v pnpm >/dev/null 2>&1; then
    pnpm add -g markdownlint-cli2
  elif command -v npm >/dev/null 2>&1; then
    npm install -g markdownlint-cli2
  else
    echo "Error: pnpm or npm required to install markdownlint-cli2"
    exit 1
  fi
else
  echo "markdownlint-cli2 already installed ($(markdownlint-cli2 --version))"
fi

# Verify installation
echo ""
echo "Verifying installation..."

if command -v markdownlint-cli2 >/dev/null 2>&1; then
  echo "✓ markdownlint-cli2: $(markdownlint-cli2 --version)"
else
  echo "✗ markdownlint-cli2 not found"
  exit 1
fi

echo ""
echo "Installation complete!"
echo ""
echo "Next steps:"
echo "1. Create .markdownlint-cli2.jsonc configuration"
echo "2. Test linting: markdownlint-cli2 '**/*.md'"
echo "3. Auto-fix issues: markdownlint-cli2 --fix '**/*.md'"
