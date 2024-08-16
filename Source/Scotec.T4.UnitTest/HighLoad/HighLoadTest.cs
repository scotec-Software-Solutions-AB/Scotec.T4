#region

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

#endregion


namespace Scotec.T4.UnitTest.HighLoad
{
    public class HighLoadTest : TestCase
    {
        [Fact]
        public void TestHighLoad()
        {
            var tempPath = @"C:\Temp\T4";

            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory( tempPath );
            }
            else
            {
                Directory.Delete(tempPath, true);
                Directory.CreateDirectory(tempPath);
            }

            var templateFile = BuildPath(@"HighLoad\HighLoadTest.t4");


            for (int i = 0; i < 100; i++)
            {
                var tempFile = Path.Combine( tempPath, "test" + i + ".t4" );
                File.Copy( templateFile, tempFile );
                //Run(tempFile, "test", new object[0]);
            }

            var files = Directory.GetFiles( tempPath, "*.t4" )
                                 .Select(file => T4Template.FromFile(file))
                                 .ToList();

            var t1 = DateTime.Now;

            // Precompile templates.
#if !FRAMEWORK35
            var task = Generator.Compile( files );
            //task.Wait();
#else
            Generator.Compile( files );
#endif //FRAMEWORK35

            // Parallel text generation.
            Parallel.ForEach(files, file =>
                {
                    var stream = new MemoryStream();
                    var textWriter = new StreamWriter(stream, Encoding.UTF32);
                    var path = BuildPath(file.File); 
                    var template = T4Template.FromFile(path);
                    Generator.Generate(template, textWriter, null);
                });


            var diffTime = DateTime.Now - t1;
            Console.WriteLine(string.Format( "Execution time: {0}.{1} s", diffTime.Seconds, diffTime.Milliseconds ));

        }
    }
}