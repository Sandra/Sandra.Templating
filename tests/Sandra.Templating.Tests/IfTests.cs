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
            ["BooleanConditionFalse"] = false,
            ["empty_collection"] = new string[] {},
            ["non_empty_collection"] = new string[] {"Item"},
            ["null_variable"] = null!
        };

        public IfTests()
        {
            engine = new TemplateEngine();
        }
        
        [Theory]
        [InlineData("Single")]
        [InlineData("Single_No_Match")]
        [InlineData("Multiple")]
        [InlineData("Nested")]
        [InlineData("Nested_Multiple")]
        [InlineData("NullValue")]
        [InlineData("Single_Casing")]
        [InlineData("Conditional")]
        [InlineData("BoolConditional")]
        [InlineData("Value_Is_Collection_With_No_Items")]
        public void TemplateTests(string filename)
        {
            var template = File.ReadAllText($"{Path}{filename}.txt");
            var expected = File.ReadAllText($"{Path}{filename}.output.txt");

            var actual = engine.Render(template, data);

            Assert.Equal(expected, actual, ignoreLineEndingDifferences: true);
        }
    }
}