using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;

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
                new QuoteParser(','),
                new NewlineParser(',')
            };

            this.target = new RecordParser(parsers);
        }

        [Theory]
        [MemberData(nameof(ValidFiles))]
        public void GivenFileWithHeader_ShouldParseFile(string input, string[][] expected)
        {
            using (var reader = new StringReader(input.Trim()))
            {
                List<string[]> actual = new List<string[]>();
                var output = target.Parse(reader);
                output.Subscribe(
                    rec => actual.Add(rec.Item1),
                    ex => throw ex);

                output.LastAsync().Wait();

                Assert.Equal(expected, actual);
            }
        }

        public static IEnumerable<object[]> ValidFiles()
        {
            var files = Directory.EnumerateFiles("test-files", "0*.csv");
            return files.Select(file =>
            {
                var input = File.ReadAllText(file);
                var expected = File.ReadAllLines($"{file}.expected")
                    .Select(line => line.Split('|').ToArray())
                    .ToArray();

                return new object[] { input, expected };
            });
        }

        [Fact]
        public void GivenInvalidRecord_WithQuoteInsideUnquotedField_ShouldThrowParseException()
        {
            AssertParseException("field 1,field\" 2", 13);
        }

        [Fact]
        public void GivenInvalidRecord_WithSingleQuoteInsideQuotedField_ShouldThrowParseException()
        {
            AssertParseException("field 1,\"quoted field with \"single quote\"", 28);
        }
 
        [Fact]
        public void GivenInvalidRecord_WithOpenQuotedFieldAtTheEndOfFile_ShouldThrowParseException()
        {
            AssertParseException("field 1,\"open quoted field", 26);
        }

        [Fact]
        public void GivenInvalidRecord_WithOpenQuotedFieldAtTheEndOfLine_ShouldThrowParseException()
        {
            AssertParseException("field 1,\"open quoted field\nfield 2,field 3", 26);
        }

        private void AssertParseException(string record, int column)
        {
            var output = target.Parse(new StringReader(record));
            Exception actual = null;
            output.Subscribe(_ => { }, ex => actual = ex);
            output.LastOrDefaultAsync().Wait();
            Assert.IsType<ParseException>(actual);
            Assert.Equal(column, ((ParseException)actual).Column);
        }
   }
}
