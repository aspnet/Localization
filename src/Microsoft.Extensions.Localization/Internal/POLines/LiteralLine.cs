// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System;
using System.Text;

namespace Microsoft.Extensions.Localization.Internal.POLines
{
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
