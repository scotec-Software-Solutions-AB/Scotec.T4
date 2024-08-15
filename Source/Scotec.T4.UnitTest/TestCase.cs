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


        protected void Run( string template, string expectedResult, IDictionary<string, object> parameters )
        {
            Run( template, false, expectedResult, parameters);
        }

        protected void Run( string template, bool noCache, string expectedResult, IDictionary<string, object> parameters)
        {
            var generator = Generator;

            //generator.Settings.EndOfLine = EndOfLine.CRLF;

            var stream = new MemoryStream();
            var textWriter = new StreamWriter( stream, Encoding.UTF32 );

            generator.Generate( BuildPath( template ), noCache, textWriter, parameters);

            stream.Seek( 0, SeekOrigin.Begin );
            var textReader = new StreamReader( stream );

            var generatedText = textReader.ReadToEnd();

            Assert.Equal( expectedResult, generatedText );
        }

        protected string Run( string template, IDictionary<string, object> parameters)
        {
            return Run( template, false, parameters);
        }

        protected string Run( string template, bool noCache, IDictionary<string, object> parameters)
        {
            var generator = Generator;

            var stream = new MemoryStream();
            var textWriter = new StreamWriter( stream, Encoding.UTF32 );

            generator.Generate( BuildPath( template ), noCache, textWriter, parameters);

            stream.Seek( 0, SeekOrigin.Begin );
            var textReader = new StreamReader( stream );

            return textReader.ReadToEnd();
        }


        protected string BuildPath( string template )
        {
            if (Path.IsPathRooted(template))
                return template;

            var path = Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location );
            return Path.Combine( path, template );
        }

        public void Dispose()
        {
            _generator = null;
        }
    }
}