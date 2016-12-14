// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Localization.Internal.POLines;

namespace Microsoft.Extensions.Localization.Internal
{
    public class POEntry
    {
        public StringBuilder OriginalBuilder { get; private set; }

        public static StringBuilder ConcatStringBuilders(StringBuilder first, StringBuilder second)
        {
            for (int i = 0; i < second.Length; i++)
            {
                first.Append(second[i]);
            }

            return first;
        }

        public void AppendOriginalLine(StringBuilder line)
        {
            if (OriginalBuilder == null)
            {
                OriginalBuilder = new StringBuilder();
            }

            ConcatStringBuilders(OriginalBuilder, line);
        }

        private string _original;

        public string Original
        {
            get
            {
                if (_original == null)
                {
                    _original = OriginalBuilder?.ToString();
                }

                return _original;
            }
        }

        public string OriginalPlural { get; set; }

        public StringBuilder TranslationBuilder { get; private set; }

        public void AppendTranslationLine(StringBuilder line)
        {
            if (TranslationBuilder == null)
            {
                TranslationBuilder = new StringBuilder();
            }

            _translation = null;
            ConcatStringBuilders(TranslationBuilder, line);
        }

        private string _translation;

        public string Translation
        {
            get
            {
                if (_translation == null)
                {
                    _translation = TranslationBuilder?.ToString();
                }

                return _translation;
            }
        }

        public IDictionary<int, string> TranslationPlurals { get; set; } = new Dictionary<int, string>();

        public string Untranslated { get; set; }

        public List<string> References { get; set; } = new List<string>();

        public List<string> Contexts { get; set; } = new List<string>();

        public List<string> Flags { get; set; } = new List<string>();

        public string Comment { get; set; }
    }

    public class POParser
    {
        private static IList<TokenLine> Lines = new List<TokenLine>
        {
            new PluralOriginal(),
            new OriginalLine(),
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
                for (int i = 0; i < Lines.Count; i++)
                {
                    var lineObject = Lines[i];
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
                        if (
                            entry.OriginalBuilder != null &&
                            (resultType == typeof(OriginalLine) ||
                                resultType.GetTypeInfo().IsSubclassOf(typeof(CommentBase))))
                        {
                            results.Add(entry.Original, entry);
                            entry = new POEntry();
                        }

                        if (resultType == typeof(OriginalLine))
                        {
                            entry.AppendOriginalLine((StringBuilder)result.Value);
                        }
                        else if (resultType == typeof(CommentLine))
                        {
                            entry.Comment = result.Value.ToString();
                        }
                        else if (resultType == typeof(ContextLine))
                        {
                            entry.Contexts.Add(result.Value.ToString());
                        }
                        else if (resultType == typeof(LiteralLine))
                        {
                            if (previousLineType == typeof(OriginalLine))
                            {
                                entry.AppendOriginalLine((StringBuilder)result.Value);
                            }
                            else if (previousLineType == typeof(TranslationLine))
                            {
                                entry.AppendTranslationLine((StringBuilder)result.Value);
                            }
                            else
                            {
                                throw new NotImplementedException();
                            }
                        }
                        else if (resultType == typeof(TranslationLine))
                        {
                            if (entry.OriginalBuilder == null || entry.TranslationBuilder != null)
                            {
                                throw new FormatException("'msgid' must come before 'msgstr'");
                            }

                            entry.AppendTranslationLine((StringBuilder)result.Value);
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
                            entry.Untranslated = result.Value.ToString();
                        }
                        else if (resultType == typeof(PluralOriginal))
                        {
                            entry.OriginalPlural = result.Value.ToString();
                        }
                        else if (resultType == typeof(PluralTranslation))
                        {
                            var plural = (PluralTranslation)result;
                            entry.TranslationPlurals.Add(plural.Plural, plural.Value.ToString());
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

                if (entry.OriginalBuilder != null)
                {
                    results.Add(entry.Original, entry);
                }
            }

            return results;
        }
    }
}
