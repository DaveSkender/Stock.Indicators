# -*- encoding: utf-8 -*-
$:.push File.expand_path("../lib", __FILE__)
require "cssminify2/version"

Gem::Specification.new do |s|
  s.name        = "cssminify2"
  s.version     = CSSminify2::VERSION
  s.author      = "Matt Spurrier"
  s.email       = "matthew@spurrier.com.au"
  s.homepage    = "https://github.com/digitalsparky/cssminify"
  s.summary     = "CSS minification with YUI compressor, but as native Ruby port."
  s.description = <<-EOF
    The CSSminify gem provides CSS compression using YUI compressor. Instead of wrapping around the Java or Javascript version of YUI compressor it uses a native Ruby port of the CSS engine. Therefore this gem has no dependencies.
    This package is a fork of the original (now unmaintained) version.
  EOF

  s.extra_rdoc_files = [
    "CHANGES.md",
    "LICENSE.md",
    "README.md"
  ]

  s.license = "MIT"
  s.rubyforge_project = "cssminify2"

  s.files         = `git ls-files`.split("\n")
  s.test_files    = `git ls-files -- spec/*`.split("\n")
  s.require_paths = ["lib"]

  s.add_development_dependency "rspec", "~> 2.7"
end
