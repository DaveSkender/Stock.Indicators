# coding: utf-8

require "spec_helper"


describe "CSSminify2" do

  context "application" do

    it "minifies CSS" do
      source = File.open(File.expand_path("../sample.css", __FILE__), "r:UTF-8").read
      minified = CSSminify2.compress(source)
      minified.length.should < source.length
      lambda {
        CSSminify2.compress(minified)
      }.should_not raise_error
    end

    it "honors the specified maximum line length" do
      source = <<-EOS
        .classname1 {
            border: none;
            background: none;
            outline: none;
        }
        .classname2 {
            border: none;
            background: none;
            outline: none;
        }
      EOS
      minified = CSSminify2.compress(source, 30)
      minified.split("\n").length.should eq(2)
      minified.should eq(".classname1{border:0;background:0;outline:0}\n.classname2{border:0;background:0;outline:0}")
    end

    it "handles strings as input format" do
      lambda {
        CSSminify2.compress(File.open(File.expand_path("../sample.css", __FILE__), "r:UTF-8").read).should_not be_empty
      }.should_not raise_error
    end

    it "handles files as input format" do
      lambda {
        CSSminify2.compress(File.open(File.expand_path("../sample.css", __FILE__), "r:UTF-8")).should_not be_empty
      }.should_not raise_error
    end

    it "works as both class and class instance" do
      lambda {
        CSSminify2.compress(File.open(File.expand_path("../sample.css", __FILE__), "r:UTF-8").read).should_not be_empty
        CSSminify2.new.compress(File.open(File.expand_path("../sample.css", __FILE__), "r:UTF-8").read).should_not be_empty
      }.should_not raise_error
    end

  end


  context "compression" do

    it "removes comments and white space" do
      source = <<-EOS
        /*****
          Multi-line comment
          before a new class name
        *****/
        .classname {
            /* comment in declaration block */
            font-weight: normal;
        }
      EOS
      CSSminify2.compress(source).should eq('.classname{font-weight:normal}')
    end

    it "preserves special comments" do
      source = <<-EOS
        /*!
          (c) Very Important Comment
        */
        .classname {
            /* comment in declaration block */
            font-weight: normal;
        }
      EOS
      result = <<-EOS
/*!
          (c) Very Important Comment
        */.classname{font-weight:normal}
      EOS
      (CSSminify2.compress(source) + "\n").should eq(result)
    end

    it "removes last semi-colon in a block" do
      source = <<-EOS
        .classname {
            border-top: 1px;
            border-bottom: 2px;
        }
      EOS
      CSSminify2.compress(source).should eq('.classname{border-top:1px;border-bottom:2px}')
    end

    it "removes extra semi-colons" do
      source = <<-EOS
        .classname {
            border-top: 1px; ;
            border-bottom: 2px;;;
        }
      EOS
      CSSminify2.compress(source).should eq('.classname{border-top:1px;border-bottom:2px}')
    end

    it "removes empty declarations" do
      source = <<-EOS
        .empty { ;}
        .nonempty {border: 0;}
      EOS
      CSSminify2.compress(source).should eq('.nonempty{border:0}')
    end

    it "simplifies zero values" do
      source = <<-EOS
        a {
            margin: 0px 0pt 0em 0%;
            background-position: 0 0ex;
            padding: 0in 0cm 0mm 0pc
        }
      EOS
      CSSminify2.compress(source).should eq('a{margin:0;background-position:0 0;padding:0}')
    end

    it "simplifies zero values preserving unit when necessary" do
      source = <<-EOS
        @-webkit-keyframes anim {
          0% {
            left: 0;
          }
          100% {
            left: -100%;
          }
        }
        @-moz-keyframes anim {
          0% {
            left: 0;
          }
          100% {
            left: -100%;
          }
        }
        @-ms-keyframes anim {
          0% {
            left: 0;
          }
          100% {
            left: -100%;
          }
        }
        @-o-keyframes anim {
          0% {
            left: 0;
          }
          100% {
            left: -100%;
          }
        }
        @keyframes anim {
          0% {
            left: 0;
          }
          100% {
            left: -100%;
          }
        }
      EOS
      CSSminify2.compress(source).should eq('@-webkit-keyframes anim{0%{left:0}100%{left:-100%}}@-moz-keyframes anim{0%{left:0}100%{left:-100%}}@-ms-keyframes anim{0%{left:0}100%{left:-100%}}@-o-keyframes anim{0%{left:0}100%{left:-100%}}@keyframes anim{0%{left:0}100%{left:-100%}}')
    end

    it "removes leading zeros from floats" do
      source = <<-EOS
        .classname {
            margin: 0.6px 0.333pt 1.2em 8.8cm;
        }
      EOS
      CSSminify2.compress(source).should eq('.classname{margin:.6px .333pt 1.2em 8.8cm}')
    end

    it "removes leading zeros from groups" do
      source = <<-EOS
        a {
          margin: 0px 0pt 0em 0%;
          _padding-top: 0ex;
          background-position: 0 0;
          padding: 0in 0cm 0mm 0pc;
          transition: opacity .0s;
          transition-delay: 0.0ms;
          transform: rotate3d(0grad, 0rad, 0deg);
          pitch: 0khz;
          pitch:
        0hz /* intentionally on next line */;
        }
      EOS
      CSSminify2.compress(source).should eq('a{margin:0;_padding-top:0;background-position:0 0;padding:0;transition:opacity 0;transition-delay:0;transform:rotate3d(0,0,0);pitch:0;pitch:0}')
    end

    it "simplifies color values but preserves filter properties, RGBa values and ID strings" do
      source = <<-EOS
        .color-me {
            color: rgb(123, 123, 123);
            border-color: #ffeedd;
            background: none repeat scroll 0 0 rgb(255, 0,0);
        }
      EOS
      CSSminify2.compress(source).should eq('.color-me{color:#7b7b7b;border-color:#fed;background:none repeat scroll 0 0 #f00}')

      source = <<-EOS
        #AABBCC {
            color: rgba(1, 2, 3, 4);
            filter: chroma(color="#FFFFFF");
        }
      EOS
      CSSminify2.compress(source).should eq('#AABBCC{color:rgba(1,2,3,4);filter:chroma(color="#FFFFFF")}')
    end

    it "only keeps the first charset declaration" do
      source = <<-EOS
        @charset "utf-8";
        #foo {
            border-width: 1px;
        }

        /* second css, merged */
        @charset "another one";
        #bar {
            border-width: 10px;
        }
      EOS
      CSSminify2.compress(source).should eq('@charset "utf-8";#foo{border-width:1px}#bar{border-width:10px}')
    end

    it "simplifies the IE opacity filter syntax" do
      source = <<-EOS
        .classname {
            -ms-filter: "progid:DXImageTransform.Microsoft.Alpha(Opacity=80)"; /* IE 8 */
            filter: progid:DXImageTransform.Microsoft.Alpha(Opacity=80);       /* IE < 8 */
        }
      EOS
      CSSminify2.compress(source).should eq('.classname{-ms-filter:"alpha(opacity=80)";filter:alpha(opacity=80)}')
    end

    it "replaces 'none' values with 0 where allowed" do
      source = <<-EOS
        .classname {
            border: none;
            background: none;
            outline: none;
        }
      EOS
      CSSminify2.compress(source).should eq('.classname{border:0;background:0;outline:0}')
    end

    it "tolerates underscore/star hacks" do
      source = <<-EOS
        #element {
            width: 1px;
            *width: 2px;
            _width: 3px;
        }
      EOS
      CSSminify2.compress(source).should eq('#element{width:1px;*width:2px;_width:3px}')
    end

    it "tolerates child selector hacks" do
      source = <<-EOS
        html >/**/ body p {
            color: blue;
        }
      EOS
      CSSminify2.compress(source).should eq('html>/**/body p{color:blue}')
    end

    it "tolerates IE5/Mac hacks" do
      source = <<-EOS
        /* Ignore the next rule in IE mac \\*/
        .selector {
            color: khaki;
        }
        /* Stop ignoring in IE mac */
      EOS
      CSSminify2.compress(source).should eq('/*\*/.selector{color:khaki}/**/')
    end

    it "tolerates box model hacks" do
      source = <<-EOS
        #elem {
            width: 100px; /* IE */
            voice-family: "\\"}\\"";
            voice-family:inherit;
            width: 200px; /* others */
        }
        html>body #elem {
            width: 200px; /* others */
        }
      EOS
      CSSminify2.compress(source).should eq('#elem{width:100px;voice-family:"\"}\"";voice-family:inherit;width:200px}html>body #elem{width:200px}')
    end

    it "should pass all the original tests included in the YUI compressor package" do
      puts "Now running original YUI compressor test files:"

      files = Dir.glob(File.join(File.dirname(__FILE__), 'tests', '*.css'))

      for file in files do
        print "  -- testing #{file} ..."
        CSSminify2.compress(File.read(file)).chomp.strip.should eq(File.read(file + '.min').chomp.strip)
        print "successful\n"
      end
    end

  end

end
