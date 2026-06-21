# Documentation website

Visit [dotnet.stockindicators.dev](https://dotnet.stockindicators.dev) to read our documentation.

It is developed with Jekyll for GitHub Pages and is not intended to be read from the code repo.

## Contributing to documentation

When adding or updating indicators:

- Add or update the `/docs/_indicators/` documentation files.
- Page image assets go here: `/docs/assets/` and can be optimized to `webp` format using [ImageMagick](https://imagemagick.org) or the [cwebp Encoder CLI](https://developers.google.com/speed/webp/docs/cwebp) and a command like `cwebp -resize 832 0 -q 100 examples.png -o examples-832.webp`

This site uses [Jekyll](https://jekyllrb.com) construction with _Front Matter_.
Our documentation site code is in the `docs` folder.
See [docs/README.md] for more information about setup and usage.

Build the site locally to test that it works properly.
See [Ruby Jekyll documentation](https://jekyllrb.com/docs) for initial setup.

```bash
# from /docs folder
bundle install
bundle exec jekyll serve --livereload
```

The site will be available at `http://127.0.0.1:4000`.

To update Ruby Gems:

```bash
bundle lock --add-platform x86_64-linux x64-mingw-ucrt arm64-darwin x86_64-darwin
bundle update --all
bundle clean --force
```

### Accessibility testing

- use Lighthouse in Chrome, or
- build the site locally (see above), then:

```bash
# from /docs folder
npx pa11y-ci \
  --yes
  --sitemap http://127.0.0.1:4000/sitemap.xml \
  --config ./.pa11yci
```
