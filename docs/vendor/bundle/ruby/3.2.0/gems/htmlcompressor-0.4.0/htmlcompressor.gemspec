# -*- encoding: utf-8 -*-
require File.expand_path('../lib/htmlcompressor/version', __FILE__)

Gem::Specification.new do |gem|
  gem.authors       = ["Paolo Chiodi"]
  gem.email         = ["chiodi84@gmail.com"]
  gem.description   = %q{Put your html on a diet}
  gem.summary       = %q{htmlcompressor provides a class and a rack middleware to minify html pages}
  gem.homepage      = ""

  gem.add_development_dependency 'yui-compressor', '~> 0.9'
  gem.add_development_dependency 'closure-compiler', '~> 1.1'
  gem.add_development_dependency 'rake', '~> 10.3.2'
  gem.add_development_dependency 'minitest', '~> 5.0'

  gem.files         = `git ls-files`.split($\)
  gem.executables   = gem.files.grep(%r{^bin/}).map{ |f| File.basename(f) }
  gem.test_files    = gem.files.grep(%r{^(test|spec|features)/})
  gem.name          = "htmlcompressor"
  gem.require_paths = ["lib"]
  gem.version       = HtmlCompressor::VERSION
end
