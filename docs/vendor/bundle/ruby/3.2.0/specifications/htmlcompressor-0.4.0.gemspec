# -*- encoding: utf-8 -*-
# stub: htmlcompressor 0.4.0 ruby lib

Gem::Specification.new do |s|
  s.name = "htmlcompressor".freeze
  s.version = "0.4.0"

  s.required_rubygems_version = Gem::Requirement.new(">= 0".freeze) if s.respond_to? :required_rubygems_version=
  s.require_paths = ["lib".freeze]
  s.authors = ["Paolo Chiodi".freeze]
  s.date = "2017-12-28"
  s.description = "Put your html on a diet".freeze
  s.email = ["chiodi84@gmail.com".freeze]
  s.homepage = "".freeze
  s.rubygems_version = "3.4.20".freeze
  s.summary = "htmlcompressor provides a class and a rack middleware to minify html pages".freeze

  s.installed_by_version = "3.4.20" if s.respond_to? :installed_by_version

  s.specification_version = 4

  s.add_development_dependency(%q<yui-compressor>.freeze, ["~> 0.9"])
  s.add_development_dependency(%q<closure-compiler>.freeze, ["~> 1.1"])
  s.add_development_dependency(%q<rake>.freeze, ["~> 10.3.2"])
  s.add_development_dependency(%q<minitest>.freeze, ["~> 5.0"])
end
