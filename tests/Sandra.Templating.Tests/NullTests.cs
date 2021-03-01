using System.Collections.Generic;
using System.Text.Json;
using FluentAssertions;
using Xunit;

namespace Sandra.Templating.Tests
{
    public class NullTests
    {
        [Fact]
        public void If_Property_Doesnt_Exist_Should_Not_Throw()
        {
            var jsonDictionary = @"{
    ""Exists"": true
}";

            var data = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonDictionary, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var template = "[if DoesNotExist]This should render[end if]Only Thing";

            var result = new TemplateEngine().Render(template, data);

            result.Should().Be("Only Thing");
        }

        [Fact]
        public void Iif_Property_Doesnt_Exist_Should_Not_Throw()
        {
            var jsonDictionary = @"{
    ""Exists"": true
}";

            var data = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonDictionary, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var template = "[iif DoesNotExist?'this':'that']";

            var result = new TemplateEngine().Render(template, data);

            result.Should().Be("that");
        }
    }
}