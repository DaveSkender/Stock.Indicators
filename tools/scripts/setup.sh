#!/bin/bash

# Dev Environment Startup Script for Stock Indicators
# Handles initialization of development dependencies on macOS

set -e

echo "🚀 Starting Stock Indicators dev environment setup..."

# ─── Ruby / Jekyll ─────────────────────────────────────────────

echo "🔍 Verifying Ruby environment..."
ruby -v

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
