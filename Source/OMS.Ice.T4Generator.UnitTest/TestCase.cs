#region

using System;
using System.IO;
using System.Reflection;
using System.Text;
using Xunit;

#endregion


namespace OMS.Ice.T4Generator.UnitTest
{
    public abstract class TestCase : IDisposable
    {
        private IGenerator _generator;

        protected IGenerator Generator => _generator;

        protected TestCase()
        {
            _generator = new Generator();
        }


        protected void Run( string template, string expectedResult, params object[] data )
        {
            var generator = Generator;

            //generator.Settings.EndOfLine = EndOfLine.CRLF;

            var stream = new MemoryStream();
            var textWriter = new StreamWriter( stream, Encoding.UTF32 );

            generator.Generate( BuildPath( template ), textWriter, data );

            stream.Seek( 0, SeekOrigin.Begin );
            var textReader = new StreamReader( stream );

            var generatedText = textReader.ReadToEnd();

            Assert.Equal( expectedResult, generatedText );
        }

        protected string Run( string template, object[] data )
        {
            var generator = Generator;

            var stream = new MemoryStream();
            var textWriter = new StreamWriter( stream, Encoding.UTF32 );

            generator.Generate( BuildPath( template ), textWriter, data );

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