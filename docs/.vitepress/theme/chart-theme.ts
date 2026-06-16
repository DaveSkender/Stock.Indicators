// Shared chart theme constants. Single source of truth for indy-charts
// annotation/axis-label backdrops so the VitePress theme switcher and the
// landing overlay stay in lockstep. Base hex matches VitePress's default
// `--vp-c-bg` values (dark `#1b1b1f`, light `#ffffff`); the `CC` alpha (~80%)
// lets a hint of the chart data show through behind axis ticks and legend
// annotations.

export const DARK_SURFACE = '#1b1b1fCC'
export const LIGHT_SURFACE = '#ffffffCC'
