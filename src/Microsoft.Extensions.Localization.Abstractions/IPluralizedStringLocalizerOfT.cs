// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

namespace Microsoft.Extensions.Localization
{
    /// <summary>
    /// Represents an <see cref="IPluralizeStringLocalizer"/> that provides pluralized strings for <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The <see cref="System.Type"/> to provide pluralized strings for.</typeparam>
    public interface IPluralizeStringLocalizer<T> : IPluralizeStringLocalizer
    {

    }
}