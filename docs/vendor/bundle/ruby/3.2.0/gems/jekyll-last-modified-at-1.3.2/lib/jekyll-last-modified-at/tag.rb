# frozen_string_literal: true

module Jekyll
  module LastModifiedAt
    class Tag < Liquid::Tag
      def initialize(tag_name, format, tokens)
        super
        @format = format.empty? ? nil : format.strip
      end

      def render(context)
        site = context.registers[:site]
        format = @format || site.config.dig("last-modified-at", "date-format")
        article_file = context.environments.first["page"]["path"]
        Determinator.new(site.source, article_file, format)
          .formatted_last_modified_date
      end
    end
  end
end

Liquid::Template.register_tag("last_modified_at", Jekyll::LastModifiedAt::Tag)
