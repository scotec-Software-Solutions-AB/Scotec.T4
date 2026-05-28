#region

using System;
using System.Collections.Generic;
using Xunit;

#endregion


namespace Scotec.T4.UnitTest.Settings
{
    public class LineEndingsTest : TestCase
    {
        [Fact]
        public void TestCSLineEndingsCR()
        {
            Generator.Options.EndOfLine = EndOfLine.CR;
            Generator.Options.TemplateParameters.Add("Language", "C#");

            Run( @"Settings\LineEndingsTest.t4", "a\rb\rc\r", new Dictionary<string, object>() );
        }

        [Fact]
        public void TestCSLineEndingsCRLF()
        {
            Generator.Options.EndOfLine = EndOfLine.CRLF;
            Generator.Options.TemplateParameters.Add("Language", "C#");

            Run( @"Settings\LineEndingsTest.t4", "a\r\nb\r\nc\r\n", new Dictionary<string, object>() );
        }

        [Fact]
        public void TestCSLineEndingsLF()
        {
            Generator.Options.EndOfLine = EndOfLine.LF;
            Generator.Options.TemplateParameters.Add("Language", "C#");

            Run( @"Settings\LineEndingsTest.t4", "a\nb\nc\n", new Dictionary<string, object>() );
        }

        [Fact]
        public void TestCSLineEndingsLFCR()
        {
            Generator.Options.EndOfLine = EndOfLine.LFCR;
            Generator.Options.TemplateParameters.Add("Language", "C#");

            Run( @"Settings\LineEndingsTest.t4", "a\n\rb\n\rc\n\r", new Dictionary<string, object>() );
        }

        [Fact]
        public void TestCSLineEndingsLS()
        {
            Generator.Options.EndOfLine = EndOfLine.LS;
            Generator.Options.TemplateParameters.Add("Language", "C#");

            Run( @"Settings\LineEndingsTest.t4", "a\u2028b\u2028c\u2028", new Dictionary<string, object>() );
        }

        [Fact]
        public void TestCSLineEndingsNEL()
        {
            Generator.Options.EndOfLine = EndOfLine.NEL;
            Generator.Options.TemplateParameters.Add("Language", "C#");

            Run( @"Settings\LineEndingsTest.t4", "a\u0085b\u0085c\u0085", new Dictionary<string, object>() );
        }

        [Fact]
        public void TestCSLineEndingsPS()
        {
            Generator.Options.EndOfLine = EndOfLine.PS;
            Generator.Options.TemplateParameters.Add("Language", "C#");

            Run( @"Settings\LineEndingsTest.t4", "a\u2029b\u2029c\u2029", new Dictionary<string, object>() );
        }

        [Fact]
        public void TestVBLineEndingsCR()
        {
            Generator.Options.EndOfLine = EndOfLine.CR;
            Generator.Options.TemplateParameters.Add("Language", "VB");

            Run(@"Settings\LineEndingsTest.t4", "a\rb\rc\r", new Dictionary<string, object>());
        }

        [Fact]
        public void TestVBLineEndingsCRLF()
        {
            Generator.Options.EndOfLine = EndOfLine.CRLF;
            Generator.Options.TemplateParameters.Add("Language", "VB");

            Run(@"Settings\LineEndingsTest.t4", "a\r\nb\r\nc\r\n", new Dictionary<string, object>());
        }

        [Fact]
        public void TestVBLineEndingsLF()
        {
            Generator.Options.EndOfLine = EndOfLine.LF;
            Generator.Options.TemplateParameters.Add("Language", "VB");

            Run(@"Settings\LineEndingsTest.t4", "a\nb\nc\n", new Dictionary<string, object>());
        }

        [Fact]
        public void TestVBLineEndingsLFCR()
        {
            Generator.Options.EndOfLine = EndOfLine.LFCR;
            Generator.Options.TemplateParameters.Add("Language", "VB");

            Run(@"Settings\LineEndingsTest.t4", "a\n\rb\n\rc\n\r", new Dictionary<string, object>());
        }

        [Fact]
        public void TestVBLineEndingsLS()
        {
            Generator.Options.EndOfLine = EndOfLine.LS;
            Generator.Options.TemplateParameters.Add("Language", "VB");

            Run(@"Settings\LineEndingsTest.t4", "a\u2028b\u2028c\u2028", new Dictionary<string, object>());
        }

        [Fact]
        public void TestVBLineEndingsNEL()
        {
            Generator.Options.EndOfLine = EndOfLine.NEL;
            Generator.Options.TemplateParameters.Add("Language", "VB");

            Run(@"Settings\LineEndingsTest.t4", "a\u0085b\u0085c\u0085", new Dictionary<string, object>());
        }

        [Fact]
        public void TestVBLineEndingsPS()
        {
            Generator.Options.EndOfLine = EndOfLine.PS;
            Generator.Options.TemplateParameters.Add("Language", "VB");

            Run(@"Settings\LineEndingsTest.t4", "a\u2029b\u2029c\u2029", new Dictionary<string, object>());
        }
    }
}