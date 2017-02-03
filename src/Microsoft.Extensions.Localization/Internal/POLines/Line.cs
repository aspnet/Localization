// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System;
using System.Text;

namespace Microsoft.Extensions.Localization.Internal.POLines
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
}