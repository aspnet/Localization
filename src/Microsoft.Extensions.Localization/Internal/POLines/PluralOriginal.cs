// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System;
using System.Text;

namespace Microsoft.Extensions.Localization.Internal.POLines
{    
    public class PluralOriginal : OriginalLine
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
            return new PluralOriginal { Value = TrimQuotes(TrimToken(new StringBuilder(value))) };
        }
    }
}