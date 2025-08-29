import { ScullyConfig } from '@scullyio/scully';

export const config: ScullyConfig = {
  projectRoot: "./src",
  projectName: "docs-app",
  outDir: './dist/static/',
  routes: {
    '/docs/:slug': {
      type: 'contentFolder',
      slug: {
        folder: "./src/docs"
      }
    }
  },
  defaultPostRenderers: []
};