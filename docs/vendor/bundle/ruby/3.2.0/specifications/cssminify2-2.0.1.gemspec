# -*- encoding: utf-8 -*-
# stub: cssminify2 2.0.1 ruby lib

Gem::Specification.new do |s|
  s.name = "cssminify2".freeze
  s.version = "2.0.1"

  s.required_rubygems_version = Gem::Requirement.new(">= 0".freeze) if s.respond_to? :required_rubygems_version=
  s.require_paths = ["lib".freeze]
  s.authors = ["Matt Spurrier".freeze]
  s.date = "2017-01-19"
  s.description = "    The CSSminify gem provides CSS compression using YUI compressor. Instead of wrapping around the Java or Javascript version of YUI compressor it uses a native Ruby port of the CSS engine. Therefore this gem has no dependencies.\n    This package is a fork of the original (now unmaintained) version.\n".freeze
  s.email = "matthew@spurrier.com.au".freeze
  s.extra_rdoc_files = ["CHANGES.md".freeze, "LICENSE.md".freeze, "README.md".freeze]
  s.files = ["CHANGES.md".freeze, "LICENSE.md".freeze, "README.md".freeze]
  s.homepage = "https://github.com/digitalsparky/cssminify".freeze
  s.licenses = ["MIT".freeze]
  s.rubygems_version = "3.4.20".freeze
  s.summary = "CSS minification with YUI compressor, but as native Ruby port.".freeze

  s.installed_by_version = "3.4.20" if s.respond_to? :installed_by_version

  s.specification_version = 4

  s.add_development_dependency(%q<rspec>.freeze, ["~> 2.7"])
end
