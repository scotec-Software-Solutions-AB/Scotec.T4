#region

using System.IO;
using Xunit;

#endregion


namespace Scotec.T4Generator.UnitTest.TextGenerator
{
    public class TextGeneratorTest : TestCase
    {
        [Fact]
        public void TestEscapeMarkupTags()
        {
            var result = Run( @"TextGenerator\EscapeMarkupTagsTest.t4", new object[0] );

            var tempFile = Path.GetTempFileName();

            var writer = File.CreateText( tempFile );
            writer.Write( result );
            writer.Close();

            Run( tempFile, "TEXT 3 TEXT a", new object[0] );

            File.Delete( tempFile );
        }

        [Fact]
        public void TestForLoopMultiLine()
        {
            Run( @"TextGenerator\ForLoopMultiLineTest.t4", "0\r\n1\r\n2\r\n3\r\n4\r\n5\r\n6\r\n7\r\n8\r\n9\r\n", new object[0] );
        }

        [Fact]
        public void TestForLoopSingleLine()
        {
            Run( @"TextGenerator\ForLoopSingleLineTest.t4", "0123456789", new object[0] );
        }

        [Fact]
        public void TestForLoopSingleLineWithWrite()
        {
            Run( @"TextGenerator\ForLoopSingleLineWithWriteTest.t4", "0123456789", new object[0] );
        }

        [Fact]
        public void TestTextLine()
        {
            Run( @"TextGenerator\TextLineTest.t4", "abcde", new object[0] );
        }

        [Fact]
        public void TestTextLines()
        {
            Run( @"TextGenerator\TextLinesTest.t4", "abcde\r\n12345", new object[0] );
        }

        [Fact]
        public void TestWriteToTextWriter()
        {
            Run(@"TextGenerator\WriteToTextWriterTest.t4", "0123456789", new object[0]);
        }
    }
}