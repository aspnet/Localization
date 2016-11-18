// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

namespace Microsoft.Extensions.Localization
{
    /// <summary>
    /// Represents the method that will determine the pluralization rule.
    /// </summary>
    /// <param name="pluralCount">The count that used to determine the plural form.</param>
    public delegate int PluralizationRule(int pluralCount);
}