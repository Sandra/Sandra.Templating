﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Sandra.Templating
{
    public class TemplateEngine
    {
        private const RegexOptions Options = RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.CultureInvariant;

        private static readonly Regex IfConditionRegex = new(@"(?s)\[if\s+(?<if>[^][]+)](?<content>(?>(?:(?!\[if\s|\[end\ if]).)+|(?<-open>)\[end\ if]|(?<open>)\[if\s+(?<if>[^][]+)])*(?(open)(?!)))\[end\ if]", Options);
        private static readonly Regex ForRegex = new(@"(?s)\[for (?<n>[^][]+) in (?<variable>[^][]+)](?>(?:(?!\[for\s|\[end\ for]).)+|(?<close-open>)\[end\ for]|(?<open>)\[for\s+(?:[^][]+)])*(?(open)(?!))\[end\ for]", Options);
        private static readonly Regex RenderRegex = new(@"(?:\[\=)(?<key>[a-zA-Z0-9\.]+)(?:\:(?<format>[a-zA-Z-0-9\\\/\-_\.\: \(\)]+))?(?:\])", Options);
        private static readonly Regex ForSplit = new(@"(?s)\[split\=(?<mod>\d+)](?<value>(?>(?:(?!\[split\s|\[split\ end]).)+|(?<-open>)\[split\ end]|(?<open>)\[split\=(?<mod>\d+)])*(?(open)(?!)))\[split\ end]", Options);
        private static readonly Regex RenderTernaryRegex = new(@"(?:\[iif[ ]*(?<variable>[a-zA-Z0-9_]+)[ =]*(?<value>[a-zA-Z0-9]*)[ \?]*(?<fq>['""]{1})(?<true_variable>(?:(?!\k<fq>).)+)\k<fq>[ :]+(?<sq>['""]{1})(?<false_variable>(?:(?!\k<sq>).)*)\k<sq>[ ]*\])", Options);
        private static readonly Regex TruncateRegex = new(@"truncate\((?<length>\d+)\)", Options);

        private readonly IList<Func<string, IDictionary<string, object>, bool, string>> processors = new List<Func<string, IDictionary<string, object>, bool, string>>();

        public TemplateEngine()
        {
            processors.Add(PerformForLoopSubstitutions);
            processors.Add(PerformTernarySubstitutions);
            processors.Add(PerformIfConditionSubstitutions);
            processors.Add(PerformReplacementSubstitutions);

            // TODO: Fix this hack with proper check for matches and replacing so it can infinitely replace
            processors.Add(PerformReplacementSubstitutions);
        }

        public string Render(string template, IDictionary<string, object> data, bool preserveContent = false)
        {
            var result = template;

            foreach (var processor in processors)
            {
                result = processor.Invoke(result, data, preserveContent);
            }

            return result;
        }

        private string PerformTernarySubstitutions(string template, IDictionary<string, object> data, bool preserveContent = false)
        {
            return RenderTernaryRegex.Replace(template, m =>
            {
                var key = m.Groups["variable"].Value;
                var value = m.Groups["value"]?.Value;

                var rawValue = data.FirstOrDefault(x => x.Key.ToLower().Equals(key.ToLower()));

                if (string.IsNullOrEmpty(value) && rawValue.Value != null && (rawValue.Value is bool boolValue || bool.TryParse(rawValue.Value.ToString(), out boolValue)))
                {
                    return boolValue ? m.Groups["true_variable"].Value : m.Groups["false_variable"].Value;
                }

                return rawValue.Value != null && rawValue.Value.Equals(value) ? m.Groups["true_variable"].Value : m.Groups["false_variable"].Value;
            });
        }

        private string PerformIfConditionSubstitutions(string template, IDictionary<string, object> data, bool preserveContent = false)
        {
            return IfConditionRegex.Replace(template, m =>
            {
                var content = m.Groups["if"].Captures[0].Value;

                if (content.Contains('='))
                {
                    var split = content.Split('=');
                    var rawValue = data.FirstOrDefault(x => x.Key.ToLower().Equals(split[0].Trim().ToLower()));

                    if (string.IsNullOrEmpty(rawValue.Key))
                    {
                        return string.Empty;
                    }

                    if (split[1].Trim().ToLower() != rawValue.Value.ToString().ToLower())
                    {
                        return string.Empty;
                    }

                    return Render(m.Groups["content"].Value, data, preserveContent);
                }
                else
                {
                    var rawValue = data.FirstOrDefault(x => x.Key.ToLower().Equals(content.ToLower()));

                    if (rawValue.Value == null)
                    {
                        return string.Empty;
                    }

                    if (string.IsNullOrEmpty(rawValue.Key))
                    {
                        return string.Empty;
                    }

                    if (rawValue.Value is ICollection { Count: 0 })
                    {
                        return string.Empty;
                    }

                    if ((rawValue.Value is bool boolValue || bool.TryParse(rawValue.Value.ToString(), out boolValue)) && !boolValue)
                    {
                        return string.Empty;
                    }

                    return Render(m.Groups["content"].Value, data, preserveContent);
                }
            });
        }

        private string PerformReplacementSubstitutions(string template, IDictionary<string, object> data, bool preserveContent = false)
        {
            return RenderRegex.Replace(template, m =>
            {
                var key = m.Groups["key"].Captures[0].Value;
                var formatStr = m.Groups["format"].Success ? m.Groups["format"].Value : string.Empty;
                var keySplit = key.Split('.');
                var keyPrefix = keySplit.First();

                var rawValue = data.FirstOrDefault(x => x.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase));
                object valueObj = null;

                if (!string.IsNullOrEmpty(rawValue.Key))
                {
                    valueObj = rawValue.Value;
                }
                else
                {
                    rawValue = data.FirstOrDefault(x => x.Key.Equals(keyPrefix, StringComparison.InvariantCultureIgnoreCase));

                    if (!string.IsNullOrEmpty(rawValue.Key))
                    {
                        // Assume we need to take the value of a property
                        if (keySplit.Length > 1)
                        {
                            valueObj = rawValue.Value?.GetType().GetProperty(keySplit.Last())?.GetValue(rawValue.Value);
                        }
                        else
                        {
                            valueObj = rawValue.Value;
                        }
                    }
                }

                if (valueObj == null)
                {
                    return preserveContent ? m.Value : string.Empty;
                }

                // Check if it's a truncate format
                var truncateMatch = TruncateRegex.Match(formatStr);
                if (truncateMatch.Success)
                {
                    int length = int.Parse(truncateMatch.Groups["length"].Value);
                    return TruncateString(valueObj.ToString(), length);
                }

                var format = GetFormat(m.Groups["format"]);
                return string.Format(format, valueObj);
            });
        }

        private string TruncateString(string value, int length)
        {
            if (string.IsNullOrEmpty(value) || value.Length <= length)
            {
                return value;
            }

            return value.Substring(0, length);
        }

        private string GetFormat(Group formatGroup)
        {
            if (!formatGroup.Success)
            {
                return "{0}";
            }

            var format = formatGroup.Captures[0].Value;

            if (string.IsNullOrEmpty(format))
            {
                return "{0}";
            }

            // Don't use standard string.Format for truncate - it's handled separately
            if (TruncateRegex.IsMatch(format))
            {
                return "{0}";
            }

            return "{0:" + format + "}";
        }

        private string PerformForLoopSubstitutions(string template, IDictionary<string, object> data, bool preserveContent = false)
        {
            return ForRegex.Replace(template, m =>
            {
                var key = m.Groups["variable"].Captures[0].Value;
                var name = m.Groups["n"].Captures[0].Value;

                var rawValue = data.FirstOrDefault(x => x.Key.ToLower().Equals(key.ToLower()));

                if (string.IsNullOrEmpty(rawValue.Key))
                {
                    return string.Empty;
                }

                // If the type is not IEnumerable, or type is string (because string is enumerable to char[])
                // Then return an error as we don't want to iterate over it.
                if (!(rawValue.Value is IEnumerable items) || rawValue.Value is string)
                {
                    return $"(ERROR: {key} is null or not a `IEnumerable`)";
                }

                var startIndex = $"[for {name} in {key}]".Length;
                var endIndex = m.Value.Length - "[end for]".Length - startIndex;

                var content = m.Value.Substring(startIndex, endIndex).Replace($"{name}.", string.Empty);
                var splitResult = ForSplit.Match(content);
                var mod = new LoopMod();

                if (splitResult.Success)
                {
                    mod.HasMod = true;
                    mod.Value = splitResult.Groups["value"].Captures[0].Value;
                    mod.ModAt = int.Parse(splitResult.Groups["mod"].Captures[0].Value);

                    content = ForSplit.Replace(content, string.Empty);
                }

                var sb = new StringBuilder();
                var index = 0;

                foreach (var item in items)
                {
                    if (mod.HasMod && index > 0 && index % mod.ModAt == 0)
                    {
                        sb.AppendLine(mod.Value);
                    }

                    var moduleScope = item.GetType().Module.ScopeName;
                    var isModuleScope = moduleScope == "CommonLanguageRuntimeLibrary" || moduleScope.StartsWith("System");

                    if (item is IDictionary<string, object> banana)
                    {
                        sb.Append(Render(content, banana, true));
                    }
                    else if (isModuleScope)
                    {
                        sb.Append(Render(content, new Dictionary<string, object>
                        {
                            [name] = item
                        }, true));
                    }
                    else
                    {
                        var itemAsDic = Convert(item, data);
                        sb.Append(Render(content, itemAsDic, true));
                    }

                    index++;
                }

                return sb.ToString();
            });
        }

        private IDictionary<string, object> Convert(object item, IDictionary<string, object> data)
        {
            var properties = item.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

            var result = new Dictionary<string, object>();

            foreach (var propertyInfo in properties)
            {
                result.Add($"{propertyInfo.Name}", propertyInfo.GetValue(item));
            }

            return result;
        }

        private class LoopMod
        {
            public bool HasMod { get; set; }
            public int ModAt { get; set; }
            public string Value { get; set; } = string.Empty;
        }
    }
}