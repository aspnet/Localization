using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace Microsoft.Extensions.Localization
{
    public class POParserTest
    {
        public static POParser Parser = new POParser();

        [Fact]
        public void ParseStream_StrBeforeId()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void ParseStream_Plural()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void ParseStream_QuotesInValue()
        {
            var stream = GetStream("BaseFile");

            var result = Parser.ParseLocalizationStream(stream);

            Assert.Equal("str with '", result["ID with '"].Translation);
            Assert.Equal("str with \"", result["ID with \""].Translation);
            Assert.Equal("'", result["'"].Translation);
            Assert.Equal("\"", result["\""].Translation);
        }

        [Fact]
        public void ParseStream_MultipleReferences()
        {
            var stream = GetStream("BaseFile");

            var result = Parser.ParseLocalizationStream(stream);

            Assert.Equal(3, result["multiple reference"].References.Count);
            Assert.Equal("reference.xml:1", result["multiple reference"].References.First());
        }

        [Fact]
        public void parseStream_MultipleFlags()
        {
            var stream = GetStream("BaseFile");

            var result = Parser.ParseLocalizationStream(stream);

            Assert.Equal(2, result["flags"].Flags.Count);
            Assert.Equal("flag2", result["flags"].Flags[1]);
        }

        [Fact]
        public void ParseStream_MultiLineIdAndTranslation()
        {
            var stream = GetStream("BaseFile");

            var result = Parser.ParseLocalizationStream(stream);

            Assert.Equal("Multi line str", result["this is a multiline"].Translation);
        }

        [Fact]
        public void ParseStream_DuplicateIDs()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void ParseStream_SecondItem_BothComplete()
        {
            throw new NotImplementedException();
        }

        private POParser GetPOParser()
        {
            return new POParser();
        }

        private Stream GetStream(string file)
        {
            return File.OpenRead(string.Format($"./POFiles/{file}.po"));
        }
    }
}
