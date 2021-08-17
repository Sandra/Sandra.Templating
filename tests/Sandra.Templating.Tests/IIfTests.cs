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
            ["interval"] = "d",
            ["IsEnabled"] = true,
            ["Colour"] = "#ffffff",
            ["IsDisabled"] = false
        };

        public IIfTests()
        {
            engine = new TemplateEngine();
        }
        
        [Theory]
        [InlineData("BooleanCheck")]
        [InlineData("Check_With_Variable")]
        [InlineData("Single-True")]
        [InlineData("Single-False")]
        [InlineData("Single-True-Single-Quotes")]
        [InlineData("Single-True-Mix-Quotes")]
        [InlineData("Multiple-Usage-Mix-Quotes")]
        public void TemplateTests(string filename)
        {
            var template = File.ReadAllText($"{Path}{filename}.txt");
            var expected = File.ReadAllText($"{Path}{filename}.output.txt");

            var actual = engine.Render(template, data);

            Assert.Equal(expected, actual, ignoreLineEndingDifferences: true);
        }
        
        [Fact]
        public void TestScenarios()
        {
            var engine = new TemplateEngine();

            var template = "[iif IsReminder?'Reminder: ':''][iif IsUploader?'[=RenderUploaderSubject]':''][iif IsEditor?'[=RenderEditorSubject]':'']";

            var data = new Dictionary<string, object>
            {
                ["IsReminder"] = false,
                ["IsEditor"] = true,
                ["IsUploader"] = false,
                ["RenderUploaderSubject"] = "Hello [=Name] you're an Uploader",
                ["RenderEditorSubject"] = "Hello [=Name] you're an Editor",
                ["Name"] = "Phillip",
            };

            var result = engine.Render(template, data);

            Assert.Equal("Hello Phillip you're an Editor", result);
        }
    }
}