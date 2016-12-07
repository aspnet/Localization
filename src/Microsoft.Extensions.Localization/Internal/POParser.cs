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
        private static IEnumerable<TokenLine> Lines = new List<TokenLine>
        {
            new PluralOrigional(),
            new OrigionalLine(),
            new TranslationLine(),
            new PluralTranslation(),
            new ContextLine(),
            new FlagLine(),
            new UntranslatedLine(),
            new ReferencesLine(),
            new ObsoleteLine(),
            new CommentLine()
        };

        public POParser(Stream poString)
        {
            _reader = new StreamReader(poString);
        }

        private StreamReader _reader;

        private Line ParseLine(string line)
        {
            Line result = null;

            if (line.StartsWith("\"") || line.StartsWith("'"))
            {
                result = new LiteralLine();
            }
            else if (line != null && string.IsNullOrWhiteSpace(line))
            {
                return null;
            }
            else
            {
                foreach (var lineObject in Lines)
                {
                    var token = lineObject.Token;

                    if (line.StartsWith(token))
                    {
                        result = lineObject;
                        break;
                    }
                }
            }

            if (result != null)
            {
                //TODO: Trim inside parse
                return result.Parse(line);
            }
            else
            {
                throw new NotImplementedException("Not a recognized line type");
            }
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

                if (entry.Original != null)
                {
                    results.Add(entry.Original, entry);
                }
            }

            return results;
        }
    }
}
