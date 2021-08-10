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
                new(true, "Phillip"),
                new(false, "Nigel"),
                new(true, "Steven")
            },
            ["LabelName"] = "Name: ",
            ["TopLevel"] = new List<Parent>
            {
                new ("Level 1a", new List<Child>
                {
                    new (1, "Level 2a"),
                    new (2, "Level 2b")
                }),
                new ("Level 1b", new List<Child>
                {
                    new (1, "Level 2a"),
                    new (2, "Level 2b")
                })
            }
        };

        private record Parent(string Name, List<Child> SecondLevel);
        private record Child(int Id, string Name);
        private record TestClass(bool ShowName, string Name);

        public ForTests()
        {
            engine = new TemplateEngine();
        }

        [Theory]
        [InlineData("3_Items_With_Parent_Reference")]
        [InlineData("5_Items")]
        [InlineData("5_Items_array")]
        [InlineData("Empty")]
        [InlineData("Nested_1_x_5")]
        [InlineData("Nested_With_Condition")]
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