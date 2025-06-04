# -*- encoding: utf-8 -*-
# stub: jekyll-last-modified-at 1.3.2 ruby lib

Gem::Specification.new do |s|
  s.name = "jekyll-last-modified-at".freeze
  s.version = "1.3.2"

  s.required_rubygems_version = Gem::Requirement.new(">= 0".freeze) if s.respond_to? :required_rubygems_version=
  s.require_paths = ["lib".freeze]
  s.authors = ["Garen J. Torikian".freeze]
  s.date = "2024-06-04"
  s.homepage = "https://github.com/gjtorikian/jekyll-last-modified-at".freeze
  s.licenses = ["MIT".freeze]
  s.rubygems_version = "3.4.20".freeze
  s.summary = "A liquid tag for Jekyll to indicate the last time a file was modified.".freeze

  s.installed_by_version = "3.4.20" if s.respond_to? :installed_by_version

  s.specification_version = 4

  s.add_runtime_dependency(%q<jekyll>.freeze, [">= 3.7", "< 5.0"])
  s.add_development_dependency(%q<rake>.freeze, [">= 0"])
  s.add_development_dependency(%q<rspec>.freeze, ["~> 3.4"])
  s.add_development_dependency(%q<rubocop>.freeze, [">= 0"])
  s.add_development_dependency(%q<rubocop-standard>.freeze, [">= 0"])
  s.add_development_dependency(%q<spork>.freeze, [">= 0"])
end
