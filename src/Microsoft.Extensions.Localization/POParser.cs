// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Microsoft.Extensions.Localization
{
    public class POEntry
    {
        public string Origional { get; set; }
        public string Translation { get; set; }
        public IList<string> References { get; set; } = new List<string>();
        public IList<string> Contexts { get; set; } = new List<string>();
        public IList<string> Flags { get; set; } = new List<string>();
    }

    public class POParser
    {
        private static readonly IEnumerable<string> _specialStarts = new List<string> {
            "#",
            "msg"
        };

        private static readonly Dictionary<char, char> _escapeTranslations = new Dictionary<char, char> {
            { 'n', '\n' },
            { 'r', '\r' },
            { 't', '\t' }
        };

        public void ParseLocalizationStream(string text, IDictionary<string, POEntry> translations, bool merge)
        {
            var reader = new StringReader(text);
            string poLine;
            var multiLineId = true;
            var poEntry = new POEntry();
            var finishingEntry = false;

            while (true)
            {
                poLine = reader.ReadLine();

                if (finishingEntry && (poLine == null || poLine.StartsWith("#") || poLine.StartsWith("msg")))
                {
                    translations.Add(poEntry.Origional, poEntry);
                    finishingEntry = false;
                    poEntry = new POEntry();
                }

                if (poLine == null)
                {
                    break;
                }

                if (poLine.StartsWith("#:"))
                {
                    poEntry.References.Add(ParseReference(poLine));
                    continue;
                }

                if (poLine.StartsWith("#,"))
                {
                    poEntry.Flags.Add(ParseFlags(poLine));
                    continue;
                }

                if (poLine.StartsWith("msgctxt"))
                {
                    //We're not able to take advantage of context due to the shape of IStringLocalizer.
                    poEntry.Contexts.Add(ParseContext(poLine));
                    continue;
                }

                if (poLine.StartsWith("msgid"))
                {
                    multiLineId = true;
                    poEntry.Origional = ParseId(poLine);
                    continue;
                }

                if (poLine.StartsWith("msgstr"))
                {
                    finishingEntry = true;
                    multiLineId = false;
                    poEntry.Translation = ParseTranslation(poLine);
                    // ignore incomplete localizations (empty msgid or msgstr)
                    // TODO: Null translation means Translation is original
                    continue;
                }

                // Continuation of a multiline
                if (poLine.StartsWith("\""))
                {
                    if (multiLineId)
                    {
                        poEntry.Origional += ParseMultiLine(poLine);
                    }
                    else
                    {
                        poEntry.Translation += ParseMultiLine(poLine);
                    }
                }
            }
        }

        private static string Unescape(string str)
        {
            StringBuilder sb = null;
            bool escaped = false;
            for (var i = 0; i < str.Length; i++)
            {
                var c = str[i];
                if (escaped)
                {
                    if (sb == null)
                    {
                        sb = new StringBuilder(str.Length);
                        if (i > 1)
                        {
                            sb.Append(str.Substring(0, i - 1));
                        }
                    }
                    char unescaped;
                    if (_escapeTranslations.TryGetValue(c, out unescaped))
                    {
                        sb.Append(unescaped);
                    }
                    else
                    {
                        // General rule: \x ==> x
                        sb.Append(c);
                    }
                    escaped = false;
                }
                else
                {
                    if (c == '\\')
                    {
                        escaped = true;
                    }
                    else if (sb != null)
                    {
                        sb.Append(c);
                    }
                }
            }
            return sb == null ? str : sb.ToString();
        }

        private static string TrimQuote(string str)
        {
            if (str.StartsWith("\"") && str.EndsWith("\""))
            {
                return str.Substring(1, str.Length - 2);
            }

            return str;
        }

        private static string ParseMultiLine(string poLine)
        {
            return Unescape(TrimQuote(poLine));
        }

        private static string ParseTranslation(string poLine)
        {
            return Unescape(TrimQuote(poLine.Substring(6).Trim()));
        }

        private static string ParseFlags(string poLine)
        {
            return Unescape(TrimQuote(poLine.Substring(2).Trim()));
        }

        private static string ParseId(string poLine)
        {
            return Unescape(TrimQuote(poLine.Substring(5).Trim()));
        }

        private static string ParseReference(string poLine)
        {
            return Unescape(TrimQuote(poLine.Substring(2).Trim()));
        }

        private static string ParseContext(string poLine)
        {
            return Unescape(TrimQuote(poLine.Substring(7).Trim()));
        }
    }
}
