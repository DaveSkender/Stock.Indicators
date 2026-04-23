#!/bin/bash

# Dev Environment Startup Script for Stock Indicators
# Handles initialization of development dependencies on macOS

set -e

echo "🚀 Starting Stock Indicators dev environment setup..."

# ─── Ruby / Jekyll ─────────────────────────────────────────────

echo "🔍 Setting up Ruby environment..."

# Ensure Homebrew Ruby 3.3 is used on macOS (system Ruby 2.6 is too old; Ruby 4.x too new for some gems)
if command -v brew &>/dev/null; then
  # Prefer pinned ruby@3.3 over the latest formula
  if ! brew list ruby@3.3 &>/dev/null; then
    echo "   Installing ruby@3.3 via Homebrew..."
    brew install ruby@3.3
  fi
  BREW_RUBY_BIN="$(brew --prefix ruby@3.3 2>/dev/null)/bin"
  if [[ ! -x "${BREW_RUBY_BIN}/ruby" ]]; then
    BREW_RUBY_BIN="$(brew --prefix ruby 2>/dev/null)/bin"
  fi
  export PATH="${BREW_RUBY_BIN}:${PATH}"
elif command -v rbenv &>/dev/null; then
  echo "   Using rbenv Ruby version manager"
  eval "$(rbenv init -)"
  RUBY_VERSION=$(cat "$(dirname "$0")/../docs/.ruby-version" 2>/dev/null || echo "3.3")
  if ! rbenv versions | grep -q "$RUBY_VERSION"; then
    echo "   Installing Ruby $RUBY_VERSION via rbenv..."
    rbenv install "$RUBY_VERSION"
  fi
else
  echo "❌ Neither Homebrew nor rbenv is available. Install one to manage Ruby." >&2
  exit 1
fi

echo "   Ruby: $(ruby --version)"

# Install/update bundler
echo "📦 Installing bundler..."
gem install bundler --no-document

# Install Ruby gems for docs site
echo "💎 Installing Ruby gems..."
cd "$(dirname "$0")/../docs"
BUNDLE_GEMFILE="$(pwd)/Gemfile" BUNDLE_PATH="$(pwd)/vendor/bundle" \
  bundle install --jobs 4
bundle clean --force

# ─── .NET ──────────────────────────────────────────────────────

echo ""
echo "🔍 Verifying .NET environment..."
dotnet --version

echo "🧰 Installing .NET tools..."
cd "$(dirname "$0")/.."
dotnet tool restore

# Refresh git repo
echo "🗂️ Fetching from git..."
git fetch 2>&1 && git pull 2>&1 || echo "   (No remote branch or already up to date)"

# Restore .NET packages
echo "📦 Restoring .NET packages..."
dotnet restore

echo ""
echo "✅ Dev environment setup complete!"
