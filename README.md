# Sandra.Templating

Sandra.Templating, is yet another templating engine, because we always need more.

This is meant to be a dead-simple-basic functionality templating engine that can be used for Email Templates, by providing only the bare minimum necessary to populate data into a template.

This isn't meant to be super fast, compile, or support complex scenarios. 

Pull requests are welcome. :)

# Usage

```csharp
var template = @"
Hello [=name]

[if fruit]
    I like [=fruit]
[end if]

Shop Name: [=shop.Name]
Location: [=shop.Location]
";

var data = new Dictionary<string, object>
{
    ["name"] = "World",
    ["fruit"] = "Bananas",
    ["shop"] = new Shop
    {
        Name = "Fruit Shop",
        Location = "Singapore"
    }
};

var engine = new TemplateEngine();

var result = engine.Render(template, data);
``` 

## Supported Features

- For Each (nested supported)
- If (nested supported)
- Render

## Can render: 

- string
- numbers (byte, short, int, long)
- DateTime

Supports basic formatting.

```csharp
[=FormattedDate:dd/MM/yyyy]
```

## Tested on:

- .NET 4.5.2
- .NET Core 2.2
- .NET Core 3.1
