using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Sandra.Templating.Tests
{
    public class IfTests
    {
        private const string Path = "./Templates/If/";
        private readonly TemplateEngine engine;
        private readonly IDictionary<string, object> data = new Dictionary<string, object>
        {
            ["variable1"] = "Variable Value One",
            ["variable2"] = "Variable Value Two",
            ["variable3"] = "Variable Value Three",
            ["variable4"] = "Variable Value Four",
            ["variable5"] = "Variable Value Five",
            ["variable6"] = "Variable Value Six",
            ["variable7"] = "Variable Value Seven",
            ["variable8"] = "Variable Value Eight",
            ["variable9"] = "Variable Value Nine",
            ["variable0"] = "Variable Value Ten",
            ["UPPER_case"] = "Variable name and template name dont match but test should pass",
            ["Banana"] = "Yellow",
            ["BooleanConditionTrue"] = true,
            ["BooleanConditionFalse"] = false
        };

        public IfTests()
        {
            engine = new TemplateEngine();
        }
        
        [Theory]
        [InlineData("Single.txt", "Single.output.txt")]
        [InlineData("Single_No_Match.txt", "Single_No_Match.output.txt")]
        [InlineData("Multiple.txt", "Multiple.output.txt")]
        [InlineData("Nested.txt", "Nested.output.txt")]
        [InlineData("Nested_Multiple.txt", "Nested_Multiple.output.txt")]
        [InlineData("Single_Casing.txt", "Single_Casing.output.txt")]
        [InlineData("Conditional.txt", "Conditional.output.txt")]
        [InlineData("BoolConditional.txt", "BoolConditional.output.txt")]
        public void TemplateTests(string input, string output)
        {
            var template = File.ReadAllText($"{Path}{input}");
            var expected = File.ReadAllText($"{Path}{output}");

            var actual = engine.Render(template, data);

            Assert.Equal(expected, actual, ignoreLineEndingDifferences: true);
        }
    }
}