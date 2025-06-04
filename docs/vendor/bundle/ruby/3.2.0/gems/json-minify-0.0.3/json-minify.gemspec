# coding: utf-8
lib = File.expand_path('../lib', __FILE__)
$LOAD_PATH.unshift(lib) unless $LOAD_PATH.include?(lib)
require 'json/minify/version'

Gem::Specification.new do |spec|
  spec.name          = 'json-minify'
  spec.version       = JSON::Minify::VERSION
  spec.authors       = ["Geoff Youngs\n\n\n"]
  spec.email         = ['git@intersect-uk.co.uk']
  spec.summary       = 'JSON.minify implementation'
  spec.description   = 'Pre-parser for JSON that removes C/C++ style comments and whitespace from formatted JSON, similar to https://github.com/getify/JSON.minify.'
  spec.homepage      = 'http://github.com/geoffyoungs/json-minify-rb'
  spec.license       = 'MIT'

  spec.files         = `git ls-files -z`.split("\x0")
  spec.executables   = spec.files.grep(%r{^bin/}) { |f| File.basename(f) }
  spec.test_files    = spec.files.grep(%r{^(test|spec|features)/})
  spec.require_paths = ['lib']

  spec.add_development_dependency 'bundler', '~> 1.5'
  spec.add_development_dependency 'rake', '~> 10.1'
  spec.add_development_dependency 'rspec', '~> 3.2'
  spec.add_dependency 'json', '> 0'
end
