using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Sandra.Templating.Tests
{
    public class IIfTests
    {
        private const string Path = "./Templates/IIf/";
        private readonly TemplateEngine engine;
        private readonly IDictionary<string, object> data = new Dictionary<string, object>
        {
            ["interval"] = "d"
        };

        public IIfTests()
        {
            engine = new TemplateEngine();
        }
        
        [Theory]
        [InlineData("Single-True.txt", "Single-True.output.txt")]
        [InlineData("Single-False.txt", "Single-False.output.txt")]
        public void TemplateTests(string input, string output)
        {
            var template = File.ReadAllText($"{Path}{input}");
            var expected = File.ReadAllText($"{Path}{output}");

            var actual = engine.Render(template, data);

            Assert.Equal(expected, actual, ignoreLineEndingDifferences: true);
        }
    }
}