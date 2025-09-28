# Spec-Kit Integration for Stock Indicators

This directory contains the GitHub Spec-Kit integration for Spec-Driven Development of technical indicators.

## Quick Start

### Using GitHub Copilot

1. Open GitHub Copilot Chat in VS Code
2. Use spec-kit commands directly:
   - `/constitution` - Review project principles
   - `/specify Build a new RSI indicator`
   - `/plan Use optimal precision with streaming support`
   - `/tasks` - Generate implementation tasks
   - `/implement` - Execute the plan

### Manual Usage

1. Install spec-kit CLI: `uv tool install specify-cli --from git+https://github.com/github/spec-kit.git`
2. Run: `specify check` to verify setup
3. Use the templates and scripts in this directory

## Directory Structure

- `memory/constitution.md` - Project governance principles
- `templates/` - Spec-kit command templates
- `scripts/` - Helper scripts for feature creation
- `specs/` - Generated feature specifications (created as needed)

## Documentation

See [Spec-Kit Integration Guide](../.github/spec-kit-integration.md) for complete usage instructions.
