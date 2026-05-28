using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Scotec.T4;

internal class Templates
{
    public static string CsCodeTemplate => ReadResource();
    public static string CsIncludeTemplate => ReadResource();
    public static string VbCodeTemplate => ReadResource();
    public static string VbIncludeTemplate => ReadResource();

    private static string ReadResource([CallerMemberName] string name = null)
    {
        // Determine path
        var assembly = Assembly.GetExecutingAssembly();
        // Format: "{Namespace}.{Folder}.{filename}.{Extension}"
        var resourcePath =
            assembly.GetManifestResourceNames()
                    .Single(str => str.Contains(name!));

        using var stream = assembly.GetManifestResourceStream(resourcePath);
        using var reader = new StreamReader(stream!);
        return reader.ReadToEnd();
    }
}
