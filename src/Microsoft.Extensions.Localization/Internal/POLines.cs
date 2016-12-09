// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System;
using System.Linq;
using System.Text;

namespace Microsoft.Extensions.Localization.Internal
{
    public abstract class Line
    {
        public object Value { get; protected set; }

        protected StringBuilder TrimQuotes(StringBuilder value)
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
                return Unescape(value.Remove(0, 1).Remove(i - 1, 1));

                //TODO: Check for values at the end
            }
            else
            {
                throw new FormatException("Line malformed");
            }
        }

        private bool IsQuote(char character)
        {
            return character == '"' || character == '\'';
        }

        private StringBuilder Unescape(StringBuilder value)
        {
            return value.Replace("\\\"", "\"").Replace("\\'", "'");
        }

        public abstract Line Parse(string value);
    }

    public abstract class TokenLine : Line
    {
        protected StringBuilder TrimToken(StringBuilder value)
        {
            return value.Remove(0, Token.Length);
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
            return new OrigionalLine { Value = TrimQuotes(TrimToken(new StringBuilder(value))) };
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
            return new PluralOrigional { Value = TrimQuotes(TrimToken(new StringBuilder(value))) };
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
            return new TranslationLine { Value = TrimQuotes(TrimToken(new StringBuilder(value))) };
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

            var sb = new StringBuilder(value);

            sb = sb.Remove(0, Token.Length);

            int i;
            for (i = 0; i < sb.Length; i++)
            {
                if (sb[i] >= '0' && sb[i] <= '9')
                {
                    digit += sb[i];
                    if (sb[i + 1] == ']' && sb[i + 2] == ' ')
                    {
                        return new PluralTranslation
                        {
                            Plural = int.Parse(digit),
                            Value = TrimQuotes(sb.Remove(0, i + 3))
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
                Value = TrimToken(new StringBuilder(value))
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
                Value = TrimToken(new StringBuilder(value))
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
                Value = TrimToken(new StringBuilder(value)).ToString().Split(',').ToList()
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
                Value = TrimToken(new StringBuilder(value))
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
                Value = TrimToken(new StringBuilder(value)).ToString().Split(' ').ToList()
            };
        }
    }

    public class LiteralLine : Line
    {
        public override Line Parse(string value)
        {
            return new LiteralLine
            {
                Value = TrimQuotes(new StringBuilder(value))
            };
        }
    }
}
