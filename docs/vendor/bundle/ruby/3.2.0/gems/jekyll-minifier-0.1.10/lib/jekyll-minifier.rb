require 'uglifier'
require 'htmlcompressor'
require 'cssminify2'
require 'json/minify'

module Jekyll
  module Compressor
    def output_file(dest, content)
      FileUtils.mkdir_p(File.dirname(dest))
      File.open(dest, 'w') do |f|
        f.write(content)
      end
    end

    def output_compressed(path, context)
      case File.extname(path)
        when '.js'
          if path.end_with?('.min.js')
            output_file(path, context)
          else
            output_js(path, context)
          end
        when '.json'
          output_json(path, context)
        when '.css'
          if path.end_with?('.min.css')
            output_file(path, context)
          else
            output_css(path, context)
          end
        else
          output_html(path, context)
      end
    end

    def output_html(path, content)
      if ( ENV['JEKYLL_ENV'] == "production" )
        html_args = { remove_comments: true, compress_css: true, compress_javascript: true, preserve_patterns: [] }
        js_args = {}

        opts = @site.config['jekyll-minifier']
        if ( !opts.nil? )
          # Javascript Arguments
          js_args[:uglifier_args] = Hash[opts['uglifier_args'].map{|(k,v)| [k.to_sym,v]}] if opts.has_key?('uglifier_args')

          # HTML Arguments
          html_args[:remove_spaces_inside_tags]   = opts['remove_spaces_inside_tags']  if opts.has_key?('remove_spaces_inside_tags')
          html_args[:remove_multi_spaces]         = opts['remove_multi_spaces']        if opts.has_key?('remove_multi_spaces')
          html_args[:remove_comments]             = opts['remove_comments']            if opts.has_key?('remove_comments')
          html_args[:remove_intertag_spaces]      = opts['remove_intertag_spaces']     if opts.has_key?('remove_intertag_spaces')
          html_args[:remove_quotes]               = opts['remove_quotes']              if opts.has_key?('remove_quotes')
          html_args[:compress_css]                = opts['compress_css']               if opts.has_key?('compress_css')
          html_args[:compress_javascript]         = opts['compress_javascript']        if opts.has_key?('compress_javascript')
          html_args[:simple_doctype]              = opts['simple_doctype']             if opts.has_key?('simple_doctype')
          html_args[:remove_script_attributes]    = opts['remove_script_attributes']   if opts.has_key?('remove_script_attributes')
          html_args[:remove_style_attributes]     = opts['remove_style_attributes']    if opts.has_key?('remove_style_attributes')
          html_args[:remove_link_attributes]      = opts['remove_link_attributes']     if opts.has_key?('remove_link_attributes')
          html_args[:remove_form_attributes]      = opts['remove_form_attributes']     if opts.has_key?('remove_form_attributes')
          html_args[:remove_input_attributes]     = opts['remove_input_attributes']    if opts.has_key?('remove_input_attributes')
          html_args[:remove_javascript_protocol]  = opts['remove_javascript_protocol'] if opts.has_key?('remove_javascript_protocol')
          html_args[:remove_http_protocol]        = opts['remove_http_protocol']       if opts.has_key?('remove_http_protocol')
          html_args[:remove_https_protocol]       = opts['remove_https_protocol']      if opts.has_key?('remove_https_protocol')
          html_args[:preserve_line_breaks]        = opts['preserve_line_breaks']       if opts.has_key?('preserve_line_breaks')
          html_args[:simple_boolean_attributes]   = opts['simple_boolean_attributes']  if opts.has_key?('simple_boolean_attributes')
          html_args[:compress_js_templates]       = opts['compress_js_templates']      if opts.has_key?('compress_js_templates')
          html_args[:preserve_patterns]          += [/<\?php.*?\?>/im]                 if opts['preserve_php'] == true
          html_args[:preserve_patterns]          += opts[:preserve_patterns].map { |pattern| Regexp.new(pattern)} if opts.has_key?(:preserve_patterns)
        end

        html_args[:css_compressor]              = CSSminify2.new()

        if ( !js_args[:uglifier_args].nil? )
          html_args[:javascript_compressor]       = Uglifier.new(js_args[:uglifier_args])
        else
          html_args[:javascript_compressor]       = Uglifier.new()
        end

        compressor = HtmlCompressor::Compressor.new(html_args)
        output_file(path, compressor.compress(content))
      else
        output_file(path, content)
      end
    end

    def output_js(path, content)
      if ( ENV['JEKYLL_ENV'] == "production" )
        js_args  = {}
        opts     = @site.config['jekyll-minifier']
        compress = true
        if ( !opts.nil? )
          compress                = opts['compress_javascript']                           if opts.has_key?('compress_javascript')
          js_args[:uglifier_args] = Hash[opts['uglifier_args'].map{|(k,v)| [k.to_sym,v]}] if opts.has_key?('uglifier_args')
        end

        if ( compress )
          if ( !js_args[:uglifier_args].nil? )
            compressor = Uglifier.new(js_args[:uglifier_args])
          else
            compressor = Uglifier.new()
          end

          output_file(path, compressor.compile(content))
        else
          output_file(path, content)
        end
      else
        output_file(path, content)
      end
    end

    def output_json(path, content)
      if ( ENV['JEKYLL_ENV'] == "production" )
        opts       = @site.config['jekyll-minifier']
        compress = true
        if ( !opts.nil? )
          compress    = opts['compress_json']               if opts.has_key?('compress_json')
        end

        if ( compress )
          output_file(path, JSON.minify(content))
        else
          output_file(path, content)
        end
      else
        output_file(path, content)
      end
    end

    def output_css(path, content)
      if ( ENV['JEKYLL_ENV'] == "production" )
        opts       = @site.config['jekyll-minifier']
        compress = true
        if ( !opts.nil? )
          compress    = opts['compress_css']               if opts.has_key?('compress_css')
        end
        if ( compress )
          compressor = CSSminify2.new()
          output_file(path, compressor.compress(content))
        else
          output_file(path, content)
        end
      else
        output_file(path, content)
      end

    end

    private

    def exclude?(dest, dest_path)
      file_name = dest_path.slice(dest.length+1..dest_path.length)
      exclude.any? { |e| e == file_name || File.fnmatch(e, file_name) }
    end

    def exclude
      @exclude ||= Array(@site.config.dig('jekyll-minifier', 'exclude'))
    end
  end

  class Document
    include Compressor

    def write(dest)
      dest_path = destination(dest)
      if exclude?(dest, dest_path)
        output_file(dest_path, output)
      else
        output_compressed(dest_path, output)
      end
      trigger_hooks(:post_write)
    end
  end

  class Page
    include Compressor

    def write(dest)
      dest_path = destination(dest)
      if exclude?(dest, dest_path)
        output_file(dest_path, output)
      else
        output_compressed(dest_path, output)
      end
      Jekyll::Hooks.trigger hook_owner, :post_write, self
    end
  end

  class StaticFile
    include Compressor

    def copy_file(path, dest_path)
      FileUtils.mkdir_p(File.dirname(dest_path))
      FileUtils.cp(path, dest_path)
    end

    def write(dest)
      dest_path = destination(dest)

      return false if File.exist?(dest_path) and !modified?
      self.class.mtimes[path] = mtime

      if exclude?(dest, dest_path)
        copy_file(path, dest_path)
      else
        case File.extname(dest_path)
          when '.js'
            if dest_path.end_with?('.min.js')
              copy_file(path, dest_path)
            else
              output_js(dest_path, File.read(path))
            end
          when '.json'
            output_json(dest_path, File.read(path))
          when '.css'
            if dest_path.end_with?('.min.css')
              copy_file(path, dest_path)
            else
              output_css(dest_path, File.read(path))
            end
          when '.xml'
            output_html(dest_path, File.read(path))
          else
            copy_file(path, dest_path)
        end
      end
      true
    end
  end
end
