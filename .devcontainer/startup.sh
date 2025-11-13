#!/bin/bash

# Dev Container Startup Script for Stock Indicators
# Handles initialization of development dependencies

echo "🚀 Starting Stock Indicators dev container setup..."

# Verify .NET is available
echo "🔍 Verifying .NET environment..."
dotnet --version

echo "🧰 Installing .NET-based tools..."
dotnet tool install --global dotnet-format
dotnet tool install --global roslynator.dotnet.cli
dotnet tool install --global dotnet-outdated-tool
dotnet tool list --global

# Restore .NET packages
echo "📦 Restoring .NET packages..."
dotnet restore

# --- Make dotnet global tool shims available system-wide (fix for postCreateCommand PATH issues) ---
DOTNET_TOOLS_DIR="${HOME}/.dotnet/tools"
PROFILE_D="/etc/profile.d/dotnet-tools.sh"

if [ -d "$DOTNET_TOOLS_DIR" ]; then
  echo "🔧 Registering dotnet global tools ($DOTNET_TOOLS_DIR) system-wide..."

  # Persist PATH for future login/non-login shells
  cat <<'EOF' | sudo tee "$PROFILE_D" >/dev/null
# Add dotnet global tools to PATH
export PATH="$HOME/.dotnet/tools:$PATH"
EOF
  sudo chmod +x "$PROFILE_D" || true

  # Symlink individual tool shims into /usr/local/bin so commands work immediately
  for shim in "$DOTNET_TOOLS_DIR"/*; do
    if [ -f "$shim" ] && [ -x "$shim" ]; then
      sudo ln -sfn "$shim" "/usr/local/bin/$(basename "$shim")" || true
    fi
  done

  # Ensure current non-interactive shell can use the tools immediately
  export PATH="$DOTNET_TOOLS_DIR:$PATH"
fi

echo "✅ Dev environment setup complete!"
