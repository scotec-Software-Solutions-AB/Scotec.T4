# API Reference

## `T4Template`

Represents a T4 template. Use the static factory methods to create instances.

### Factory Methods

| Method | Description |
|---|---|
| `T4Template.FromFile(string file)` | Creates a template from a file path. The template name is derived from the file name without extension. |
| `T4Template.FromString(string template, string name)` | Creates a template from a string. A name must be provided explicitly. |

### Properties

| Property | Type | Description |
|---|---|---|
| `Name` | `string` | The template name, used as the base for the generated class name. |
| `File` | `string` | The source file path, or `null` when created from a string. |
| `Template` | `string` | The raw template content, or `null` when created from a file. |

---

## `Generator`

The main entry point for generating text from a T4 template. Implements `IGenerator`.

### Constructors

```csharp
// Uses default T4Options
var generator = new Generator();

// Uses custom options
var options   = new T4Options { EndOfLine = EndOfLine.LF };
var generator = new Generator(options);
```

### Properties

| Property | Type | Description |
|---|---|---|
| `Options` | `T4Options` | The generator configuration. |

### Methods

#### `Build`

Parses and compiles the template, returning a `TextGenerator` that can be executed multiple times with different parameters.

```csharp
TextGenerator textGen = generator.Build(template);
```

Use this when you want to compile a template once and generate output many times.

---

#### `Generate`

Parses, compiles, and executes the template, writing the result to a `TextWriter`.

```csharp
generator.Generate(T4Template template, TextWriter output, IDictionary<string, object> parameters);
```

```csharp
using var stream = new MemoryStream();
using var writer = new StreamWriter(stream);

generator.Generate(template, writer, new Dictionary<string, object>
{
    { "title", "My Report" }
});
```

---

#### `GenerateToFile`

Parses, compiles, and executes the template, writing the result directly to a file. The output directory is created automatically.

```csharp
// Default encoding (UTF-8)
generator.GenerateToFile(T4Template template, string outputFile, IDictionary<string, object> parameters);

// Explicit encoding
generator.GenerateToFile(T4Template template, string outputFile, Encoding encoding, IDictionary<string, object> parameters);
```

```csharp
generator.GenerateToFile(template, @"C:\Output\report.txt", new Dictionary<string, object>
{
    { "title", "My Report" }
});
```

---

#### `Compile`

Asynchronously precompiles a collection of templates without producing output. Precompiling is optional but speeds up subsequent `Generate` calls, especially when processing many templates.

```csharp
Task Compile(IEnumerable<T4Template> templates);
```

```csharp
var templates = Directory.GetFiles(templateDir, "*.t4")
                         .Select(T4Template.FromFile)
                         .ToList();

// Fire and forget — generation calls benefit from cached assemblies
var compileTask = generator.Compile(templates);

// ... you may await later, or proceed immediately
await compileTask;
```

---

## `TextGenerator`

Returned by `Generator.Build`. Holds the compiled generator type and allows repeated execution.

### Methods

#### `Generate(TextWriter writer, IDictionary<string, object> parameters)`

Executes the compiled template and writes to the supplied `TextWriter`.

```csharp
var textGen = generator.Build(template);

foreach (var item in data)
{
    using var stream = new MemoryStream();
    using var writer = new StreamWriter(stream);
    textGen.Generate(writer, new Dictionary<string, object> { { "model", item } });
    // ...
}
```

#### `Generate(IDictionary<string, object> parameters) ? string`

Convenience overload that returns the generated text as a `string`.

```csharp
string output = generator.Build(template).Generate(parameters);
```

---

## `T4Options`

Configuration for a `Generator` instance. Accessed via `Generator.Options`.

### Properties

| Property | Type | Default | Description |
|---|---|---|---|
| `ReferencePaths` | `IList<string>` | `[]` | Directories searched when resolving assembly references from `assembly` directives. |
| `ReferenceAssemblies` | `IList<string>` | `[]` | Additional assemblies to reference when compiling templates. Use this when your template uses types from assemblies not referenced by a directive in the template itself. |
| `SearchPaths` | `IList<string>` | `[]` | Directories searched when resolving `include` directives. |
| `Options` | `IDictionary<string, string>` | `{}` | CodeDom provider-specific compiler options. |
| `EndOfLine` | `EndOfLine` | `CRLF` | Line-ending character(s) used in the generated output. |
| `TemplateParameters` | `IDictionary<string, string>` | `{}` | Key/value pairs used to resolve `%MacroName%` placeholders in directives. |

### `EndOfLine` Enum

| Value | Characters | Description |
|---|---|---|
| `CRLF` | `\r\n` | Windows (default) |
| `LF` | `\n` | Unix / macOS / Linux |
| `CR` | `\r` | Legacy Mac OS |
| `LFCR` | `\n\r` | RISC OS |
| `NEL` | `U+0085` | Next Line (Unicode) |
| `LS` | `U+2028` | Line Separator (Unicode) |
| `PS` | `U+2029` | Paragraph Separator (Unicode) |

### Example

```csharp
var generator = new Generator();

// Reference a custom assembly used in the template
generator.Options.ReferenceAssemblies.Add(@"C:\libs\MyDomain.dll");

// Or reference by full name (useful for GAC assemblies)
generator.Options.ReferenceAssemblies.Add(
    Assembly.GetAssembly(typeof(MyDomainClass)).Location);

// Add a search path for include files
generator.Options.SearchPaths.Add(@"C:\T4\Includes");

// Use Unix line endings
generator.Options.EndOfLine = EndOfLine.LF;

// Define macro values used in template directives
generator.Options.TemplateParameters["Language"] = "C#";
generator.Options.TemplateParameters["Company"]  = "Acme";
```

---

## `T4CompilerException`

Thrown when the template code cannot be compiled. Inherits from `T4Exception`.

### Properties

| Property | Type | Description |
|---|---|---|
| `Errors` | `IList<Diagnostic>` | Roslyn diagnostics produced by the compiler. |
| `GeneratedCode` | `string` | The C# or VB source code that was generated from the template before compilation. |
| `CodeBehind` | `string` | The content of any code-behind file referenced in the `codefile` attribute of the `template` directive. |

### Example

```csharp
try
{
    generator.Generate(template, writer, parameters);
}
catch (T4CompilerException ex)
{
    Console.WriteLine("Compilation failed:");
    foreach (var error in ex.Errors)
        Console.WriteLine($"  {error}");

    Console.WriteLine("--- Generated Code ---");
    Console.WriteLine(ex.GeneratedCode);
}
```

---

## `IGenerator`

The interface implemented by `Generator`. Program against this interface to make your code testable and to allow alternative implementations.

```csharp
public class MyService
{
    private readonly IGenerator _generator;

    public MyService(IGenerator generator)
    {
        _generator = generator;
    }
}
```
