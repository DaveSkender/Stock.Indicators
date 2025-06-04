require "htmlcompressor/exceptions"

module HtmlCompressor
  class Compressor

    JS_COMPRESSOR_YUI = "yui";
    JS_COMPRESSOR_CLOSURE = "closure";

    # Predefined pattern that matches <code>&lt;?php ... ?></code> tags.
    # Could be passed inside a list to {@link #setPreservePatterns(List) setPreservePatterns} method.
    PHP_TAG_PATTERN = /<\?php.*?\?>/im

    # Predefined pattern that matches <code>&lt;% ... %></code> tags.
    # Could be passed inside a list to {@link #setPreservePatterns(List) setPreservePatterns} method.
    SERVER_SCRIPT_TAG_PATTERN = /<%.*?%>/m

    # Predefined pattern that matches <code>&lt;--# ... --></code> tags.
    # Could be passed inside a list to {@link #setPreservePatterns(List) setPreservePatterns} method.
    SERVER_SIDE_INCLUDE_PATTERN = /<!--\s*#.*?-->/m

    # Predefined list of tags that are very likely to be block-level.
    #Could be passed to {@link #setRemoveSurroundingSpaces(String) setRemoveSurroundingSpaces} method.
    BLOCK_TAGS_MIN = "html,head,body,br,p"

    # Predefined list of tags that are block-level by default, excluding <code>&lt;div></code> and <code>&lt;li></code> tags.
    #Table tags are also included.
    #Could be passed to {@link #setRemoveSurroundingSpaces(String) setRemoveSurroundingSpaces} method.
    BLOCK_TAGS_MAX = BLOCK_TAGS_MIN + ",h1,h2,h3,h4,h5,h6,blockquote,center,dl,fieldset,form,frame,frameset,hr,noframes,ol,table,tbody,tr,td,th,tfoot,thead,ul"

    # Could be passed to {@link #setRemoveSurroundingSpaces(String) setRemoveSurroundingSpaces} method
    # to remove all surrounding spaces (not recommended).
    ALL_TAGS = "all"

    # temp replacements for preserved blocks
    TEMP_COND_COMMENT_BLOCK = "%%%~COMPRESS~COND~{0,number,#}~%%%"
    TEMP_PRE_BLOCK = "%%%~COMPRESS~PRE~{0,number,#}~%%%"
    TEMP_TEXT_AREA_BLOCK = "%%%~COMPRESS~TEXTAREA~{0,number,#}~%%%"
    TEMP_SCRIPT_BLOCK = "%%%~COMPRESS~SCRIPT~{0,number,#}~%%%"
    TEMP_STYLE_BLOCK = "%%%~COMPRESS~STYLE~{0,number,#}~%%%"
    TEMP_EVENT_BLOCK = "%%%~COMPRESS~EVENT~{0,number,#}~%%%"
    TEMP_LINE_BREAK_BLOCK = "%%%~COMPRESS~LT~{0,number,#}~%%%"
    TEMP_SKIP_BLOCK = "%%%~COMPRESS~SKIP~{0,number,#}~%%%"
    TEMP_USER_BLOCK = "%%%~COMPRESS~USER{0,number,#}~{1,number,#}~%%%"

    # compiled regex patterns
    EMPTY_PATTERN = Regexp.new("\\s")
    SKIP_PATTERN = Regexp.new("<!--\\s*\\{\\{\\{\\s*-->(.*?)<!--\\s*\\}\\}\\}\\s*-->", Regexp::MULTILINE | Regexp::IGNORECASE)
    COND_COMMENT_PATTERN = Regexp.new("(<!(?:--)?\\[[^\\]]+?\\]>)(.*?)(<!\\[[^\\]]+\\]-->)", Regexp::MULTILINE | Regexp::IGNORECASE)
    COMMENT_PATTERN = Regexp.new("<!---->|<!--[^\\[].*?-->", Regexp::MULTILINE | Regexp::IGNORECASE)
    INTERTAG_PATTERN_TAG_TAG = Regexp.new(">\\s+<", Regexp::MULTILINE | Regexp::IGNORECASE)
    INTERTAG_PATTERN_TAG_CUSTOM = Regexp.new(">\\s+%%%~", Regexp::MULTILINE | Regexp::IGNORECASE)
    INTERTAG_PATTERN_CUSTOM_TAG = Regexp.new("~%%%\\s+<", Regexp::MULTILINE | Regexp::IGNORECASE)
    INTERTAG_PATTERN_CUSTOM_CUSTOM = Regexp.new("~%%%\\s+%%%~", Regexp::MULTILINE | Regexp::IGNORECASE)
    MULTISPACE_PATTERN = Regexp.new("\\s+", Regexp::MULTILINE | Regexp::IGNORECASE)
    TAG_END_SPACE_PATTERN = Regexp.new("(<(?:[^>]+?))(?:\\s+?)(/?>)", Regexp::MULTILINE | Regexp::IGNORECASE)
    TAG_LAST_UNQUOTED_VALUE_PATTERN = Regexp.new("=\\s*[a-z0-9\\-_]+$", Regexp::IGNORECASE)
    TAG_QUOTE_PATTERN = Regexp.new("\\s*=\\s*([\"'])([a-z0-9\\-_]+?)\\1(/?)(?=[^<]*?>)", Regexp::IGNORECASE)
    PRE_PATTERN = Regexp.new("(<pre[^>]*?>)(.*?)(</pre>)", Regexp::MULTILINE | Regexp::IGNORECASE)
    TA_PATTERN = Regexp.new("(<textarea[^>]*?>)(.*?)(</textarea>)", Regexp::MULTILINE | Regexp::IGNORECASE)
    SCRIPT_PATTERN = Regexp.new("(<script[^>]*?>)(.*?)(</script>)", Regexp::MULTILINE | Regexp::IGNORECASE)
    STYLE_PATTERN = Regexp.new("(<style[^>]*?>)(.*?)(</style>)", Regexp::MULTILINE | Regexp::IGNORECASE)
    TAG_PROPERTY_PATTERN = Regexp.new("(\\s\\w+)\\s*=\\s*(?=[^<]*?>)", Regexp::IGNORECASE)
    CDATA_PATTERN = Regexp.new("\\s*<!\\[CDATA\\[(.*?)\\]\\]>\\s*", Regexp::MULTILINE | Regexp::IGNORECASE)
    DOCTYPE_PATTERN = Regexp.new("<!DOCTYPE[^>]*>", Regexp::MULTILINE | Regexp::IGNORECASE)
    TYPE_ATTR_PATTERN = Regexp.new("type\\s*=\\s*([\\\"']*)(.+?)\\1", Regexp::MULTILINE | Regexp::IGNORECASE)
    JS_TYPE_ATTR_PATTERN = Regexp.new("(<script[^>]*)type\\s*=\\s*([\"']*)(?:text|application)\/javascript\\2([^>]*>)", Regexp::MULTILINE | Regexp::IGNORECASE)
    JS_LANG_ATTR_PATTERN = Regexp.new("(<script[^>]*)language\\s*=\\s*([\"']*)javascript\\2([^>]*>)", Regexp::MULTILINE | Regexp::IGNORECASE)
    STYLE_TYPE_ATTR_PATTERN = Regexp.new("(<style[^>]*)type\\s*=\\s*([\"']*)text/(?:style|css)\\2([^>]*>)", Regexp::MULTILINE | Regexp::IGNORECASE)
    LINK_TYPE_ATTR_PATTERN = Regexp.new("(<link[^>]*)type\\s*=\\s*([\"']*)text/(?:css|plain)\\2([^>]*>)", Regexp::MULTILINE | Regexp::IGNORECASE)
    LINK_REL_ATTR_PATTERN = Regexp.new("<link(?:[^>]*)rel\\s*=\\s*([\"']*)(?:alternate\\s+)?stylesheet\\1(?:[^>]*)>", Regexp::MULTILINE | Regexp::IGNORECASE)
    FORM_METHOD_ATTR_PATTERN = Regexp.new("(<form[^>]*)method\\s*=\\s*([\"']*)get\\2([^>]*>)", Regexp::MULTILINE | Regexp::IGNORECASE)
    INPUT_TYPE_ATTR_PATTERN = Regexp.new("(<input[^>]*)type\\s*=\\s*([\"']*)text\\2([^>]*>)", Regexp::MULTILINE | Regexp::IGNORECASE)
    BOOLEAN_ATTR_PATTERN = Regexp.new("(<\\w+[^>]*[\"' ])(checked|selected|disabled|readonly)\\s*=\\s*([\"']*)\\w*\\3([^>]*>)", Regexp::MULTILINE | Regexp::IGNORECASE)
    EVENT_JS_PROTOCOL_PATTERN = Regexp.new("^javascript:\\s*(.+)", Regexp::MULTILINE | Regexp::IGNORECASE)
    HTTP_PROTOCOL_PATTERN = Regexp.new("(<[^>]+?(?:href|src|cite|action)\\s*=\\s*['\"])http:(//[^>]+?>)", Regexp::MULTILINE | Regexp::IGNORECASE)
    HTTPS_PROTOCOL_PATTERN = Regexp.new("(<[^>]+?(?:href|src|cite|action)\\s*=\\s*['\"])https:(//[^>]+?>)", Regexp::MULTILINE | Regexp::IGNORECASE)
    REL_EXTERNAL_PATTERN = Regexp.new("<(?:[^>]*)rel\\s*=\\s*([\"']*)(?:alternate\\s+)?external\\1(?:[^>]*)>", Regexp::MULTILINE | Regexp::IGNORECASE)
    EVENT_PATTERN1 = Regexp.new("(\\son[a-z]+\\s*=\\s*\")([^\"\\\\\\r\\n]*(?:\\\\.[^\"\\\\\\r\\n]*)*)(\")", Regexp::IGNORECASE) # unmasked: \son[a-z]+\s*=\s*"[^"\\\r\n]*(?:\\.[^"\\\r\n]*)*""
    EVENT_PATTERN2 = Regexp.new("(\\son[a-z]+\\s*=\\s*')([^'\\\\\\r\\n]*(?:\\\\.[^'\\\\\\r\\n]*)*)(')", Regexp::IGNORECASE)
    LINE_BREAK_PATTERN = Regexp.new("(?:[[:blank:]]*(\\r?\\n)[[:blank:]]*)+")
    SURROUNDING_SPACES_MIN_PATTERN = Regexp.new("\\s*(</?(?:" + BLOCK_TAGS_MIN.gsub(",", "|") + ")(?:>|[\\s/][^>]*>))\\s*", Regexp::MULTILINE | Regexp::IGNORECASE)
    SURROUNDING_SPACES_MAX_PATTERN = Regexp.new("\\s*(</?(?:" + BLOCK_TAGS_MAX.gsub(",", "|") + ")(?:>|[\\s/][^>]*>))\\s*", Regexp::MULTILINE | Regexp::IGNORECASE)
    SURROUNDING_SPACES_ALL_PATTERN = Regexp.new("\\s*(<[^>]+>)\\s*", Regexp::MULTILINE | Regexp::IGNORECASE)

    # patterns for searching for temporary replacements
    TEMP_COND_COMMENT_PATTERN = Regexp.new("%%%~COMPRESS~COND~(\\d+?)~%%%")
    TEMP_PRE_PATTERN = Regexp.new("%%%~COMPRESS~PRE~(\\d+?)~%%%")
    TEMP_TEXT_AREA_PATTERN = Regexp.new("%%%~COMPRESS~TEXTAREA~(\\d+?)~%%%")
    TEMP_SCRIPT_PATTERN = Regexp.new("%%%~COMPRESS~SCRIPT~(\\d+?)~%%%")
    TEMP_STYLE_PATTERN = Regexp.new("%%%~COMPRESS~STYLE~(\\d+?)~%%%")
    TEMP_EVENT_PATTERN = Regexp.new("%%%~COMPRESS~EVENT~(\\d+?)~%%%")
    TEMP_SKIP_PATTERN = Regexp.new("%%%~COMPRESS~SKIP~(\\d+?)~%%%")
    TEMP_LINE_BREAK_PATTERN = Regexp.new("%%%~COMPRESS~LT~(\\d+?)~%%%")

    JAVASCRIPT_COMPRESSORS_OPTIONS = {
      :closure => { :compilation_level => 'ADVANCED_OPTIMIZATIONS' },
      :yui => { :munge => true, :preserve_semicolons => true, :optimize => true, :line_break => nil }
    }

    CSS_COMPRESSORS_OPTIONS = {
      :yui => { :line_break => -1 }
    }

    DEFAULT_OPTIONS = {
      :enabled => true,

      # default settings
      :remove_comments => true,
      :remove_multi_spaces => true,
      :remove_spaces_inside_tags => true,

      # optional settings
      :javascript_compressor => :yui,
      :css_compressor => :yui,
      :remove_intertag_spaces => false,
      :remove_quotes => false,
      :compress_javascript => false,
      :compress_css => false,
      :simple_doctype => false,
      :remove_script_attributes => false,
      :remove_style_attributes => false,
      :remove_link_attributes => false,
      :remove_form_attributes => false,
      :remove_input_attributes => false,
      :simple_boolean_attributes => false,
      :remove_javascript_protocol => false,
      :remove_http_protocol => false,
      :remove_https_protocol => false,
      :preserve_line_breaks => false,
      :remove_surrounding_spaces => nil,

      :preserve_patterns => nil
    }

    def initialize(options = {})

      @options = DEFAULT_OPTIONS.merge(options)

      if @options[:compress_js_templates]
        @options[:remove_quotes] = false

        js_template_types = [ 'text/x-jquery-tmpl' ]

        unless @options[:compress_js_templates].is_a? TrueClass
          js_template_types << @options[:compress_js_templates]
          js_template_types.flatten!
        end

        @options[:js_template_types] = js_template_types
      else
        @options[:js_template_types] = []
      end

      detect_external_compressors
    end

    def detect_external_compressors
      @javascript_compressors = {}
      @css_compressors = {}

      # Try Closure.
      begin
        require 'closure-compiler'
        @javascript_compressors[:closure] = Closure::Compiler
      rescue LoadError
      end

      # Try YUI
      begin
        require 'yui/compressor'
        @javascript_compressors[:yui] = YUI::JavaScriptCompressor
        @css_compressors[:yui] = YUI::CssCompressor
      rescue LoadError
      end
    end

    def get_javascript_compressor(compressor_name)

      if @javascript_compressors.has_key? compressor_name
        @javascript_compressors[compressor_name].new JAVASCRIPT_COMPRESSORS_OPTIONS[compressor_name]
      end

    end

    def get_css_compressor(compressor_name)

      if @css_compressors.has_key? compressor_name
        @css_compressors[compressor_name].new CSS_COMPRESSORS_OPTIONS[compressor_name]
      end

    end

    def compress html
      if not @options[:enabled] or html.nil? or html.length == 0
        return html
      end

      # preserved block containers
      condCommentBlocks = []
      preBlocks = []
      taBlocks = []
      scriptBlocks = []
      styleBlocks = []
      eventBlocks = []
      skipBlocks = []
      lineBreakBlocks = []
      userBlocks = []

      # preserve blocks
      html = preserve_blocks(html, preBlocks, taBlocks, scriptBlocks, styleBlocks, eventBlocks, condCommentBlocks, skipBlocks, lineBreakBlocks, userBlocks)

      # process pure html
      html = process_html(html)

      # process preserved blocks
      process_preserved_blocks(preBlocks, taBlocks, scriptBlocks, styleBlocks, eventBlocks, condCommentBlocks, skipBlocks, lineBreakBlocks, userBlocks)

      # put preserved blocks back
      html = return_blocks(html, preBlocks, taBlocks, scriptBlocks, styleBlocks, eventBlocks, condCommentBlocks, skipBlocks, lineBreakBlocks, userBlocks)

      html
    end

    private

    def preserve_blocks(html, preBlocks, taBlocks, scriptBlocks, styleBlocks, eventBlocks, condCommentBlocks, skipBlocks, lineBreakBlocks, userBlocks)

      # preserve user blocks
      preservePatterns = @options[:preserve_patterns]
      unless (preservePatterns.nil?)
        preservePatterns.each_with_index do |preservePattern, i|
          userBlock = []
          index = -1

          html = html.gsub(preservePattern) do |match|
            if match.strip.length > 0
              userBlock << match
              index += 1
              message_format(TEMP_USER_BLOCK, i, index)
            else
              ''
            end
          end

          userBlocks << userBlock
        end
      end

      # preserve <!-- {{{ ---><!-- }}} ---> skip blocks
      skipBlockIndex = -1

      html = html.gsub(SKIP_PATTERN) do |match|
        if $1.strip.length > 0
          skipBlocks << match
          skipBlockIndex += 1
          message_format(TEMP_SKIP_BLOCK, skipBlockIndex)
        else
          match
        end
      end

      # preserve conditional comments
      condCommentCompressor = self.clone
      index = -1

      html = html.gsub(COND_COMMENT_PATTERN) do |match|
        if $2.strip.length > 0
          index += 1
          condCommentBlocks << ($1 + condCommentCompressor.compress($2) + $3)
          message_format(TEMP_COND_COMMENT_BLOCK, index)
        else
          ''
        end
      end

      # preserve inline events
      index = -1

      html = html.gsub(EVENT_PATTERN1) do |match|
        if $2.strip.length > 0
          eventBlocks << $2
          index += 1
          $1 + message_format(TEMP_EVENT_BLOCK, index) + $3
        else
          ''
        end
      end

      html = html.gsub(EVENT_PATTERN2) do |match|
        if $2.strip.length > 0
          eventBlocks << $2
          index += 1
          $1 + message_format(TEMP_EVENT_BLOCK, index) + $3
        else
          ''
        end
      end

      # preserve PRE tags
      index = -1
      html = html.gsub PRE_PATTERN do |match|
        if $2.strip.length > 0
          index += 1
          preBlocks << $2
          $1 + message_format(TEMP_PRE_BLOCK, index) + $3
        else
          ''
        end
      end

      # preserve SCRIPT tags
      index = -1

      html = html.gsub(SCRIPT_PATTERN) do |match|
        group_1 = $1
        group_2 = $2
        group_3 = $3
        # ignore empty scripts
        if group_2.strip.length > 0
          # check type
          type = ""
          if group_1 =~ TYPE_ATTR_PATTERN
            type = $2.downcase
          end

          if type.length == 0 or type == 'text/javascript' or type == 'application/javascript'
            # javascript block, preserve and compress with js compressor
            scriptBlocks << group_2
            index += 1
            group_1 + message_format(TEMP_SCRIPT_BLOCK, index) + group_3
          elsif @options[:js_template_types].include?(type)
            # jquery template, ignore so it gets compressed with the rest of html
            match
          else
            # some custom script, preserve it inside "skip blocks" so it won't be compressed with js compressor
            skipBlocks << group_2
            skipBlockIndex += 1
            group_1 + message_format(TEMP_SKIP_BLOCK, skipBlockIndex) + group_3
          end

        else
          match
        end
      end

      # preserve STYLE tags
      index = -1

      html = html.gsub(STYLE_PATTERN) do |match|
        if $2.strip.length > 0
          styleBlocks << $2
          index += 1
          $1 + message_format(TEMP_STYLE_BLOCK, index) + $3
        else
          match
        end
      end

      # preserve TEXTAREA tags
      index = -1
      html = html.gsub(TA_PATTERN) do |match|
        index += 1

        if $2.strip.length > 0
          taBlocks << $2
        else
          taBlocks << ''
        end

        $1 + message_format(TEMP_TEXT_AREA_BLOCK, index) + $3
      end

      # preserve line breaks
      if @options[:preserve_line_breaks]
        index = -1
        html = html.gsub(LINE_BREAK_PATTERN) do |match|
          lineBreakBlocks << $1
          index += 1
          message_format(TEMP_LINE_BREAK_BLOCK, index)
        end
      end

      html
    end

    def return_blocks(html, preBlocks, taBlocks, scriptBlocks, styleBlocks, eventBlocks, condCommentBlocks, skipBlocks, lineBreakBlocks, userBlocks)

      # put skip blocks back
      html = html.gsub(TEMP_SKIP_PATTERN) do |match|
        i = $1.to_i
        if skipBlocks.size > i
          skipBlocks[i]
        else
          ''
        end
      end

      # put line breaks back
      if @options[:preserve_line_breaks]
        html = html.gsub(TEMP_LINE_BREAK_PATTERN) do |match|
          i = $1.to_i
          if lineBreakBlocks.size > i
            lineBreakBlocks[i]
          else
            ''
          end
        end
      end

      # put TEXTAREA blocks back
      html = html.gsub(TEMP_TEXT_AREA_PATTERN) do |match|
        i = $1.to_i
        if taBlocks.size > i
          taBlocks[i]
        else
          ''
        end
      end

      # put STYLE blocks back
      html = html.gsub(TEMP_STYLE_PATTERN) do |match|
        i = $1.to_i
        if styleBlocks.size > i
          styleBlocks[i]
        else
          ''
        end
      end

      # put SCRIPT blocks back
      html = html.gsub(TEMP_SCRIPT_PATTERN) do |match|
        i = $1.to_i
        if scriptBlocks.size > i
          scriptBlocks[i]
        end
      end

      # put PRE blocks back
      html = html.gsub TEMP_PRE_PATTERN do |match|
        i = $1.to_i
        if preBlocks.size > i
          preBlocks[i] # quoteReplacement ?
        else
          ''
        end
      end

      # put event blocks back
      html = html.gsub(TEMP_EVENT_PATTERN) do |match|
        i = $1.to_i
        if eventBlocks.size > i
          eventBlocks[i]
        else
          ''
        end
      end

      # put conditional comments back
      html = html.gsub(TEMP_COND_COMMENT_PATTERN) do |match|
        i = $1.to_i
        if condCommentBlocks.size > i
          condCommentBlocks[i] # quoteReplacement ?
        else
          ''
        end
      end


      # put user blocks back
      unless @options[:preserve_patterns].nil?
        @options[:preserve_patterns].each_with_index do |preservePattern, p|
          tempUserPattern = Regexp.new("%%%~COMPRESS~USER#{p}~(\\d+?)~%%%")
          html = html.gsub(tempUserPattern).each do |match|
            i = $1.to_i
            if userBlocks.size > p and userBlocks[p].size > i
              userBlocks[p][i]
            else
              ''
            end
          end
        end
      end

      html
    end

    def process_preserved_blocks(preBlocks, taBlocks, scriptBlocks, styleBlocks, eventBlocks, condCommentBlocks, skipBlocks, lineBreakBlocks, userBlocks)
      # processPreBlocks(preBlocks)
      # processTextAreaBlocks(taBlocks)
      process_script_blocks(scriptBlocks)
      process_style_blocks(styleBlocks)
      process_event_blocks(eventBlocks)
      # processCondCommentBlocks(condCommentBlocks)
      # processSkipBlocks(skipBlocks)
      # processUserBlocks(userBlocks)
      # processLineBreakBlocks(lineBreakBlocks)
    end

    def process_script_blocks(scriptBlocks)
      if @options[:compress_javascript]
        scriptBlocks.map! do |block|
          compress_javascript(block)
        end
      end
    end

    def process_style_blocks(styleBlocks)
      if @options[:compress_css]
        styleBlocks.map! do |block|
          compress_css_styles(block)
        end
      end
    end

    def process_event_blocks(eventBlocks)
      if @options[:remove_javascript_protocol]
        eventBlocks.map! do |block|
          remove_javascript_protocol(block)
        end
      end
    end

    def compress_javascript(source)
      # set default javascript compressor
      javascript_compressor = @options[:javascript_compressor]

      if javascript_compressor.is_a?(Symbol)
        javascript_compressor = get_javascript_compressor(javascript_compressor)
      end

      if javascript_compressor.nil?
        if @options[:javascript_compressor].is_a?(Symbol)
          raise NotFoundCompressorError, "JavaScript Compressor \"#{@options[:javascript_compressor]}\" not found, please check :javascript_compressor option"
        else
          raise MissingCompressorError, "No JavaScript Compressor. Please set the :javascript_compressor option"
        end
      end

      # detect CDATA wrapper
      cdataWrapper = false
      if source =~ CDATA_PATTERN
        cdataWrapper = true
        source = $1
      end

      result = javascript_compressor.compress(source).strip

      if cdataWrapper
        result = "/*<![CDATA[*/" + result + "/*]]>*/"
      end

      result
    end

    def compress_css_styles(source)
      # set default css compressor
      css_compressor = @options[:css_compressor]

      if css_compressor.is_a?(Symbol)
        css_compressor = get_css_compressor(css_compressor)
      end

      if css_compressor.nil?
        if @options[:css_compressor].is_a?(Symbol)
          raise NotFoundCompressorError, "CSS Compressor \"#{@options[:css_compressor]}\" not found, please check :css_compressor option"
        else
          raise MissingCompressorError, "No CSS Compressor. Please set the :css_compressor option"
        end
      end

      # detect CDATA wrapper
      cdataWrapper = false
      if source =~ CDATA_PATTERN
        cdataWrapper = true
        source = $1
      end

      result = css_compressor.compress(source)

      if cdataWrapper
        result = "<![CDATA[" + result + "]]>"
      end

      result
    end

    def remove_javascript_protocol(source)
      # remove javascript: from inline events
      source.sub(EVENT_JS_PROTOCOL_PATTERN, '\1')
    end

    def process_html(html)

      # remove comments
      html = remove_comments(html)

      # simplify doctype
      html = simple_doctype(html)

      # remove script attributes
      html = remove_script_attributes(html)

      # remove style attributes
      html = remove_style_attributes(html)

      # remove link attributes
      html = remove_link_attributes(html)

      # remove form attributes
      html = remove_form_attributes(html)

      # remove input attributes
      html = remove_input_attributes(html)

      # simplify boolean attributes
      html = simple_boolean_attributes(html)

      # remove http from attributes
      html = remove_http_protocol(html)

      # remove https from attributes
      html = remove_https_protocol(html)

      # remove inter-tag spaces
      html = remove_intertag_spaces(html)

      # remove multi whitespace characters
      html = remove_multi_spaces(html)

      # remove spaces around equals sign and ending spaces
      html = remove_spaces_inside_tags(html)

      # remove quotes from tag attributes
      html = remove_quotes_inside_tags(html)

      # # remove surrounding spaces
      html = remove_surrounding_spaces(html)

      html.strip
    end

    def remove_comments(html)

      # remove comments
      if @options[:remove_comments]
        html = html.gsub(COMMENT_PATTERN, '')
      end

      html
    end

    def simple_doctype(html)
      # simplify doctype
      if @options[:simple_doctype]
        html = html.gsub(DOCTYPE_PATTERN, '<!DOCTYPE html>')
      end

      html
    end

    def remove_script_attributes(html)
      if @options[:remove_script_attributes]
        #remove type from script tags
        html = html.gsub(JS_TYPE_ATTR_PATTERN, '\1\3')

        #remove language from script tags
        html = html.gsub(JS_LANG_ATTR_PATTERN, '\1\3')
      end

      html
    end

    def remove_style_attributes(html)
      # remove type from style tags
      if @options[:remove_style_attributes]
        html = html.gsub(STYLE_TYPE_ATTR_PATTERN, '\1\3')
      end

      html
    end

    def remove_link_attributes(html)
      # remove type from link tags with rel=stylesheet
      if @options[:remove_link_attributes]
        html = html.gsub(LINK_TYPE_ATTR_PATTERN) do |match|
          group_1 = $1
          group_3 = $3
          # if rel=stylesheet
          if match =~ LINK_REL_ATTR_PATTERN
            group_1 + group_3
          else
            match
          end
        end
      end

      html
    end

    def remove_form_attributes(html)
      # remove method from form tags
      if @options[:remove_form_attributes]
        html = html.gsub(FORM_METHOD_ATTR_PATTERN, '\1\3')
      end

      html
    end

    def remove_input_attributes(html)
      # remove type from input tags
      if @options[:remove_input_attributes]
        html = html.gsub(INPUT_TYPE_ATTR_PATTERN, '\1\3')
      end

      html
    end

    def remove_http_protocol(html)
      # remove http protocol from tag attributes
      if @options[:remove_http_protocol]
        html = html.gsub(HTTP_PROTOCOL_PATTERN) do |match|
          group_1 = $1
          group_2 = $2

          if match =~ REL_EXTERNAL_PATTERN
            match
          else
            "#{group_1}#{group_2}"
          end
        end
      end

      html
    end

    def remove_https_protocol(html)
      # remove https protocol from tag attributes
      if @options[:remove_https_protocol]
        html = html.gsub(HTTPS_PROTOCOL_PATTERN) do |match|
          group_1 = $1
          group_2 = $2

          if match =~ REL_EXTERNAL_PATTERN
            match
          else
            "#{group_1}#{group_2}"
          end
        end
      end

      html
    end

    def remove_intertag_spaces(html)

      # remove inter-tag spaces
      if @options[:remove_intertag_spaces]
        html = html.gsub(INTERTAG_PATTERN_TAG_TAG, '><')
        html = html.gsub(INTERTAG_PATTERN_TAG_CUSTOM, '>%%%~')
        html = html.gsub(INTERTAG_PATTERN_CUSTOM_TAG, '~%%%<')
        html = html.gsub(INTERTAG_PATTERN_CUSTOM_CUSTOM, '~%%%%%%~')
      end

      html
    end

    def remove_spaces_inside_tags(html)
      #remove spaces around equals sign inside tags

      if @options[:remove_spaces_inside_tags]
        html = html.gsub(TAG_PROPERTY_PATTERN, '\1=')

        #remove ending spaces inside tags

        html.gsub!(TAG_END_SPACE_PATTERN) do |match|

          group_1 = $1
          group_2 = $2

          # keep space if attribute value is unquoted before trailing slash
          if group_2.start_with?("/") and (TAG_LAST_UNQUOTED_VALUE_PATTERN =~ group_1)
            "#{group_1} #{group_2}"
          else
            "#{group_1}#{group_2}"
          end
        end
      end

      html
    end

    def remove_quotes_inside_tags(html)
      if @options[:remove_quotes]
        html = html.gsub(TAG_QUOTE_PATTERN) do |match|
          # if quoted attribute is followed by "/" add extra space
          if $3.strip.length == 0
            "=#{$2}"
          else
            "=#{$2} #{$3}"
          end
        end
      end

      html
    end

    def remove_multi_spaces(html)
      # collapse multiple spaces
      if @options[:remove_multi_spaces]
        html = html.gsub(MULTISPACE_PATTERN, ' ')
      end

      html
    end

    def simple_boolean_attributes(html)
      # simplify boolean attributes
      if @options[:simple_boolean_attributes]
        html = html.gsub(BOOLEAN_ATTR_PATTERN, '\1\2\4')
      end

      html
    end

    def remove_surrounding_spaces(html)
      # remove spaces around provided tags

      unless @options[:remove_surrounding_spaces].nil?
        pattern = case @options[:remove_surrounding_spaces].downcase
        when BLOCK_TAGS_MIN
          SURROUNDING_SPACES_MIN_PATTERN
        when BLOCK_TAGS_MAX
          SURROUNDING_SPACES_MAX_PATTERN
        when ALL_TAGS
          SURROUNDING_SPACES_ALL_PATTERN
        else
          Regexp.new("\\s*(</?(?:" + @options[:remove_surrounding_spaces].gsub(",", "|") + ")(?:>|[\\s/][^>]*>))\\s*", Regexp::MULTILINE | Regexp::IGNORECASE)
        end

        html = html.gsub(pattern, '\1')
      end

      html
    end

    def message_format(message, *params)
      message.gsub(/\{(\d+),number,#\}/) do
        params[$1.to_i]
      end
    end

  end

end
