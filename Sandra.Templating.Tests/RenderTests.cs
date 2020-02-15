using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Sandra.Templating.Tests
{
    public class RenderTests
    {
        private const string Path = "./Templates/Render/";
        private readonly TemplateEngine engine;
        private readonly IDictionary<string, object> data = new Dictionary<string, object>
        {
            ["fruit"] = "Banana"
        };

        public RenderTests()
        {
            engine = new TemplateEngine();
        }
        
        [Theory]
        [InlineData("Single_Match.txt", "Single_Match.output.txt")]
//        [InlineData("Single_No_Match.txt", "Single_No_Match.output.txt")]
//        [InlineData("Multiple.txt", "Multiple.output.txt")]
//        [InlineData("Nested.txt", "Nested.output.txt")]
//        [InlineData("Nested_Multiple.txt", "Nested_Multiple.output.txt")]
        public void TemplateTests(string input, string output)
        {
            var template = File.ReadAllText($"{Path}{input}");
            var expected = File.ReadAllText($"{Path}{output}");

            var actual = engine.Render(template, data);

            Assert.Equal(expected, actual, ignoreLineEndingDifferences: true);
        }
    }
}