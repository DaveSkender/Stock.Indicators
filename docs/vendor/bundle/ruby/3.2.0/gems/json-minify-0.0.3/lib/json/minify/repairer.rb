require 'strscan'
require 'json'
require 'json/minify'
require 'json/minify/version'

module JSON
  module Minify
    #
    class Repairer
      TOKENS = {
        /\s+/ => [:whitespace, :skip],
        /[{},:\]\[]/ => [:punctuation, :keep],
        /""|(".*?[^\\])?"/ => [:string, :keep],
        /(true|false|null)/ => [:reserved, :keep],
        /-?\d+([.]\d+)?([eE][-+]?[0-9]+)?/ => [:number, :keep],
        %r{//.*?(\r?\n|$)} => [:comment, :skip],
        %r{/[*].*?[*]/} => [:comment, :skip]
      }.freeze

      def self.combine(tokens, type)
        regexes = tokens.map do |key, bits|
          key if bits[1] == type
        end.compact
        Regexp.new(regexes.join('|'))
      end

      KEEP = combine(TOKENS, :keep)
      SKIP = combine(TOKENS, :skip)
      ALL  = Regexp.new(TOKENS.keys.join('|'))

      def initialize(str)
        @str = str
      end

      def tokens
        scanner, buf = StringScanner.new(@str), []

        until scanner.eos?
          token = scanner.scan(KEEP)
          buf << token if token
          skip = scanner.skip(SKIP)

          # Anything else is invalid JSON
          next if skip || token || scanner.eos?

          cur = scanner.pos
          extra = scanner.scan_until(ALL)
          if extra
            invalid_string = scanner.pre_match[cur..-1]
            next_token = scanner.matched
            buf << next_token if KEEP.match(next_token)
          else
            invalid_string = scanner.rest
          end

          alternate_token = transform(invalid_string)
          if alternate_token
            buf << alternate_token
          else
            raise SyntaxError, "Unable to pre-scan string: #{invalid_string}"
          end
        end

        buf
      end

      def parse
        JSON.parse(tokens.join)
      end

      def transform(token)
        case token
        when 'NaN'
          '0'
        end
      end

      def minify_parse(buf)
        JSON.parse(minify(buf))
      end
    end
  end
end
