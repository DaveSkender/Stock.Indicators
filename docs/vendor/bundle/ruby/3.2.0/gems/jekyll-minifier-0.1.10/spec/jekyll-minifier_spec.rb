require 'spec_helper'

describe "JekyllMinifier" do
  let(:overrides) { Hash.new }
  let(:config) do
    Jekyll.configuration(Jekyll::Utils.deep_merge_hashes({
      "full_rebuild" => true,
      "source"      => source_dir,
      "destination" => dest_dir,
      "show_drafts" => true,
      "url"         => "http://example.org",
      "name"       => "My awesome site",
      "author"      => {
        "name"        => "Dr. Jekyll"
      },
      "collections" => {
        "my_collection" => { "output" => true },
        "other_things"  => { "output" => false }
      }
    }, overrides))
  end
  let(:site)     { Jekyll::Site.new(config) }
  let(:context)  { make_context(site: site) }
  before(:each) do
    allow(ENV).to receive(:[]).and_call_original
    allow(ENV).to receive(:[]).with('JEKYLL_ENV').and_return('production')
    site.process
  end

  context "test_atom" do
    it "creates a atom.xml file" do
      expect(Pathname.new(dest_dir("atom.xml"))).to exist
    end

    let(:atom) { File.read(dest_dir("atom.xml")) }

    it "puts all the posts in the atom.xml file" do
      expect(atom).to match "http://example.org/random/random.html"
      expect(atom).to match "http://example.org/reviews/test-review-1.html"
      expect(atom).to match "http://example.org/reviews/test-review-2.html"
    end

    let(:feed) { RSS::Parser.parse(atom) }

    it "outputs an RSS feed" do
      expect(feed.feed_type).to eql("atom")
      expect(feed.feed_version).to eql("1.0")
      expect(feed.encoding).to eql("UTF-8")
    end

    it "outputs the link" do
      expect(feed.link.href).to eql("http://example.org/atom.xml")
    end
  end

  context "test_css" do
    it "creates a assets/css/style.css file" do
      expect(Pathname.new(dest_dir("assets/css/style.css"))).to exist
    end

    let(:file) { File.read(dest_dir("assets/css/style.css")) }

    it "ensures assets/css/style.css file has length" do
      expect(file.length).to be > 0
    end
  end

  context "test_404" do
    it "creates a 404.html file" do
      expect(Pathname.new(dest_dir("404.html"))).to exist
    end

    let(:file) { File.read(dest_dir("404.html")) }

    it "ensures 404.html file has length" do
      expect(file.length).to be > 0
    end
  end

  context "test_index" do
    it "creates a index.html file" do
      expect(Pathname.new(dest_dir("index.html"))).to exist
    end

    let(:file) { File.read(dest_dir("index.html")) }

    it "ensures index.html file has length" do
      expect(file.length).to be > 0
    end
  end

  context "test_random_index" do
    it "creates a random/index.html file" do
      expect(Pathname.new(dest_dir("random/index.html"))).to exist
    end

    let(:file) { File.read(dest_dir("random/index.html")) }

    it "ensures random/index.html file has length" do
      expect(file.length).to be > 0
    end
  end

  context "test_random_random" do
    it "creates a random/random.html file" do
      expect(Pathname.new(dest_dir("random/random.html"))).to exist
    end

    let(:file) { File.read(dest_dir("random/random.html")) }

    it "ensures random/random.html file has length" do
      expect(file.length).to be > 0
    end
  end

  context "test_reviews_index" do
    it "creates a reviews/index.html file" do
      expect(Pathname.new(dest_dir("reviews/index.html"))).to exist
    end

    let(:file) { File.read(dest_dir("reviews/index.html")) }

    it "ensures reviews/index.html file has length" do
      expect(file.length).to be > 0
    end
  end

  context "test_reviews_test-review-1" do
    it "creates a reviews/test-review-1.html file" do
      expect(Pathname.new(dest_dir("reviews/test-review-1.html"))).to exist
    end

    let(:file) { File.read(dest_dir("reviews/test-review-1.html")) }

    it "ensures reviews/test-review-1.html file has length" do
      expect(file.length).to be > 0
    end
  end

  context "test_reviews_test-review-2" do
    it "creates a reviews/test-review-2.html file" do
      expect(Pathname.new(dest_dir("reviews/test-review-2.html"))).to exist
    end

    let(:file) { File.read(dest_dir("reviews/test-review-2.html")) }

    it "ensures reviews/test-review-2.html file has length" do
      expect(file.length).to be > 0
    end
  end

end
