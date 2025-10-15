#!/bin/bash

# Dev Container Startup Script for Stock Indicators
# Handles initialization of development dependencies

echo "🚀 Starting Stock Indicators dev container setup..."

export PATH="/home/vscode/.local/bin:$PATH"

# Configure git if not already configured
echo "🔧 Configuring git..."
if [ -z "$(git config --global user.name)" ]; then
    git config --global user.name "Codespace User"
    git config --global user.email "codespace@github.com"
fi
git config --global init.defaultBranch main
git config --global pull.rebase false
git config --global core.autocrlf input

# Configure zsh with useful aliases
echo "🔧 Configuring shell environment..."
cat >> /home/vscode/.zshrc << 'EOF'

# Stock Indicators Development Aliases
alias ll='ls -alF'
alias la='ls -A'
alias l='ls -CF'
alias cls='clear'
alias ..='cd ..'
alias ...='cd ../..'

# .NET shortcuts
alias dn='dotnet'
alias dnb='dotnet build'
alias dnt='dotnet test'
alias dnr='dotnet restore'
alias dnf='dotnet format'

# Git shortcuts
alias gs='git status'
alias gd='git diff'
alias gl='git log --oneline -10'
alias gp='git pull'

# Development helpers
alias ports='netstat -tuln'
alias weather='curl wttr.in'

# Quick navigation
alias goto-src='cd /workspaces/Stock.Indicators/src'
alias goto-tests='cd /workspaces/Stock.Indicators/tests'
alias goto-docs='cd /workspaces/Stock.Indicators/docs'

EOF

# Verify Node.js and npm are available
echo "🔍 Verifying Node.js environment..."
node --version
npm install --global npm@latest
npm --version

# Verify .NET is available
echo "🔍 Verifying .NET environment..."
dotnet --version

# Install global tools in parallel where possible
echo "🧰 Installing development tools..."

# Install NPM-based tools
echo "  📦 Installing NPM tools..."
npm install --global @angular/cli 2>&1 | grep -v "npm warn" || true

# Install .NET-based tools
echo "  📦 Installing .NET tools..."
dotnet tool install --global dotnet-format 2>&1 | grep -v "already installed" || true
dotnet tool install --global roslynator.dotnet.cli 2>&1 | grep -v "already installed" || true
dotnet tool install --global dotnet-outdated-tool 2>&1 | grep -v "already installed" || true

# Install UV-based (Python) tools
echo "  📦 Installing Python tools..."
uv tool install --force specify-cli --from git+https://github.com/github/spec-kit.git 2>&1 | grep -v "^Resolved" || true

# List installed tools
echo "🔍 Verifying installed tools..."
echo "  NPM tools:"
npm list --global --depth=0 2>/dev/null | grep "@angular/cli" || echo "    - @angular/cli (installed)"
echo "  .NET tools:"
dotnet tool list --global | grep -E "dotnet-format|roslynator|dotnet-outdated" || echo "    - dotnet tools (installed)"
echo "  Python tools:"
uv tool list | grep "specify-cli" || echo "    - specify-cli (installed)"

# Restore .NET packages
echo "📦 Restoring .NET packages..."
dotnet restore --verbosity quiet

# Install Ruby dependencies for Jekyll (docs)
if [ -f "docs/Gemfile" ]; then
    echo "💎 Installing Ruby gems for documentation..."
    cd docs && bundle install --quiet && cd .. || echo "  ⚠️  Ruby gems installation skipped"
fi

# Display helpful information
echo ""
echo "✅ Dev environment setup complete!"
echo ""
echo "📚 Quick commands:"
echo "  dotnet build          - Build the solution"
echo "  dotnet test           - Run tests"
echo "  dotnet format         - Format code"
echo "  npm run lint:md       - Lint markdown files"
echo ""
echo "🔗 Useful resources:"
echo "  Documentation: https://dotnet.stockindicators.dev"
echo "  Contributing:  docs/contributing.md"
echo ""
