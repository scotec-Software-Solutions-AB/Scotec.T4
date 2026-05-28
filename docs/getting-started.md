# Getting Started

## Installation

Install the NuGet package in your project:

```bash
dotnet add package Scotec.T4
```

Or via the NuGet Package Manager in Visual Studio by searching for **Scotec.T4**.

---

## Your First Template

### Step 1 — Create a template file

Create a file named `HelloWorld.t4`. The file extension `.t4` is recommended.

> **Important:** Do _not_ create the file using Visual Studio's built-in **Text Template** item template, because that assigns `TextTemplatingFileGenerator` to the `CustomTool` property and triggers design-time generation.

```
<#@ template language="C#" #>
Hello World!
```

The `template` directive must be the first line of every template file. The `language` attribute specifies the scripting language (`C#` or `VB`).

If you add the template file to a Visual Studio project, set **Copy to Output Directory** to *Copy if newer* or *Copy always* so the file is available at runtime.

---

### Step 2 — Generate output from a file

```csharp
using Scotec.T4;

var generator = new Generator();
var template  = T4Template.FromFile("HelloWorld.t4");

// Write to a TextWriter
using var stream = new MemoryStream();
using var writer = new StreamWriter(stream);
generator.Generate(template, writer, null);

stream.Seek(0, SeekOrigin.Begin);
using var reader = new StreamReader(stream);
Console.WriteLine(reader.ReadToEnd()); // Hello World!
```

Alternatively, use the `TextGenerator.Generate` overload that returns a string directly:

```csharp
var generator = new Generator();
var template  = T4Template.FromFile("HelloWorld.t4");

string result = generator.Build(template).Generate(null);
Console.WriteLine(result); // Hello World!
```

---

### Step 3 — Generate output from a string

Templates do not have to come from the file system. You can supply the template content as a plain string:

```csharp
const string templateText = "<#@ template language=\"C#\" #>\r\nHello World!";

var generator = new Generator();
var template  = T4Template.FromString(templateText, "MyTemplate");

string result = generator.Build(template).Generate(null);
Console.WriteLine(result); // Hello World!
```

`T4Template.FromString` requires a `name` argument. The name is used internally to identify the compiled generator class.

---

### Step 4 — Generate output to a file

Use `GenerateToFile` when you want to write the result directly to disk:

```csharp
var generator = new Generator();
var template  = T4Template.FromFile("HelloWorld.t4");

generator.GenerateToFile(template, @"C:\Output\hello.txt", null);
```

An overload that accepts an `Encoding` is also available:

```csharp
generator.GenerateToFile(template, @"C:\Output\hello.txt", Encoding.UTF8, null);
```

The output directory is created automatically if it does not exist.
