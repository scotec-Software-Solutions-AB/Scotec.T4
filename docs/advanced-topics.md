# Advanced Topics

## Passing Parameters to Templates

Parameters are passed as an `IDictionary<string, object>`. Each key must match the `name` attribute of a `<#@ parameter #>` directive in the template.

### Template

```
<#@ template language="C#" #>
<#@ import namespace="System.Collections.Generic" #>
<#@ import namespace="System.Linq" #>
<#@ parameter type="IList<string>" name="friends" #>
<#
    foreach (var friend in SortFriends(friends))
    {
#>
Hello <#= friend #>!
<#
    }
#>
<#+
private IEnumerable<string> SortFriends(IEnumerable<string> friends)
{
    return from f in friends orderby f ascending select f;
}
#>
```

### C# Code

```csharp
var generator = new Generator();
var template  = T4Template.FromFile("Greetings.t4");

var parameters = new Dictionary<string, object>
{
    { "friends", new List<string> { "Mary", "Peter", "Paul" } }
};

string output = generator.Build(template).Generate(parameters);
// Hello Mary!
// Hello Paul!
// Hello Peter!
```

### Referencing Custom Types

If a parameter type is defined in your own assembly, register that assembly with the generator before the first `Generate` call:

```csharp
generator.Options.ReferenceAssemblies.Add(
    Assembly.GetAssembly(typeof(MyModel)).Location);
```

---

## Including Other Templates

The `include` directive merges the content of another template file at the directive's position.

`Report.t4`:
```
<#@ template language="C#" #>
<#@ include file="Header.t4" #>
Report body text.
<#@ include file="Footer.t4" #>
```

`Header.t4`:
```
=== Report Header ===
```

By default the file is resolved relative to the including template. Add directories to `T4Options.SearchPaths` to make include files discoverable from a central location:

```csharp
generator.Options.SearchPaths.Add(@"C:\T4\Shared");
```

---

## Macro Substitution

Macros allow you to inject values into directive attributes without modifying the template file. A macro has the form `%MacroName%`.

```
<#@ template language="%Language%" #>
<#@ assembly name="%AssemblyPath%" #>
<#@ template language="C#" classname="%Company%.%Module%.MyGen" codefile="%Module%Gen.t4.cs" #>
```

Macros are resolved from `T4Options.TemplateParameters` first, then from **environment variables** as a fallback.

```csharp
generator.Options.TemplateParameters["Language"]     = "C#";
generator.Options.TemplateParameters["AssemblyPath"] = @"C:\libs\Domain.dll";
generator.Options.TemplateParameters["Company"]      = "Acme";
generator.Options.TemplateParameters["Module"]       = "Reports";
```

---

## Configuring Line Endings

The `EndOfLine` option controls which line-break characters are used in the output, independently of the platform the application runs on.

```csharp
// Unix-style line endings
generator.Options.EndOfLine = EndOfLine.LF;

// Windows-style line endings (default)
generator.Options.EndOfLine = EndOfLine.CRLF;
```

See the `EndOfLine` enum in the [API Reference](api-reference.md) for all supported values.

---

## Precompiling Templates

When processing a large number of templates, parsing and compiling each one on first use can be time-consuming. `Generator.Compile` precompiles a list of templates in parallel background threads. Subsequent `Generate` calls for those templates use the cached compiled assembly.

```csharp
var generator = new Generator();

var templates = Directory.GetFiles(@"C:\T4\Templates", "*.t4")
                         .Select(T4Template.FromFile)
                         .ToList();

// Start precompilation in the background — you do not have to await immediately
var compileTask = generator.Compile(templates);

// Other application startup work can happen here...
await compileTask; // wait before generation if desired

Parallel.ForEach(templates, template =>
{
    string output = generator.Build(template).Generate(myParameters);
    File.WriteAllText(Path.Combine(@"C:\Output", template.Name + ".txt"), output);
});
```

> **Note:** Reuse the same `Generator` instance across all generation calls. Each compiled template assembly is cached inside the generator. Creating a new `Generator` per template discards the cache.

---

## High-Load / Parallel Generation

`Generator` is safe to use from multiple threads simultaneously once templates are compiled. Use `Parallel.ForEach` or `Task.WhenAll` for bulk generation:

```csharp
var generator = new Generator();

var templates = Directory.GetFiles(templateDir, "*.t4")
                         .Select(T4Template.FromFile)
                         .ToList();

// Optional: precompile all templates up front
await generator.Compile(templates);

Parallel.ForEach(templates, template =>
{
    using var stream = new MemoryStream();
    using var writer = new StreamWriter(stream);
    generator.Generate(T4Template.FromFile(template.File), writer, parameters);
    // process output ...
});
```

---

## Code-Behind Files

A `codefile` attribute on the `template` directive instructs the generator to compile an additional C# or VB source file together with the template. This is useful for sharing helper classes across multiple templates.

`MyTemplate.t4`:
```
<#@ template language="C#" classname="MyGen" codefile="MyHelpers.t4.cs" #>
<#= FormatDate(DateTime.Now) #>
```

`MyHelpers.t4.cs`:
```csharp
public partial class MyGen
{
    private string FormatDate(DateTime dt) => dt.ToString("yyyy-MM-dd");
}
```

The `classname` attribute and `codefile` attribute both support `%MacroName%` substitution:

```
<#@ template language="C#" classname="%Company%.%Module%.MyGen" codefile="%Module%Helpers.t4.cs" #>
```

---

## Error Handling

Catch `T4CompilerException` to access detailed compiler diagnostics when a template contains syntax errors. Catch `T4Exception` for all other generator errors.

```csharp
try
{
    generator.Generate(template, writer, parameters);
}
catch (T4CompilerException ex)
{
    // ex.Errors contains Roslyn Diagnostic objects
    foreach (var diagnostic in ex.Errors)
        Console.Error.WriteLine(diagnostic.ToString());

    // ex.GeneratedCode is the C#/VB source produced from the template
    Console.Error.WriteLine(ex.GeneratedCode);
}
catch (T4Exception ex)
{
    Console.Error.WriteLine($"Generator error: {ex.Message}");
}
```

The `Diagnostic` objects from `ex.Errors` include file, line number, and message information, making it straightforward to display precise error feedback to users.
