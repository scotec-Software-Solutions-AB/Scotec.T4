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
    private T4Template(string template, string name)
    {
        Template = template;
        Name = name;
        File = null;
    }

    private T4Template(string file)
    {
        File = file;
        Name = Path.GetFileNameWithoutExtension(file);
        Template = null;
    }


    /// <summary>
    /// 
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// </summary>
    public string File { get; }

    /// <summary>
    /// </summary>
    public string Template { get; }

    /// <summary>
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public static T4Template FromFile(string file)
    {
        return new T4Template(file);
    }

    /// <summary>
    /// </summary>
    /// <param name="template"></param>
    /// <returns></returns>
    public static T4Template FromString(string template, string name)
    {
        return new T4Template(template, name);
    }
}
