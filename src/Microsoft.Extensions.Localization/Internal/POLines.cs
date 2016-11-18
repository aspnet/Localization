// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System;
using System.Linq;

namespace Microsoft.Extensions.Localization.Internal
{
    public abstract class Line
    {
        public object Value { get; protected set; }

        public abstract void Parse(string value);

        protected string TrimQuotes(string value)
        {
            if (IsQuote(value[0]))
            {
                var whichQuote = value[0];
                int i = 1;
                while (value[i] != whichQuote || value[i - 1] == '\\')
                {
                    i++;
                }
                if (i < value.Length - 1)
                {
                    throw new FormatException();
                }
                return Unescape(value.Substring(1, i - 1));

                //TODO: Check for values at the end
            }
            else
            {
                throw new FormatException("Line malformed");
            }
        }

        protected string TrimSpaces(string value)
        {
            return value.Trim();
        }

        private bool IsQuote(char character)
        {
            return character == '"' || character == '\'';
        }

        private string Unescape(string value)
        {
            return value.Replace("\\\"", "\"").Replace("\\'", "'");
        }
    }

    public abstract class TokenLine : Line
    {
        public static string GetToken()
        {
            throw new NotImplementedException();
        }
    }

    public class OrigionalLine : TokenLine
    {
        public static new string GetToken()
        {
            return "msgid ";
        }

        public override void Parse(string value)
        {
            Value = TrimQuotes(value);
        }
    }

    public class PluralOrigional : OrigionalLine
    {
        public static new string GetToken()
        {
            return "msgid_plural ";
        }
    }

    public class TranslationLine : TokenLine
    {
        public static new string GetToken()
        {
            return "msgstr ";
        }

        public override void Parse(string value)
        {
            Value = TrimQuotes(value);
        }
    }

    public class PluralTranslation : TranslationLine
    {
        public int Plural { get; private set; }

        public static new string GetToken()
        {
            return "msgstr[";
        }

        public override void Parse(string value)
        {
            var digit = "";

            int i;
            for (i = 0; i < value.Length; i++)
            {
                if (value[i] >= '0' && value[i] <= '9')
                {
                    digit += value[i];
                    if (value[i + 1] == ']' && value[i + 2] == ' ')
                    {
                        i += 3;
                        Plural = int.Parse(digit);
                        Value = TrimQuotes(value.Substring(i, value.Length - i));
                        return;
                    }
                    else
                    {
                        throw new FormatException();
                    }
                }
            }
        }
    }

    public class ContextLine : TokenLine
    {
        public static new string GetToken()
        {
            return "msgctxt ";
        }

        public override void Parse(string value)
        {
            Value = TrimSpaces(value);
        }
    }

    public abstract class CommentBase : TokenLine
    {
    }

    public class CommentLine : CommentBase
    {
        public static new string GetToken()
        {
            return "# ";
        }

        public override void Parse(string value)
        {
            Value = TrimSpaces(value);
        }
    }

    public class FlagLine : CommentBase
    {
        public static new string GetToken()
        {
            return "#, ";
        }

        public override void Parse(string value)
        {
            Value = value.Split(',').ToList();
        }
    }

    public class UntranslatedLine : CommentBase
    {
        public static new string GetToken()
        {
            return "#|";
        }

        public override void Parse(string value)
        {
            Value = value;
        }
    }

    public class ReferencesLine : CommentBase
    {
        public static new string GetToken()
        {
            return "#: ";
        }

        public override void Parse(string value)
        {
            Value = value.Split(' ').ToList();
        }
    }

    public class LiteralLine : Line
    {
        public override void Parse(string value)
        {
            Value = TrimQuotes(value);
        }
    }
}
