using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Localization.Internal;
using Xunit;

namespace Microsoft.Extensions.Localization
{
    public class POParserTest
    {
        [Fact]
        public void ParseStream_StrBeforeId()
        {
            var parser = GetPOParser("StrBeforeId");

            Assert.Throws<FormatException>(() => { parser.ParseLocalizationStream(); });
        }

        [Fact]
        public void ParseStream_Plural()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void ParseStream_UnescapedQuote()
        {
            var parser = GetPOParser("UnescapedQuote");

            Assert.Throws<FormatException>(() => parser.ParseLocalizationStream());
        }

        [Fact]
        public void ParseStream_QuotesInValue()
        {
            var parser = GetPOParser("BaseFile");

            var result = parser.ParseLocalizationStream();

            Assert.Equal("str with \" strings", result["id with \" strings"].Translation);
            Assert.Equal("str with '", result["ID with '"].Translation);
            Assert.Equal("str with \"", result["ID with \""].Translation);
            Assert.Equal("'", result["'"].Translation);
            Assert.Equal("\"", result["\""].Translation);
        }

        [Fact]
        public void ParseStream_MultipleReferences()
        {
            var parser = GetPOParser("BaseFile");

            var result = parser.ParseLocalizationStream();

            Assert.Equal(3, result["multiple reference"].References.Count);
            Assert.Equal("reference.xml:1", result["multiple reference"].References.First());
        }

        [Fact]
        public void parseStream_MultipleFlags()
        {
            var parser = GetPOParser("BaseFile");

            var result = parser.ParseLocalizationStream();

            Assert.Equal(2, result["flags"].Flags.Count);
            Assert.Equal("flag2", result["flags"].Flags[1]);
        }

        [Fact]
        public void ParseStream_MultiLineIdAndTranslation()
        {
            var parser = GetPOParser("BaseFile");

            var result = parser.ParseLocalizationStream();

            Assert.Equal("Multi line str", result["this is a multiline"].Translation);
        }

        [Fact]
        public void ParseStream_DuplicateIDs()
        {
            var parser = GetPOParser("DuplicateIds");
            Assert.Throws<ArgumentException>(() => parser.ParseLocalizationStream());
        }

        private POParser GetPOParser(string file)
        {
            var stream = File.OpenRead(string.Format($"./POFiles/{file}.po"));

            return new POParser(stream);
        }
    }
}
