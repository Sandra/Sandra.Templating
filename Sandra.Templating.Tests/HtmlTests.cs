using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Sandra.Templating.Tests
{
    public class HtmlTests
    {
        private const string HtmlPath = "./Templates/HTMLTests/";
        private readonly TemplateEngine engine;

        private readonly IDictionary<string, object> data = new Dictionary<string, object>
        {
            ["peeps"] = new List<IDictionary<string, object>>
            {
                new Dictionary<string, object>
                {
                    ["name"] = "Phillip"
                },
                new Dictionary<string, object>
                {
                    ["name"] = "Demi"
                },
                new Dictionary<string, object>
                {
                    ["name"] = "Richard"
                },
                new Dictionary<string, object>
                {
                    ["name"] = "Maggie"
                }
            }
        };

        public HtmlTests()
        {
            engine = new TemplateEngine();
        }

        [Theory]
        [InlineData("Table_Sample.html", "Table_Sample.output.html")]
        public void HtmlTemplateTests(string input, string output)
        {
            var template = File.ReadAllText($"{HtmlPath}{input}");
            var expected = File.ReadAllText($"{HtmlPath}{output}");

            var actual = engine.Render(template, data);

            Assert.Equal(expected, actual, ignoreLineEndingDifferences: true);
        }
    }
}