// Shared chart theme constants. Single source of truth for indy-charts
// annotation/axis-label backdrops so the VitePress theme switcher and the
// landing overlay stay in lockstep. Base hex matches the `--vp-c-bg` values
// defined in `custom.scss`; the `CC` alpha (~80%) lets a hint of the chart
// data show through behind axis ticks and legend annotations.

export const DARK_SURFACE = '#22272eCC'
export const LIGHT_SURFACE = '#f3f4f6CC'
