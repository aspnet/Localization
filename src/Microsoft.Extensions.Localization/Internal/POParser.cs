// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Microsoft.Extensions.Localization.Internal
{
    public class POEntry
    {
        public string Original { get; set; }
        public string OriginalPlural { get; set; }
        public string Translation { get; set; }
        public IDictionary<int, string> TranslationPlurals { get; } = new Dictionary<int, string>();
        public string Untranslated { get; set; }
        public List<string> References { get; set; } = new List<string>();
        public List<string> Contexts { get; set; } = new List<string>();
        public List<string> Flags { get; set; } = new List<string>();
        public string Comment { get; set; }
    }

    public class POParser
    {
        private static IEnumerable<Type> Lines
        {
            get
            {
                return new List<Type>
            {
                typeof(OrigionalLine),
                typeof(PluralOrigional),
                typeof(TranslationLine),
                typeof(PluralTranslation),
                typeof(ContextLine),
                typeof(CommentLine),
                typeof(FlagLine),
                typeof(UntranslatedLine),
                typeof(ReferencesLine)
            };
            }
        }

        public POParser(Stream poString)
        {
            _reader = new StreamReader(poString);
        }

        private StreamReader _reader;

        private Line ParseLine(string line)
        {
            Line result = null;
            int i = 0;

            foreach (var lineType in Lines)
            {
                // Reflection to decrease code. Gross.
                var getToken = lineType.GetMethod("GetToken");
                var token = (string)getToken.Invoke(null, new object[] { });

                if (line.StartsWith(token))
                {
                    var constructor = lineType.GetConstructor(new Type[] { });
                    result = (Line)constructor.Invoke(new Type[] { });
                    i = token.Length;
                }
                else if (line.StartsWith("\"") || line.StartsWith("'"))
                {
                    result = new LiteralLine();
                    i = 0;
                }
                else if (line != null && string.IsNullOrWhiteSpace(line))
                {
                    return null;
                }
            }

            if (result != null)
            {
                result.Parse(line.Substring(i));
            }
            else
            {
                throw new NotImplementedException();
            }
            return result;
        }

        internal IDictionary<string, POEntry> ParseLocalizationStream()
        {
            var results = new Dictionary<string, POEntry>();
            var entry = new POEntry();

            using (_reader)
            {
                Type previousLineType = null;

                string line;
                while ((line = _reader.ReadLine()) != null)
                {
                    var result = ParseLine(line);

                    if (result != null)
                    {
                        var resultType = result.GetType();
                        if (entry.Original != null && (resultType == typeof(OrigionalLine) || resultType.GetTypeInfo().IsSubclassOf(typeof(CommentBase))))
                        {
                            results.Add(entry.Original, entry);
                            entry = new POEntry();
                        }

                        if (resultType == typeof(OrigionalLine))
                        {
                            entry.Original = (string)result.Value;
                        }
                        else if (resultType == typeof(CommentLine))
                        {
                            entry.Comment = (string)result.Value;
                        }
                        else if (resultType == typeof(ContextLine))
                        {
                            entry.Contexts.Add((string)result.Value);
                        }
                        else if (resultType == typeof(LiteralLine))
                        {
                            if (previousLineType == typeof(OrigionalLine))
                            {
                                entry.Original += (string)result.Value;
                            }
                            else if (previousLineType == typeof(TranslationLine))
                            {
                                entry.Translation += (string)result.Value;
                            }
                            else
                            {
                                throw new NotImplementedException();
                            }
                        }
                        else if (resultType == typeof(TranslationLine))
                        {
                            if (entry.Original == null || entry.Translation != null)
                            {
                                throw new FormatException("'msgid' must come before 'msgstr'");
                            }

                            entry.Translation = (string)result.Value;
                        }
                        else if (resultType == typeof(FlagLine))
                        {
                            entry.Flags.AddRange((List<string>)result.Value);
                        }
                        else if (resultType == typeof(ReferencesLine))
                        {
                            entry.References.AddRange((List<string>)result.Value);
                        }
                        else if (resultType == typeof(UntranslatedLine))
                        {
                            entry.Untranslated = (string)result.Value;
                        }
                        else if (resultType == typeof(PluralOrigional))
                        {
                            entry.OriginalPlural = (string)result.Value;
                        }
                        else if (resultType == typeof(PluralTranslation))
                        {
                            var plural = (PluralTranslation)result;
                            entry.TranslationPlurals.Add(plural.Plural, (string)plural.Value);
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }

                        if (resultType != typeof(LiteralLine))
                        {
                            previousLineType = resultType;
                        }
                    }
                }

                results.Add(entry.Original, entry);
            }

            return results;
        }
    }
}
