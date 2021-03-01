using System.Collections.Generic;
using System.Text.Json;
using FluentAssertions;
using Xunit;

namespace Sandra.Templating.Tests.Templates
{
    public class SystemTextJsonBugTests
    {
        [Fact]
        public void Deserialized_Json_Should_Allow_Boolean_Checks_For_Iif()
        {
            var jsonDictionary = @"{
    ""BooleanTrue"": true
}";

            var data = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonDictionary, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var template = "[iif BooleanTrue?'Hello World':'fail']";

            var result = new TemplateEngine().Render(template, data);

            result.Should().Be("Hello World");
        }

        [Fact]
        public void Deserialized_Json_Should_Allow_Boolean_Checks_For_If()
        {
            var jsonDictionary = @"{
    ""BooleanTrue"": true
}";

            var data = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonDictionary, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var template = "[if BooleanTrue]This should render[end if]";

            var result = new TemplateEngine().Render(template, data);

            result.Should().Be("This should render");
        }
    }
}