// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System.Text;

namespace Microsoft.Extensions.Localization.Internal.POLines
{
    public abstract class TokenLine : Line
    {
        protected StringBuilder TrimToken(StringBuilder value)
        {
            return value.Remove(0, Token.Length);
        }

        public abstract string Token { get; }
    }
}