# Compatibility patch for Liquid 4.0.3 with Ruby >= 3.2
# The `tainted?` method was removed from Ruby in version 3.2,
# but liquid 4.0.3 (pinned by github-pages) still calls it.
# This no-op restores the method so Jekyll can build normally.
# See: https://github.com/jekyll/jekyll/issues/9451

return unless RUBY_VERSION >= '3.2'

class Object
  def tainted?
    false
  end
end
