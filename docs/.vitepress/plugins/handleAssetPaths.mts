// Plugin to handle absolute asset paths in markdown
export default function handleAssetPaths() {
  return {
    name: 'handle-asset-paths',
    enforce: 'pre',
    transform(code, id) {
      // Transform markdown files to replace image syntax with HTML
      if (id.endsWith('.md')) {
        // Replace ![alt](/assets/...) with <img src="/assets/..." alt="alt" />
        const transformed = code.replace(
          /!\[([^\]]*)\]\((\/assets\/[^)]+)\)/g,
          '<img src="$2" alt="$1" />'
        )
        if (transformed !== code) {
          return {
            code: transformed,
            map: null
          }
        }
      }
      return null
    },
    resolveId(id) {
      if (id.startsWith('/assets/')) {
        // During SSR, return a dummy module to prevent import errors
        return '\0virtual-asset:' + id
      }
      return null
    },
    load(id) {
      if (id.startsWith('\0virtual-asset:')) {
        // Return a dummy export for asset imports during SSR
        const path = id.replace('\0virtual-asset:', '')
        return `export default "${path}"`
      }
      return null
    }
  }
}
