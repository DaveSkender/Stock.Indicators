# frozen_string_literal: true

require "open3"

module Jekyll
  module LastModifiedAt
    module Executor
      class << self
        def sh(*args)
          stdout_str, stderr_str, status = Open3.capture3(*args)
          "#{stdout_str} #{stderr_str}".strip if status.success?
        end
      end
    end
  end
end
