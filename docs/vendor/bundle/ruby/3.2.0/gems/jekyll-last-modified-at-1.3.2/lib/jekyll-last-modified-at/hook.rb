# frozen_string_literal: true

module Jekyll
  module LastModifiedAt
    module Hook
      class << self
        def add_determinator_proc
          proc { |item|
            format = item.site.config.dig("last-modified-at", "date-format")
            item.data["last_modified_at"] = Determinator.new(
              item.site.source,
              item.path,
              format,
            )
          }
        end
      end

      Jekyll::Hooks.register(:posts, :post_init, &Hook.add_determinator_proc)
      Jekyll::Hooks.register(:pages, :post_init, &Hook.add_determinator_proc)
      Jekyll::Hooks.register(:documents, :post_init, &Hook.add_determinator_proc)
    end
  end
end
