// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System;
using System.Text;

namespace Microsoft.Extensions.Localization.Internal.POLines
{    
    public class OriginalLine : TokenLine
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
            return new OriginalLine { Value = TrimQuotes(TrimToken(new StringBuilder(value))) };
        }
    }
}