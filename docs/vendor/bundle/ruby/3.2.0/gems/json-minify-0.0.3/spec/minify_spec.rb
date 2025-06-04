$LOAD_PATH << File.dirname(__FILE__) + '../lib'

require 'json/minify'

describe JSON::Minify do
  it 'should remove whitespace from JSON' do
    expect(JSON.minify('{ }')).to eql('{}')
    expect(JSON.minify(%({"foo": "bar"\n}\n))).to eql(%({"foo":"bar"}))
  end

  it 'should remove comments from JSON' do
    expect(JSON.minify('{ /* foo */ } /* bar */')).to eql('{}')
    expect(JSON.minify(%({"foo": "bar"\n}\n // this is a comment))).to eql(%({"foo":"bar"}))
  end

  it 'should throw a SyntaxError on invalid JSON' do
    expect { JSON.minify('{ /* foo */ } /* bar ') }.to raise_error(SyntaxError)
    expect { JSON.minify(%<{ "foo": new Date(1023) }>) }.to raise_error(SyntaxError)
  end

  it 'should cope with the example from https://github.com/getify/JSON.minify' do
    expect(JSON.minify(%({ /* comment */ "foo": 42 \n }))).to eql('{"foo":42}')
  end

  it 'should cope with the https://gist.github.com/mattheworiordan/72db0dfc933f743622eb' do
    expect(JSON.minify('{ "PolicyName": { "Fn::Join" : [ "", [ "AblySnsPublish-", { "Ref" : "AWS::Region" }, "-", { "Ref" : "DataCenterID" } ] ] } }')).to \
      eql('{"PolicyName":{"Fn::Join":["",["AblySnsPublish-",{"Ref":"AWS::Region"},"-",{"Ref":"DataCenterID"}]]}}')
  end

  it 'should cope with escaped double quoted strings' do
    expect(JSON.minify('{ "Statement1": "he said \"no way\" to the dog", "Statement2": "she said \"really?\"" }')).to \
      eql('{"Statement1":"he said \"no way\" to the dog","Statement2":"she said \"really?\""}')
  end
end
