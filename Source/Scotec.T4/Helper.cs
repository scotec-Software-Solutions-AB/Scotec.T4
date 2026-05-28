using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Scotec.T4;

internal static class Helper
{
    public static string FindFile(string fileName, IList<string> searchPaths)
    {
        if (Path.IsPathRooted(fileName))
        {
            if (File.Exists(fileName))
            {
                return fileName;
            }

            throw new T4Exception($"Cannot find file '{fileName}'.");
        }

        foreach (var path in searchPaths)
        {
            var fullPath = Path.Combine(path, fileName);
            if (File.Exists(fullPath))
            {
                return fullPath;
            }
        }

        throw new T4Exception($"Cannot find file '{fileName}'.");
    }
}
