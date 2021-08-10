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
            ["wrong_type"] = "not a list",
            ["array_list"] = new[]
            {
                "One",
                "Two",
                "Three",
                "Four",
                "Five"
            },
            ["Names"] = new List<TestClass>
            {
                new()
                {
                    Name = "Phillip",
                    ShowName = true
                },
                new()
                {
                    Name = "Nigel",
                    ShowName = false
                },
                new()
                {
                    Name = "Steven",
                    ShowName = true
                }
            }
        };

        private class TestClass
        {
            public bool ShowName { get; set; }

            public string Name { get; set; }
        }

        public ForTests()
        {
            engine = new TemplateEngine();
        }

        [Theory]
        [InlineData("Nested_With_Condition")]
        [InlineData("Empty")]
        [InlineData("5_Items")]
        [InlineData("5_Items_array")]
        [InlineData("No_Key")]
        [InlineData("Wrong_Type")]
        public void TemplateTests(string filename)
        {
            var template = File.ReadAllText($"{TextPath}{filename}.txt");
            var expected = File.ReadAllText($"{TextPath}{filename}.output.txt");

            var actual = engine.Render(template, data);

            Assert.Equal(expected, actual, ignoreLineEndingDifferences: true);
        }
    }
}