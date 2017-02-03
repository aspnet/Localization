// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System;
using System.Text;

namespace Microsoft.Extensions.Localization.Internal.POLines
{
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
}