using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Sandra.Templating.Tests
{
    public class RenderTests
    {
        public class Person
        {
            public string Name { get; set; } = string.Empty;

            public int Age { get; set; }
        }
        
        private const string Path = "./Templates/Render/";
        private readonly TemplateEngine engine;
        private readonly IDictionary<string, object> data = new Dictionary<string, object>
        {
            ["fruit"] = "Banana",
            ["person"] = new Person
            {
                Name = "Phillip",
                Age = 33
            },
            ["a"] = (byte)100,
            ["b"] = (short)1_000,
            ["c"] = (int)100_000,
            ["d"] = (long)10_000_000_000
        };

        public RenderTests()
        {
            engine = new TemplateEngine();
        }
        
        [Theory]
        [InlineData("Single_Match.txt", "Single_Match.output.txt")]
        [InlineData("Single_Class_Match.txt", "Single_Class_Match.output.txt")]
        [InlineData("Types.txt", "Types.output.txt")]
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
        
        [Theory]
        [InlineData("Single_Match.txt", "Single_Match.output.txt")]
        [InlineData("Single_Class_Match.txt", "Single_Class_Match.output.txt")]
//        [InlineData("Single_No_Match.txt", "Single_No_Match.output.txt")]
//        [InlineData("Multiple.txt", "Multiple.output.txt")]
//        [InlineData("Nested.txt", "Nested.output.txt")]
//        [InlineData("Nested_Multiple.txt", "Nested_Multiple.output.txt")]
        public void TemplateTests_100k(string input, string output)
        {
            var template = File.ReadAllText($"{Path}{input}");
            var expected = File.ReadAllText($"{Path}{output}");

            for (int i = 0; i < 100_000; i++)
            {
                engine.Render(template, data);
            }
        }
    }
}