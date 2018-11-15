// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
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
        /// <remarks>This method ensures that <paramref name="requestCultureProvider"/> has priority over other <see cref="RequestCultureProvider"/> instances in <see cref="RequestLocalizationOptions.RequestCultureProviders"/>.</remarks>
        public static RequestLocalizationOptions AddInitialRequestCultureProvider(
            this RequestLocalizationOptions requestLocalizationOptions,
            RequestCultureProvider requestCultureProvider)
        {
            if (requestLocalizationOptions == null)
            {
                throw new ArgumentNullException(nameof(requestLocalizationOptions));
            }

            if (requestCultureProvider == null)
            {
                throw new ArgumentNullException(nameof(requestCultureProvider));
            }

            requestLocalizationOptions.RequestCultureProviders.Insert(0, requestCultureProvider);

            return requestLocalizationOptions;
        }
    }
}
