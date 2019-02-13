using System;
using System.IO;
using System.Linq;

using Xunit;
using Xunit.Abstractions;

using DTDemo.DealProcessing;
using DTDemo.DealProcessing.Csv;

namespace DTDemo.Test.DealProcessing
{
    public class RecordParserTests
    {
        private readonly RecordParser target;

        public RecordParserTests()
        {
            var parsers = new IParser[] {
                new InitialParser(','),
                new GenericParser(','),
                new StringParser(','),
                new QuoteParser(',')
            };

            this.target = new RecordParser(parsers);
        }

        [Theory]
        [InlineData("field 1,field 2", new[] { "field 1", "field 2" })]
        [InlineData("field 1,\"quoted field 2\"", new[] { "field 1", "quoted field 2" })]
        [InlineData("\"quoted field with \"\"inner quote\"\"\"", new[] { "quoted field with \"inner quote\"" })]
        [InlineData("\"quoted field with, inner, comma\"", new[] { "quoted field with, inner, comma" })]
        [InlineData("should be trimmed field 1 ,    should be   trimmed field 2  ", new[] { "should be trimmed field 1", "should be   trimmed field 2" })]
        [InlineData(
            "5469,Milli Fulton,Sun of Saskatoon,2017 Ferrari 488 Spider,\"429,987\",6/19/2018",
            new[] { "5469", "Milli Fulton", "Sun of Saskatoon", "2017 Ferrari 488 Spider", "429,987", "6/19/2018" })]
        public void GivenValidRecord_ShouldParseFields(string record, string[] expected)
        {
            var actual = target.Parse(record);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GivenInvalidRecord_WithQuoteInsideUnquotedField_ShouldThrowParseException()
        {
            var record = "field 1,field\" 2";
            Action act = () => target.Parse(record);
            Assert.Throws<ParseException>(act);
        }

        [Fact]
        public void GivenInvalidRecord_WithSingleQuoteInsideQuotedField_ShouldThrowParseException()
        {
            var record = "field 1,\"quoted field with \"single quote\"";
            Action act = () => target.Parse(record);
            Assert.Throws<ParseException>(act);
        }

        [Fact]
        public void GivenInvalidRecord_WithOpenQuotedFieldAtTheEnd_ShouldThrowParseException()
        {
            var record = "field 1,\"open quoted field";
            Action act = () => target.Parse(record);
            Assert.Throws<ParseException>(act);
        }
    }
}
