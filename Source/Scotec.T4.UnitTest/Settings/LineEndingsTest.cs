#region

using Xunit;

#endregion


namespace Scotec.T4.UnitTest.Settings
{
    public class LineEndingsTest : TestCase
    {
        [Fact]
        public void TestCSLineEndingsCR()
        {
            Generator.Settings.EndOfLine = EndOfLine.CR;
            Generator.Settings.TemplateParameters.Add("Language", "C#");

            Run( @"Settings\LineEndingsTest.t4", true, "a\rb\rc\r", new object[0] );
        }

        [Fact]
        public void TestCSLineEndingsCRLF()
        {
            Generator.Settings.EndOfLine = EndOfLine.CRLF;
            Generator.Settings.TemplateParameters.Add("Language", "C#");

            Run( @"Settings\LineEndingsTest.t4", true, "a\r\nb\r\nc\r\n", new object[0] );
        }

        [Fact]
        public void TestCSLineEndingsLF()
        {
            Generator.Settings.EndOfLine = EndOfLine.LF;
            Generator.Settings.TemplateParameters.Add("Language", "C#");

            Run( @"Settings\LineEndingsTest.t4", true, "a\nb\nc\n", new object[0] );
        }

        [Fact]
        public void TestCSLineEndingsLFCR()
        {
            Generator.Settings.EndOfLine = EndOfLine.LFCR;
            Generator.Settings.TemplateParameters.Add("Language", "C#");

            Run( @"Settings\LineEndingsTest.t4", true, "a\n\rb\n\rc\n\r", new object[0] );
        }

        [Fact]
        public void TestCSLineEndingsLS()
        {
            Generator.Settings.EndOfLine = EndOfLine.LS;
            Generator.Settings.TemplateParameters.Add("Language", "C#");

            Run( @"Settings\LineEndingsTest.t4", true, "a\u2028b\u2028c\u2028", new object[0] );
        }

        [Fact]
        public void TestCSLineEndingsNEL()
        {
            Generator.Settings.EndOfLine = EndOfLine.NEL;
            Generator.Settings.TemplateParameters.Add("Language", "C#");

            Run( @"Settings\LineEndingsTest.t4", true, "a\u0085b\u0085c\u0085", new object[0] );
        }

        [Fact]
        public void TestCSLineEndingsPS()
        {
            Generator.Settings.EndOfLine = EndOfLine.PS;
            Generator.Settings.TemplateParameters.Add("Language", "C#");

            Run( @"Settings\LineEndingsTest.t4", true, "a\u2029b\u2029c\u2029", new object[0] );
        }

        [Fact]
        public void TestVBLineEndingsCR()
        {
            Generator.Settings.EndOfLine = EndOfLine.CR;
            Generator.Settings.TemplateParameters.Add("Language", "VB");

            Run(@"Settings\LineEndingsTest.t4", true, "a\rb\rc\r", new object[0]);
        }

        [Fact]
        public void TestVBLineEndingsCRLF()
        {
            Generator.Settings.EndOfLine = EndOfLine.CRLF;
            Generator.Settings.TemplateParameters.Add("Language", "VB");

            Run(@"Settings\LineEndingsTest.t4", true, "a\r\nb\r\nc\r\n", new object[0]);
        }

        [Fact]
        public void TestVBLineEndingsLF()
        {
            Generator.Settings.EndOfLine = EndOfLine.LF;
            Generator.Settings.TemplateParameters.Add("Language", "VB");

            Run(@"Settings\LineEndingsTest.t4", true, "a\nb\nc\n", new object[0]);
        }

        [Fact]
        public void TestVBLineEndingsLFCR()
        {
            Generator.Settings.EndOfLine = EndOfLine.LFCR;
            Generator.Settings.TemplateParameters.Add("Language", "VB");

            Run(@"Settings\LineEndingsTest.t4", true, "a\n\rb\n\rc\n\r", new object[0]);
        }

        [Fact]
        public void TestVBLineEndingsLS()
        {
            Generator.Settings.EndOfLine = EndOfLine.LS;
            Generator.Settings.TemplateParameters.Add("Language", "VB");

            Run(@"Settings\LineEndingsTest.t4", true, "a\u2028b\u2028c\u2028", new object[0]);
        }

        [Fact]
        public void TestVBLineEndingsNEL()
        {
            Generator.Settings.EndOfLine = EndOfLine.NEL;
            Generator.Settings.TemplateParameters.Add("Language", "VB");

            Run(@"Settings\LineEndingsTest.t4", true, "a\u0085b\u0085c\u0085", new object[0]);
        }

        [Fact]
        public void TestVBLineEndingsPS()
        {
            Generator.Settings.EndOfLine = EndOfLine.PS;
            Generator.Settings.TemplateParameters.Add("Language", "VB");

            Run(@"Settings\LineEndingsTest.t4", true, "a\u2029b\u2029c\u2029", new object[0]);
        }
    }
}