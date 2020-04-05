using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Sandra.Templating.Tests
{
    public class SplitTests
    {
        private const string TextPath = "./Templates/Split/";
        private readonly TemplateEngine engine;

        private readonly IDictionary<string, object> data = new Dictionary<string, object>
        {
            ["small_list"] = new List<IDictionary<string, object>>
            {
                new Dictionary<string, object>
                {
                    ["name"] = "Banana"
                },
                new Dictionary<string, object>
                {
                    ["name"] = "Apple"
                },
                new Dictionary<string, object>
                {
                    ["name"] = "Grape"
                },
                new Dictionary<string, object>
                {
                    ["name"] = "Watermelon"
                },
                new Dictionary<string, object>
                {
                    ["name"] = "Kiwifruit"
                },
                new Dictionary<string, object>
                {
                    ["name"] = "Peach"
                }
            },
            ["object_list"] = new List<dynamic>
            {
                new { Name = "Bob" },
                new { Name = "Fred" },
                new { Name = "James" },
                new { Name = "John" },
                new { Name = "Dan" },
                new { Name = "Peter" },
                new { Name = "Zack" },
                new { Name = "Sam" }
            }
        };

        public SplitTests()
        {
            engine = new TemplateEngine();
        }

        [Theory]
        [InlineData("6_Items.txt", "6_Items.output.txt")]
        [InlineData("8_Objects_List.txt", "8_Objects_List.output.txt")]
        public void TemplateTests(string input, string output)
        {
            var template = File.ReadAllText($"{TextPath}{input}");
            var expected = File.ReadAllText($"{TextPath}{output}");

            var actual = engine.Render(template, data);

            Assert.Equal(expected, actual, ignoreLineEndingDifferences: true);
        }
    }
}