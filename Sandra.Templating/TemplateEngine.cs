using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Sandra.Templating
{
    public class TemplateEngine
    {
        private static RegexOptions Options = RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline;
        private Regex IfConditionRegex = new Regex(@"(?s)\[if\s+(?<if>[^][]+)](?<content>(?>(?:(?!\[if\s|\[end\ if]).)+|(?<-open>)\[end\ if]|(?<open>)\[if\s+(?<if>[^][]+)])*(?(open)(?!)))\[end\ if]", Options);
        private Regex ForRegex = new Regex(@"(?s)\[for (?<name>[^][]+) in (?<variable>[^][]+)](?>(?:(?!\[for\s|\[end\ for]).)+|(?<close-open>)\[end\ for]|(?<open>)\[for\s+(?:[^][]+)])*(?(open)(?!))\[end\ for]", Options);
        private Regex RenderRegex = new Regex(@"(?:\[\=)(?<key>[a-zA-Z0-9]+)(?:\])", Options);

        private IList<Func<string, IDictionary<string, object>, string>> processors = new List<Func<string, IDictionary<string, object>, string>>(); 
        
        public TemplateEngine()
        {
            processors.Add(PerformIfConditionSubstitutions);
            processors.Add(PerformReplacementSubstitutions);
            processors.Add(PerformForLoopSubstitutions);
        }
        
        public string Render(string template, IDictionary<string, object> data)
        {
            var result = template;

            foreach (var processor in processors)
            {
                result = processor.Invoke(result, data);
            }

            return result;
        }

        private string PerformIfConditionSubstitutions(string template, IDictionary<string, object> data)
        {
            return IfConditionRegex.Replace(template, m =>
            {
                var key = m.Groups["if"].Captures[0].Value.ToLower();
                
                if (!data.ContainsKey(key))
                {
                    return string.Empty;
                }

                return Render(m.Groups["content"].Value, data);
            });
        }

        private string PerformReplacementSubstitutions(string template, IDictionary<string, object> data)
        {
            return RenderRegex.Replace(template, m =>
            {
                var key = m.Groups["key"].Captures[0].Value;
                
                if (!data.ContainsKey(key.ToLower()))
                {
                    return string.Empty;
                }

                return data[key.ToLower()].ToString();
            });
        }

        private string PerformForLoopSubstitutions(string template, IDictionary<string, object> data)
        {
            return ForRegex.Replace(template, m =>
            {
                var key = m.Groups["variable"].Captures[0].Value;
                var name = m.Groups["name"].Captures[0].Value;
                
                if (!data.ContainsKey(key.ToLower()))
                {
                    return string.Empty;
                }

                if (!(data[key] is IList<IDictionary<string, object>> items))
                {
                    return $"(ERROR: {key} is null or not a `IList<IDictionary<string, object>>`)";
                }

                var startIndex = $"[for {name} in {key}]".Length;
                var endIndex = m.Value.Length - "[end for]".Length - startIndex;

                var content = m.Value.Substring(startIndex, endIndex).Replace($"[={name}.", "[=");

                var sb = new StringBuilder();
                
                foreach (var item in items)
                {
                    sb.AppendLine(Render(content, item));
                }

                return sb.ToString();
            });
        }
    }
}