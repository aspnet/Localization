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
        public string Origional { get; set; }
        public string Translation { get; set; }
        public string Untranslated { get; set; }
        public List<string> References { get; set; } = new List<string>();
        public List<string> Contexts { get; set; } = new List<string>();
        public List<string> Flags { get; set; } = new List<string>();
        public string Comment { get; set; }
    }

    public enum POState
    {
        Original,
        Translation,
        References,
        Contexts,
        Flags
    }

    public class POParser
    {
        public POParser(Stream poString)
        {
            _reader = new StreamReader(poString);
        }

        private StreamReader _reader;

        private Line ParseLine(string line)
        {
            Line result = null;

            int i = 0;
            if (line.StartsWith(OrigionalLine.GetToken()))
            {
                result = new OrigionalLine();
                i = OrigionalLine.GetToken().Length;
            }
            else if (line.StartsWith(TranslationLine.GetToken()))
            {
                result = new TranslationLine();
                i = TranslationLine.GetToken().Length;
            }
            else if (line.StartsWith(ContextLine.GetToken()))
            {
                result = new ContextLine();
                i = ContextLine.GetToken().Length;
            }
            else if (line.StartsWith(CommentLine.GetToken()))
            {
                result = new CommentLine();
                i = CommentLine.GetToken().Length;
            }
            else if (line.StartsWith(FlagLine.GetToken()))
            {
                result = new FlagLine();
                i = FlagLine.GetToken().Length;
            }
            else if (line.StartsWith(ReferencesLine.GetToken()))
            {
                result = new ReferencesLine();
                i = ReferencesLine.GetToken().Length;
            }
            else if (line.StartsWith(UntranslatedLine.GetToken()))
            {
                result = new UntranslatedLine();
                i = UntranslatedLine.GetToken().Length;
            }
            else if (line.StartsWith("\"") || line.StartsWith("'"))
            {
                result = new LiteralLine();
                i = 0;
            }
            else if (line != null && string.IsNullOrWhiteSpace(line))
            {
                result = null;
            }
            else
            {
                throw new NotImplementedException();
            }

            if (result != null)
            {
                result.Parse(line.Substring(i));
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
                        if (entry.Origional != null && (resultType == typeof(OrigionalLine) || resultType.GetTypeInfo().IsSubclassOf(typeof(CommentBase))))
                        {
                            results.Add(entry.Origional, entry);
                            entry = new POEntry();
                        }

                        if (resultType == typeof(OrigionalLine))
                        {
                            entry.Origional = (string)result.Value;
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
                                entry.Origional += (string)result.Value;
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
                            if (entry.Origional == null || entry.Translation != null)
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

                results.Add(entry.Origional, entry);
            }

            return results;
        }
    }
}
