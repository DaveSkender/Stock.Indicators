$LOAD_PATH << File.dirname(__FILE__) + '../lib'

require 'json/minify/repairer'

describe JSON::Minify::Repairer do
  subject { described_class.new(sample) }

  context 'with a NaN' do
    let(:sample) { '{ "hello": NaN }' }
    let(:tokens) { ['{', '"hello"', ':', '0', '}'] }

    it 'should remove whitespace from JSON' do
      expect(subject.tokens).to eql(tokens)
    end
  end

  context 'when parsing data' do
    let(:sample) { '{ "hello": NaN }' }

    it 'should remove whitespace from JSON' do
      expect(subject.parse).to include('hello' => 0)
    end
  end
end
