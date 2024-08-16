using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Scotec.T4;

/// <summary>
/// </summary>
public class T4Template
{
    private T4Template(string template)
    {
        Template = template;
        File = null;
        // TODO Generate hashcode.
        Id = GetHashCode(template); 
    }

    private T4Template(FileInfo file)
    {
        File = file;
        Template = null;
        Id = GetHashCode(file.Name);
    }

    /// <summary>
    /// 
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// </summary>
    public FileInfo File { get; }

    /// <summary>
    /// </summary>
    public string Template { get; }

    /// <summary>
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public static T4Template FromFile(string file)
    {
        return new T4Template(new FileInfo(MakePathAbsolute(file)));
    }

    /// <summary>
    /// </summary>
    /// <param name="template"></param>
    /// <returns></returns>
    public static T4Template FromString(string template)
    {
        return new T4Template(template);
    }

    private static string GetHashCode(string input)
    {
        var algorithm = SHA256.Create();
        var digest = algorithm.ComputeHash(Encoding.UTF8.GetBytes(input));
        return BitConverter.ToString(digest);
    }

    private static string MakePathAbsolute(string file)
    {
        if (Path.IsPathRooted(file))
            return file;

        var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        return Path.Combine(path, file);
    }

}
