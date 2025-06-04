# JSON::Minify

JSON.minify implementation - rationale is here: https://github.com/getify/JSON.minify

> Though comments are not officially part of the JSON standard, this post from
> Douglas Crockford back in late 2005 helps explain the motivation behind this project.
>
> http://tech.groups.yahoo.com/group/json/message/152
>
> "A JSON encoder MUST NOT output comments. A JSON decoder MAY accept and ignore comments."

## Credits

 - Matthew O'Riordan github.com/mattheworiordan - fix for handling empty strings
   (in v0.0.2)

## Installation

Add this line to your application's Gemfile:

    gem 'json-minify'

And then execute:

    $ bundle

Or install it yourself as:

    $ gem install json-minify

## Usage

API is compatible with JSON.minify in the npm node-json-minify package.

    JSON.minify("{ }") #=> "{}"

    JSON.minify("{ /* comment */ }") #=> "{}"


## Contributing

1. Fork it ( http://github.com/geoffyoungs/json-minify-rb/fork )
2. Create your feature branch (`git checkout -b my-new-feature`)
3. Commit your changes (`git commit -am 'Add some feature'`)
4. Push to the branch (`git push origin my-new-feature`)
5. Create new Pull Request
