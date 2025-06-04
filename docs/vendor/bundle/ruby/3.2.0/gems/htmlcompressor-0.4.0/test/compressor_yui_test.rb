require 'test_helper'

module HtmlCompressor

  class TestCompressor < Minitest::Test

    def test_compress_javascript_yui
      source = read_resource("testCompressJavaScript.html");
      result = read_resource("testCompressJavaScriptYuiResult.html");

      compressor = Compressor.new(
        :compress_javascript => true,
        :javascript_compressor => :yui,
        :remove_intertag_spaces => true,
        :compress_js_templates => true
      )

      assert_equal result, compressor.compress(source)
    end

    def test_compress_css
      source = read_resource("testCompressCss.html")
      result = read_resource("testCompressCssResult.html")

      compressor = Compressor.new(
        :compress_css => true,
        :css_compressor => :yui,
        :remove_intertag_spaces => true
      )

      assert_equal result, compressor.compress(source)
    end

  end

end
