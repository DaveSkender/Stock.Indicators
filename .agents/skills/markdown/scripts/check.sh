#!/usr/bin/env bash
# Check markdown linting tool installation and configuration

echo "=== Markdown tooling check ==="
echo ""

# markdownlint-cli2
echo "--- markdownlint-cli2 ---"
if command -v markdownlint-cli2 >/dev/null 2>&1; then
  echo "✓ markdownlint-cli2: $(markdownlint-cli2 --version)"
else
  echo "✗ markdownlint-cli2: not installed"
  echo "  Install: pnpm install -g markdownlint-cli2"
fi

# Configuration file
echo ""
echo "--- Configuration ---"
for conf in .markdownlint-cli2.jsonc .markdownlint-cli2.yaml .markdownlint-cli2.yml .markdownlint-cli2.json .markdownlint.json .markdownlint.yaml .markdownlint.yml; do
  if [[ -f "$conf" ]]; then
    echo "✓ Config found: $conf"
    break
  fi
done

if ! compgen -G ".markdownlint*" >/dev/null 2>&1; then
  echo "✗ No markdownlint config found"
  echo "  Create: .markdownlint-cli2.jsonc"
fi

# VS Code extension
echo ""
echo "--- VS Code ---"
if command -v code >/dev/null 2>&1; then
  if code --list-extensions 2>/dev/null | grep -q "DavidAnson.vscode-markdownlint"; then
    echo "✓ vscode-markdownlint extension installed"
  else
    echo "✗ vscode-markdownlint extension not installed"
    echo "  Install: code --install-extension DavidAnson.vscode-markdownlint"
  fi
else
  echo "  VS Code CLI not in PATH — skipping extension check"
fi

echo ""
echo "=== Check complete ==="
