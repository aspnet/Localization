// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

namespace Microsoft.Extensions.Localization.Abstractions
{
    /// <summary>
    /// Represents a service that provides localized and pluralized strings.
    /// </summary>
    public interface IPluralizedLocalizer : IStringLocalizer
    {
        /// <summary>
        /// Gets the string resource with the given name for the given plurality.
        /// </summary>
        /// <param name="name">The name of the string resource.</param>
        /// <param name="number">The plurality of the string resource</param>
        /// <returns>The string pluralized resource as a <see cref="LocalizedString"/>.</returns>
        LocalizedString this[string name, int number] { get; }

        /// <summary>
        /// Gets the string resource with the given name for the given plurality.
        /// </summary>
        /// <param name="name">The name of the string resource.</param>
        /// <param name="number">The plurality of the string resource</param>
        /// <param name="arguments">The values to format the string with.</param>
        /// <returns>The string pluralized resource as a <see cref="LocalizedString"/>.</returns>
        LocalizedString this[string name, int number, params object[] arguments] { get; }
    }
}
