# -*- encoding: utf-8 -*-
# stub: jekyll-default-layout 0.1.5 ruby lib

Gem::Specification.new do |s|
  s.name = "jekyll-default-layout".freeze
  s.version = "0.1.5"

  s.required_rubygems_version = Gem::Requirement.new(">= 0".freeze) if s.respond_to? :required_rubygems_version=
  s.require_paths = ["lib".freeze]
  s.authors = ["Ben Balter".freeze]
  s.date = "2020-10-06"
  s.email = ["ben.balter@github.com".freeze]
  s.homepage = "https://github.com/benbalter/jekyll-default-layout".freeze
  s.licenses = ["MIT".freeze]
  s.rubygems_version = "3.4.20".freeze
  s.summary = "Silently sets default layouts for Jekyll pages and posts".freeze

  s.installed_by_version = "3.4.20" if s.respond_to? :installed_by_version

  s.specification_version = 4

  s.add_runtime_dependency(%q<jekyll>.freeze, [">= 3.0", "< 5.0"])
  s.add_development_dependency(%q<rspec>.freeze, ["~> 3.5"])
  s.add_development_dependency(%q<rubocop>.freeze, ["~> 0.8"])
  s.add_development_dependency(%q<rubocop-jekyll>.freeze, ["~> 0.11"])
  s.add_development_dependency(%q<rubocop-performance>.freeze, ["~> 1.6"])
end
