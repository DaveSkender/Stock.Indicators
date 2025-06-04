# coding: utf-8

require "cssminify2/cssmin"
require "cssminify2/version"

class CSSminify2

  def initialize
  end


  #
  # Compress CSS with YUI
  #
  # @param [String, #read] CSS String or IO-like object that supports #read
  # @param [Integer] length Maximum line length
  # @return [String] Compressed CSS
  def self.compress(source, length = 5000)
    self.new.compress(source, length)
  end


  #
  # Compress CSS with YUI
  #
  # @param [String, #read] CSS String or IO-like object that supports #read
  # @param [Integer] length Maximum line length
  # @return [String] Compressed CSS
  def compress(source = '', length = 5000)
    source = source.respond_to?(:read) ? source.read : source.to_s

    CssCompressor.compress(source, length)
  end
end
