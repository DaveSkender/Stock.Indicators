# -*- encoding: utf-8 -*-
# stub: github-pages-health-check 1.18.2 ruby lib

Gem::Specification.new do |s|
  s.name = "github-pages-health-check".freeze
  s.version = "1.18.2"

  s.required_rubygems_version = Gem::Requirement.new(">= 0".freeze) if s.respond_to? :required_rubygems_version=
  s.require_paths = ["lib".freeze]
  s.authors = ["GitHub, Inc.".freeze]
  s.date = "2023-10-19"
  s.description = "Checks your GitHub Pages site for commons DNS configuration issues.".freeze
  s.email = "support@github.com".freeze
  s.homepage = "https://github.com/github/github-pages-health-check".freeze
  s.licenses = ["MIT".freeze]
  s.required_ruby_version = Gem::Requirement.new(">= 2.2.0".freeze)
  s.rubygems_version = "3.4.20".freeze
  s.summary = "Checks your GitHub Pages site for commons DNS configuration issues".freeze

  s.installed_by_version = "3.4.20" if s.respond_to? :installed_by_version

  s.specification_version = 4

  s.add_runtime_dependency(%q<addressable>.freeze, ["~> 2.3"])
  s.add_runtime_dependency(%q<dnsruby>.freeze, ["~> 1.60"])
  s.add_runtime_dependency(%q<octokit>.freeze, [">= 4", "< 8"])
  s.add_runtime_dependency(%q<public_suffix>.freeze, [">= 3.0", "< 6.0"])
  s.add_runtime_dependency(%q<typhoeus>.freeze, ["~> 1.3"])
end
