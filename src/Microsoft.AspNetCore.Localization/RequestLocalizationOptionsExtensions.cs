// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Localization;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// Extension methods for the <see cref="RequestLocalizationOptions"/>.
    /// </summary>
    public static class RequestLocalizationOptionsExtensions
    {
        /// <summary>
        /// Adds a new <see cref="RequestCultureProvider"/> to the <see cref="RequestLocalizationOptions.RequestCultureProviders"/>.
        /// </summary>
        /// <param name="requestLocalizationOptions">The cultures to be added.</param>
        /// <param name="requestCultureProvider">The cultures to be added.</param>
        /// <returns>The <see cref="RequestLocalizationOptions"/>.</returns>
        /// <remarks>Adding <paramref name="requestCultureProvider"/> ensures that it has priority over any of the default <see cref="RequestCultureProvider"/>s.</remarks>
        public static RequestLocalizationOptions AddRequestCultureProvider(
            this RequestLocalizationOptions requestLocalizationOptions,
            RequestCultureProvider requestCultureProvider)
        {
            requestLocalizationOptions.RequestCultureProviders.Insert(0, requestCultureProvider);

            return requestLocalizationOptions;
        }
    }
}
