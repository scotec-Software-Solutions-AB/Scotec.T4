using System.IO;

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
        Id = template; 
    }

    private T4Template(FileInfo file)
    {
        File = file;
        Template = null;
        Id = file.Name;
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
    public static T4Template FromFile(FileInfo file)
    {
        return new T4Template(file);
    }

    /// <summary>
    /// </summary>
    /// <param name="template"></param>
    /// <returns></returns>
    public static T4Template FromString(string template)
    {
        return new T4Template(template);
    }
}
