# Documentation website

Visit [dotnet.stockindicators.dev](https://dotnet.stockindicators.dev) to read our documentation.

This documentation site is built with [MkDocs](https://www.mkdocs.org/) using the [Material theme](https://squidfunk.github.io/mkdocs-material/).

## Developer Setup

### Prerequisites

- Python 3.8 or later
- pip (Python package manager)

### Installation

1. Clone the repository:

   ```bash
   git clone https://github.com/DaveSkender/Stock.Indicators.git
   cd Stock.Indicators/docs
   ```

2. Install required dependencies:

   ```bash
   pip install -r requirements.txt
   ```

### Local Development

To build the documentation site:

```bash
python -m mkdocs build
```

To serve the site locally with hot-reloading:

```bash
python -m mkdocs serve
```

This will start a local server at <http://127.0.0.1:8000> where you can preview the site.

Alternatively, you can use the provided scripts:

- `./build.sh` - Builds the documentation
- `./serve.sh` - Builds and serves the documentation locally

### Project Structure

- `mkdocs.yml` - Configuration file for MkDocs
- `website/` - Source Markdown files for the documentation
- `site/` - Generated HTML files (created when building)
- `requirements.txt` - Python dependencies

### Adding or Editing Content

1. Documentation content is in the `website/` directory
2. Edit Markdown files directly or add new ones
3. Update `mkdocs.yml` if adding new pages to the navigation
4. Run `python -m mkdocs serve` to preview changes locally

## CI/CD Workflows

GitHub Actions workflows handle:

- Accessibility testing
- Link checking
- Website deployment

When contributing, make sure your changes pass these checks before submitting a PR.
