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
- IIf (ternary rendering based on a condition)
- Render
- Truncate (limiting text length)

## Can render: 

- string
- numbers (byte, short, int, long)
- DateTime

Supports basic formatting.

```csharp
// Data
private readonly IDictionary<string, object> data = new()
{
    ["FormattedDate"] = DateTime.Parse("2020-03-08 13:45:03.534")
};

// Template
[=FormattedDate:dd/MM/yyyy]
[=FormattedDate:dd:MM:yyyy]
[=FormattedDate:d MMM yyyy \a\t HH:mmtt]

// Output
08/03/2020
08:03:2020
8 Mar 2020 at 13:45pm
```

## IIf

Using the following in a template

```csharp
[iif variable = banana ? "A fruit is I" : "I is not a fruit :("]
```

Given
```csharp 
var data = new Dictionary<string, object>
{
    ["variable"] = "banana"
}
```

Would render:

> A fruit is I

## Truncate

You can limit the length of text using the truncate format:

```csharp
// Data
private readonly IDictionary<string, object> data = new()
{
    ["LongDescription"] = "This is a very long description that might need truncation in some contexts."
};

// Template
[=LongDescription:truncate(20)]
[=LongDescription:truncate(10)]

// Output
This is a very long
This is a 
```

The truncate format simply cuts the text at the specified length.
    
## Tested on:

- .NET 4.5.2
- .NET Core 2.2
- .NET Core 3.1
- .NET 6.0
- .NET 9.0
