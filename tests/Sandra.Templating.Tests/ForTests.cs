using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Sandra.Templating.Tests
{
    public class ForTests
    {
        private const string TextPath = "./Templates/For/";
        private readonly TemplateEngine engine;

        private readonly IDictionary<string, object> data = new Dictionary<string, object>
        {
            ["empty_list"] = new List<IDictionary<string, object>>(),
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
                }
            },
            ["wrong_type"] = "not a list"
        };

        public ForTests()
        {
            engine = new TemplateEngine();
        }

        [Theory]
        [InlineData("Empty.txt", "Empty.output.txt")]
        [InlineData("5_Items.txt", "5_Items.output.txt")]
        [InlineData("No_Key.txt", "No_Key.output.txt")]
        [InlineData("Wrong_Type.txt", "Wrong_Type.output.txt")]
        public void TemplateTests(string input, string output)
        {
            var template = File.ReadAllText($"{TextPath}{input}");
            var expected = File.ReadAllText($"{TextPath}{output}");

            var actual = engine.Render(template, data);

            Assert.Equal(expected, actual, ignoreLineEndingDifferences: true);
        }
    }
}