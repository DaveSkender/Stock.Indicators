# frozen_string_literal: true

module Jekyll
  module LastModifiedAt
    require "jekyll-last-modified-at/tag"
    require "jekyll-last-modified-at/hook"

    autoload :VERSION, "jekyll-last-modified-at/version"
    autoload :Executor, "jekyll-last-modified-at/executor"
    autoload :Determinator, "jekyll-last-modified-at/determinator"
    autoload :Git, "jekyll-last-modified-at/git"

    PATH_CACHE = {}
    REPO_CACHE = {}
  end
end
