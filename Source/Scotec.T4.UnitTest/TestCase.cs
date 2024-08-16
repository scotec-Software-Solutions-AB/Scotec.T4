#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Xunit;

#endregion


namespace Scotec.T4.UnitTest
{
    public abstract class TestCase : IDisposable
    {
        private IGenerator _generator;

        protected IGenerator Generator => _generator;

        protected TestCase()
        {
            _generator = new Generator();
        }


        protected void Run( string templateFile, string expectedResult, IDictionary<string, object> parameters )
        {
            Run( templateFile, false, expectedResult, parameters);
        }

        protected void Run( string templateFile, bool noCache, string expectedResult, IDictionary<string, object> parameters)
        {
            var generator = Generator;

            //generator.Settings.EndOfLine = EndOfLine.CRLF;

            using var stream = new MemoryStream();
            using var textWriter = new StreamWriter( stream, Encoding.UTF32);
            var template = T4Template.FromFile(templateFile);
            generator.Generate( template, textWriter, parameters);

            stream.Seek( 0, SeekOrigin.Begin );
            var textReader = new StreamReader( stream );

            var generatedText = textReader.ReadToEnd();

            Assert.Equal( expectedResult, generatedText );
        }


        protected string Run( string templateFile, IDictionary<string, object> parameters)
        {
            var generator = Generator;

            var stream = new MemoryStream();
            var textWriter = new StreamWriter( stream, Encoding.UTF32 );
            var path = BuildPath(templateFile);
            var template = T4Template.FromFile(path);

            generator.Generate(template, textWriter, parameters);

            stream.Seek( 0, SeekOrigin.Begin );
            var textReader = new StreamReader( stream );

            return textReader.ReadToEnd();
        }


        protected string BuildPath( string file )
        {
            if (Path.IsPathRooted(file))
                return file;

            var path = Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location );
            return Path.Combine( path, file );
        }

        public void Dispose()
        {
            _generator = null;
        }
    }
}