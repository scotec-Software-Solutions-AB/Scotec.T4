# Scotec.T4 — Runtime T4 Text Template Generator

**Scotec.T4** is an in-process T4 text template parser, compiler, and generator for .NET. It allows you to parse and execute T4 text templates at **runtime**, with no dependency on Visual Studio.

## Key Features

- Parse, compile, and execute `.t4` template files at runtime
- Load templates from files or directly from strings
- Pass typed parameters to templates via a dictionary
- Support for C# and Visual Basic templates
- `import`, `assembly`, `parameter`, and `include` directives
- Configurable line-ending style (`CRLF`, `LF`, `CR`, …)
- Macro substitution (`%MacroName%`) in directive attributes
- Optional code-behind files (`codefile` attribute)
- Precompile multiple templates asynchronously for high-load scenarios
- Supports .NET 8 and .NET 10

---

## Installation

```bash
dotnet add package Scotec.T4
```

---

## Quick Start

### Template file — `HelloWorld.t4`

```
<#@ template language="C#" #>
Hello World!
```

### Generate output

```csharp
using Scotec.T4;

var generator = new Generator();
var template  = T4Template.FromFile("HelloWorld.t4");

string result = generator.Build(template).Generate(null);
Console.WriteLine(result); // Hello World!
```

### Pass parameters

```
<#@ template language="C#" #>
<#@ import namespace="System.Collections.Generic" #>
<#@ parameter type="IList<string>" name="friends" #>
<# foreach (var friend in friends) { #>
Hello <#= friend #>!
<# } #>
```

```csharp
var parameters = new Dictionary<string, object>
{
    { "friends", new List<string> { "Mary", "Peter", "Paul" } }
};

string result = generator.Build(template).Generate(parameters);
```

---

## Documentation

| Document | Description |
|---|---|
| [Getting Started](docs/getting-started.md) | Installation and your first template |
| [Template Syntax](docs/template-syntax.md) | Directives, control blocks, and escaping |
| [API Reference](docs/api-reference.md) | `Generator`, `T4Template`, `T4Options`, `TextGenerator` |
| [Advanced Topics](docs/advanced-topics.md) | Precompilation, high-load, macros, error handling |

---

## License

See [LICENSE](LICENSE) for details.