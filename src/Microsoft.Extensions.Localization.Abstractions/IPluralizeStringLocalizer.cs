// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

namespace Microsoft.Extensions.Localization
{
    /// <summary>
    /// Represents a service that provides localized strings with pluralization support.
    /// </summary>
    public interface IPluralizeStringLocalizer : IStringLocalizer
    {
        /// <summary>
        /// Gets the plural form for a given <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the string resource.</param>
        /// <param name="count">The number of items you are trying to pluralize.</param>
        /// <returns>The pluralized string resource as a <see cref="LocalizedString"/>.</returns>
        LocalizedString Pluralize(string name, int count);

        /// <summary>
        /// Gets the pluralization rule for a given language.
        /// </summary>
        /// <param name="twoLetterISOLanguageName">The language that used to retrieve the plural forms for.</param>
        /// <returns> The pluralization rule as <see cref="PluralizationRule"/>.</returns>
        PluralizationRule GetPluralRule(string twoLetterISOLanguageName);
    }
}