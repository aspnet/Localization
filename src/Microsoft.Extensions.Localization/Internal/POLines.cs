// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System;
using System.Linq;

namespace Microsoft.Extensions.Localization.Internal
{
    public abstract class Line
    {
        public object Value { get; protected set; }

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

        public abstract Line Parse(string value);
    }

    public abstract class TokenLine : Line
    {
        protected string TrimToken(string value)
        {
            return value.Substring(Token.Length);
        }

        public abstract string Token { get; }
    }

    public class OrigionalLine : TokenLine
    {
        public override string Token
        {
            get
            {
                return "msgid ";
            }
        }

        public override Line Parse(string value)
        {
            return new OrigionalLine { Value = TrimQuotes(TrimToken(value)) };
        }
    }

    public class PluralOrigional : OrigionalLine
    {
        public override string Token
        {
            get
            {
                return "msgid_plural ";
            }
        }

        public override Line Parse(string value)
        {
            return new PluralOrigional { Value = TrimQuotes(TrimToken(value)) };
        }
    }

    public class TranslationLine : TokenLine
    {
        public override string Token
        {
            get
            {
                return "msgstr ";
            }
        }

        public override Line Parse(string value)
        {
            return new TranslationLine { Value = TrimQuotes(TrimToken(value)) };
        }
    }

    public class PluralTranslation : TranslationLine
    {
        public int Plural { get; private set; }

        public override string Token
        {
            get
            {
                return "msgstr[";
            }
        }

        public override Line Parse(string value)
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
                        return new PluralTranslation
                        {
                            Plural = int.Parse(digit),
                            Value = TrimQuotes(value.Substring(i, value.Length - i))
                        };
                    }
                    else
                    {
                        throw new FormatException();
                    }
                }
            }

            throw new NotImplementedException("Line malformed, should never reach here");
        }
    }

    public class ContextLine : TokenLine
    {
        public override string Token
        {
            get
            {
                return "msgctxt ";
            }
        }

        public override Line Parse(string value)
        {
            return new ContextLine
            {
                Value = TrimSpaces(TrimToken(value))
            };
        }
    }

    public abstract class CommentBase : TokenLine
    {
    }

    public class CommentLine : CommentBase
    {
        public override string Token
        {
            get
            {
                return "#";
            }
        }

        public override Line Parse(string value)
        {
            return new CommentLine
            {
                Value = TrimSpaces(TrimToken(value))
            };
        }
    }

    public class ObsoleteLine : CommentBase
    {
        public override string Token
        {
            get
            {
                return "#~ ";
            }
        }

        public override Line Parse(string value)
        {
            return null;
        }
    }

    public class FlagLine : CommentBase
    {
        public override string Token
        {
            get
            {
                return "#, ";
            }
        }

        public override Line Parse(string value)
        {
            return new FlagLine
            {
                Value = TrimToken(value).Split(',').ToList()
            };
        }
    }

    public class UntranslatedLine : CommentBase
    {
        public override string Token
        {
            get
            {
                return "#|";
            }
        }

        public override Line Parse(string value)
        {
            return new UntranslatedLine
            {
                Value = TrimToken(value)
            };
        }
    }

    public class ReferencesLine : CommentBase
    {
        public override string Token
        {
            get
            {
                return "#: ";
            }
        }

        public override Line Parse(string value)
        {
            return new ReferencesLine
            {
                Value = TrimToken(value).Split(' ').ToList()
            };
        }
    }

    public class LiteralLine : Line
    {
        public override Line Parse(string value)
        {
            return new LiteralLine
            {
                Value = TrimQuotes(value)
            };
        }
    }
}
