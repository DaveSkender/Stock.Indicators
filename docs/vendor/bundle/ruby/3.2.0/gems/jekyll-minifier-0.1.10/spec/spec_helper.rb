require 'jekyll'
require 'rss'
require 'rspec'
require File.expand_path('../lib/jekyll-minifier', File.dirname(__FILE__))

Jekyll.logger.log_level = :error

RSpec.configure do |config|

  SOURCE_DIR = File.expand_path("../fixtures", __FILE__)
  DEST_DIR   = File.expand_path("../dest",     __FILE__)

  def source_dir(*files)
    File.join(SOURCE_DIR, *files)
  end

  def dest_dir(*files)
    File.join(DEST_DIR, *files)
  end

  def make_context(registers = {})
    Liquid::Context.new({}, {}, { site: site }.merge(registers))
  end
end
