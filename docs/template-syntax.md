# Template Syntax

A T4 template is a plain text file (typically with a `.t4` extension) that mixes static text with special markup tags. Everything outside markup tags is emitted verbatim to the output. Markup tags contain directives or code that control what is generated.

---

## Directives

Directives configure the template and the generator. They are enclosed in `<#@ ... #>`.

### `template` Directive

Must be the **first** line in every template file.

```
<#@ template language="C#" #>
```

| Attribute | Description |
|---|---|
| `language` | Scripting language: `C#` or `VB`. Supports the `%MacroName%` syntax. |
| `classname` | Optional fully-qualified class name for the generated generator class. Supports macros. |
| `codefile` | Optional path to a C# or VB code-behind file that is compiled together with the template. Supports macros. |

---

### `import` Directive

Equivalent to a `using` statement (C#) or `Imports` (VB). Multiple directives are allowed.

```
<#@ import namespace="System.Collections.Generic" #>
<#@ import namespace="System.Linq" #>
```

---

### `assembly` Directive

References an assembly that the generated template code should be compiled against. Equivalent to adding a reference in a project file.

```
<#@ assembly name="System.Core.dll" #>
```

Use full assembly paths or assembly names resolvable from the paths listed in `T4Options.ReferencePaths`.

---

### `parameter` Directive

Declares a typed parameter that is passed into the template at generation time.

```
<#@ parameter type="string" name="firstName" #>
<#@ parameter type="IList<string>" name="items" #>
```

Parameters are passed as a `IDictionary<string, object>` to `Generate` / `GenerateToFile`. The dictionary key must match the `name` attribute.

```csharp
var parameters = new Dictionary<string, object>
{
    { "firstName", "Alice" },
    { "items", new List<string> { "one", "two", "three" } }
};

generator.Generate(template, writer, parameters);
```

---

### `include` Directive

Inserts the contents of another template file at the location of the directive.

```
<#@ include file="SharedHeader.t4" #>
```

The file is resolved relative to the including template. Additional search paths can be configured via `T4Options.SearchPaths`.

---

## Control Blocks

### Standard Control Block `<# ... #>`

Contains any valid C# or VB statements. Used for logic such as loops and conditionals.

```
<#@ template language="C#" #>
<#@ parameter type="IList<string>" name="items" #>
<#@ import namespace="System.Collections.Generic" #>
<#
    foreach (var item in items)
    {
#>
Item: <#= item #>
<#
    }
#>
```

You can also call the built-in `Output.Write()` method to emit text programmatically:

```
<#@ template language="C#" #>
<#
    for (int i = 0; i < 5; i++)
    {
        Output.Write(i);
    }
#>
```

---

### Expression Control Block `<#= ... #>`

Evaluates a C# or VB expression and inserts the result into the output.

```
<#@ template language="C#" #>
<#@ parameter type="string" name="name" #>
Hello <#= name #>!
```

---

### Feature Control Block `<#+ ... #>`

Declares helper methods, properties, or nested classes that can be called from standard or expression control blocks. Feature blocks are typically placed at the end of the template.

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

---

### Comment Block `<#-- ... #>`

Comments are ignored by the parser and do not appear in the output.

```
<#-- This is a comment and will not appear in the output #>
```

---

## Escaping Markup Tags

To emit a literal `<#` or `#>` sequence in the output, prefix each tag delimiter with a backslash:

```
<#@ template language="C#" #>
\<#@ template language="C#" \#>
\<# int i = 3; \#>
TEXT \<#= i.ToString() \#>
```

Output:
```
<#@ template language="C#" #>
<# int i = 3; #>
TEXT <#= i.ToString() #>
```

---

## Macro Substitution in Directives

Attribute values in directives can contain macros of the form `%MacroName%`. Macros are resolved at generation time from `T4Options.TemplateParameters` and then from environment variables.

This is useful for parameterising the template language or class names without changing the template file:

```
<#@ template language="%Language%" #>
<#@ template language="C#" classname="%Company%.%Module%.MyGenerator" codefile="%Module%Generator.t4.cs" #>
```

```csharp
generator.Options.TemplateParameters["Language"] = "C#";
generator.Options.TemplateParameters["Company"]  = "Acme";
generator.Options.TemplateParameters["Module"]   = "Reports";
```
