# Dev Container Configuration

This directory contains the configuration for the Stock Indicators development container, optimized for GitHub Codespaces and VS Code Remote - Containers.

## Features

### Shell Environment

- **Zsh with Oh My Zsh**: Modern shell with better autocompletion and theming
- **Useful aliases**: Pre-configured shortcuts for common commands
- **Custom prompt**: Enhanced terminal experience

### Development Tools

- **.NET SDK 9.0**: Latest .NET development environment
- **Node.js LTS**: For tooling and documentation builds
- **Ruby 3.3**: For Jekyll documentation site
- **Python UV**: For Spec Kit workflow tools
- **GitHub CLI**: Command-line access to GitHub features

### VS Code Extensions

The container pre-installs essential extensions:

- **C# Development**: IntelliCode, C# extension
- **Markdown**: Preview and linting
- **Git**: GitLens for enhanced git features
- **GitHub**: Copilot and Actions support
- **Code Quality**: EditorConfig, spell checker

### Environment Optimizations

- **Fast startup**: Optimized installation with minimal output
- **Persistent config**: `.config` directory mounted for settings persistence
- **Git configuration**: Pre-configured with sensible defaults
- **Ruby gems**: Automatic Jekyll setup for documentation

## Quick Commands

The container includes pre-configured aliases:

### .NET Shortcuts

```bash
dn          # dotnet
dnb         # dotnet build
dnt         # dotnet test
dnr         # dotnet restore
dnf         # dotnet format
```

### Git Shortcuts

```bash
gs          # git status
gd          # git diff
gl          # git log --oneline -10
gp          # git pull
```

### Navigation

```bash
goto-src    # cd to src directory
goto-tests  # cd to tests directory
goto-docs   # cd to docs directory
```

### Other Helpers

```bash
ll          # ls -alF (detailed list)
..          # cd .. (go up one directory)
...         # cd ../.. (go up two directories)
```

## Usage

### With GitHub Codespaces

1. Click "Code" → "Create codespace on [branch]"
2. Wait for container to build and configure (~2-3 minutes)
3. Start developing!

### With VS Code Remote - Containers

1. Install "Remote - Containers" extension
2. Open repository in VS Code
3. Click "Reopen in Container" when prompted
4. Wait for container to build and configure

## Customization

### Adding Your Git Config

The startup script sets default git credentials. To use your own:

```bash
git config --global user.name "Your Name"
git config --global user.email "your.email@example.com"
```

### Adding Custom Aliases

Edit your `~/.zshrc` file:

```bash
echo "alias myalias='my command'" >> ~/.zshrc
source ~/.zshrc
```

### Installing Additional Tools

Add to `startup.sh` or install manually:

```bash
# NPM packages
npm install --global <package-name>

# .NET tools
dotnet tool install --global <tool-name>

# Python tools with UV
uv tool install <package-name>
```

## Troubleshooting

### Container won't start

- Check Docker daemon is running
- Verify you have sufficient disk space
- Try rebuilding: "Remote-Containers: Rebuild Container"

### Missing tools

- Run startup script manually: `.devcontainer/startup.sh`
- Check tool installation logs in terminal

### Performance issues

- Ensure Docker has adequate resources (4GB+ RAM recommended)
- Close unnecessary VS Code extensions
- Use workspace trust features to limit background tasks

## Files

- `devcontainer.json` - Container configuration and VS Code settings
- `startup.sh` - Post-creation initialization script
- `README.md` - This documentation file

## Resources

- [Dev Containers Documentation](https://containers.dev/)
- [VS Code Remote Development](https://code.visualstudio.com/docs/remote/remote-overview)
- [GitHub Codespaces](https://github.com/features/codespaces)
- [Stock Indicators Contributing Guide](../docs/contributing.md)
