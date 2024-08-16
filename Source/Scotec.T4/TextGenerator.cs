using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Scotec.T4;

public class TextGenerator
{
    private readonly Type _generatorType;
    private readonly string _name;
    private readonly string _lineEndings;

    internal TextGenerator(Type generatorType, string name, string lineEndings)
    {
        _generatorType = generatorType;
        _name = name;
        _lineEndings = lineEndings;
    }

    public async Task Generate(TextWriter writer, IDictionary<string, object> parameters)
    {
        var parameterList = new List<object> { _lineEndings, parameters }.ToArray();

        var generator = (T4Generator)Activator.CreateInstance(_generatorType, parameterList);
        if (generator == null)
        {
            throw new T4Exception($"Could not create instance of T4 generator. (Generator name: {_name})");
        }

        await generator.GenerateAsync(writer);
    }
}
