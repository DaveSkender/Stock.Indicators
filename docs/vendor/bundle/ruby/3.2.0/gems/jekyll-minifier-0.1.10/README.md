# jekyll-minifier [![Build Status](https://travis-ci.org/digitalsparky/jekyll-minifier.svg?branch=master)](https://travis-ci.org/digitalsparky/jekyll-minifier) [![Gem Version](https://badge.fury.io/rb/jekyll-minifier.svg)](http://badge.fury.io/rb/jekyll-minifier)

Requires Ruby 2.3+

Minifies HTML, XML, CSS, JSON and JavaScript both inline and as separate files utilising yui-compressor and htmlcompressor.

This was created due to the previous minifier (jekyll-press) not being CSS3 compatible, which made me frown.

Note: this is my first ever gem, I'm learning, so feedback is much appreciated.

** This minifier now only runs when JEKYLL_ENV="production" is set in the environment **

Easy to use, just install the jekyll-minifier gem:

<pre><code>gem install jekyll-minifier</code></pre>

Then add this to your \_config.yml:

<pre><code>plugins:
    - jekyll-minifier
</code></pre>

Optionally, you can also add exclusions using:

<pre><code>jekyll-minifier:
  exclude: 'atom.xml' # Exclude files from processing - file name, glob pattern or array of file names and glob patterns
</code></pre>

and toggle features and settings using:

<pre><code>jekyll-minifier:
  preserve_php: true                # Default: false
  remove_spaces_inside_tags: true   # Default: true
  remove_multi_spaces: true         # Default: true
  remove_comments: true             # Default: true
  remove_intertag_spaces: true      # Default: false
  remove_quotes: false              # Default: false
  compress_css: true                # Default: true
  compress_javascript: true         # Default: true
  compress_json: true               # Default: true
  simple_doctype: false             # Default: false
  remove_script_attributes: false   # Default: false
  remove_style_attributes: false    # Default: false
  remove_link_attributes: false     # Default: false
  remove_form_attributes: false     # Default: false
  remove_input_attributes: false    # Default: false
  remove_javascript_protocol: false # Default: false
  remove_http_protocol: false       # Default: false
  remove_https_protocol: false      # Default: false
  preserve_line_breaks: false       # Default: false
  simple_boolean_attributes: false  # Default: false
  compress_js_templates: false      # Default: false
  preserve_patterns:                # Default: (empty)
  uglifier_args:                    # Default: (empty)
</code></pre>

js_args can be found in the the uglifier documentation at listed below

Note: es6 has been implemented as experimental only via the upstream uglifier package.
See https://github.com/lautis/uglifier for more information.

To enable es6 syntax use:

<pre><code>
jekyll-minifier:
  uglifier_args:
    harmony: true

</code></pre>
