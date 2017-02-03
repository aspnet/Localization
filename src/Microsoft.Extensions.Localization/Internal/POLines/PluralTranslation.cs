// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System;
using System.Text;

namespace Microsoft.Extensions.Localization.Internal.POLines
{

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
}