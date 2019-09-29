[![Build status](https://ci.appveyor.com/api/projects/status/iygj517aax6rdsbh/branch/master?svg=true)](https://ci.appveyor.com/project/phillip-haydon/sandra-templating/branch/master)

# Sandra.Templating

Sandra.Templating, is yet another templating engine, because we always need more.

This is meant to be a dead-simple-basic functionality templating engine that can be used for Email Templates, by providing only the bare minimum necessary to populate data into a template.

# Usage

```csharp
var template = @"
Hello [=name]

[if fruit]
    I like [=fruit]
[end if]
";

var data = new Dictionary<string, object>
{
    ["name"] = "World",
    ["fruit"] = "Bananas"
};

var engine = new TemplateEngine();

var result = engine.Render(template, data);
``` 

# Supported Features

- For Each (nested supported)
- If (nested supported)
- Render
