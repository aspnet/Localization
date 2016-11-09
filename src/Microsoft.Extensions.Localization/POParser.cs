// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Microsoft.Extensions.Localization
{
    public class POEntry
    {
        public string Origional { get; set; }
        public string Translation { get; set; }
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
        internal IDictionary<string, POEntry> ParseLocalizationStream(Stream poStream)
        {
            var results = new Dictionary<string, POEntry>();
            var entry = new POEntry();

            using (var reader = new StreamReader(poStream))
            {
                string line;
                POState? state = null;
                while ((line = reader.ReadLine()) != null)
                {
                    if ((line.StartsWith("#") || line.StartsWith("msgid")) && entry.Origional != null)
                    {
                        results.Add(entry.Origional, entry);
                        entry = new POEntry();
                    }

                    if (line.StartsWith("\"") || line.StartsWith("'"))
                    {
                        var unquoted = Unquote(line);
                        switch (state)
                        {
                            case POState.Original:
                                entry.Origional += unquoted;
                                break;
                            case POState.Translation:
                                entry.Translation += unquoted;
                                break;
                        }
                    }
                    else if (line.StartsWith("#:"))
                    {
                        entry.References.AddRange(ParseReferences(line));
                        state = POState.References;
                    }
                    else if (line.StartsWith("#,"))
                    {
                        entry.Flags.AddRange(ParseFlags(line));
                        state = POState.Flags;
                    }
                    else if (line.StartsWith("#"))
                    {
                        entry.Comment = ParseComment(line);
                    }
                    else if (line.StartsWith("msgid"))
                    {
                        entry.Origional = ParseOrigional(line);
                        state = POState.Original;
                    }
                    else if (line.StartsWith("msgstr"))
                    {
                        entry.Translation = ParseTranslation(line);
                        state = POState.Translation;
                    }
                    else if (line.StartsWith("msgctxt"))
                    {
                        entry.Contexts.Add(ParseContext(line));
                    }
                    else if (string.IsNullOrWhiteSpace(line))
                    {
                        // empty line, do nothing
                    }
                    else
                    {
                        throw new NotImplementedException("Unknown!");
                    }
                }

                // Add the final entry
                results.Add(entry.Origional, entry);
            }

            return results;
        }

        private string ParseContext(string value)
        {
            return Trim(value, 7);
        }

        private string ParseComment(string value)
        {
            return Trim(value, 1);
        }

        private string ParseOrigional(string value)
        {
            return Unquote(Trim(value, 5));
        }

        private string ParseTranslation(string value)
        {
            return Unquote(Trim(value, 6));
        }

        private IEnumerable<string> ParseReferences(string value)
        {
            return Trim(value, 2).Split(null);
        }

        private IEnumerable<string> ParseFlags(string value)
        {
            return Trim(value, 2).Split(null);
        }

        private string Unquote(string value)
        {
            if ((value.StartsWith("'") && value.EndsWith("'")) || (value.StartsWith("\"") && value.EndsWith("\"")))
            {
                return value.Trim().Substring(1, value.Length - 2);
            }
            else
            {
                throw new FormatException("msgid and msgstr values must be surrounded with ' or \"");
            }
        }

        private string Trim(string value, int chars)
        {
            return value.Substring(chars).Trim();
        }
    }
}
