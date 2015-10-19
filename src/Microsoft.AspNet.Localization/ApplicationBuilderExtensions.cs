// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System;
using System.Globalization;
using Microsoft.AspNet.Localization;

namespace Microsoft.AspNet.Builder
{
    /// <summary>
    /// Extension methods for adding the <see cref="RequestLocalizationMiddleware"/> to an application.
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds the <see cref="RequestLocalizationMiddleware"/> to automatically set culture information for
        /// requests based on information provided by the client using the default options.
        /// </summary>
        /// <param name="builder">The <see cref="IApplicationBuilder"/>.</param>
        /// <param name="defaultRequestCulture">The default <see cref="RequestCulture"/> to use if none of the
        /// requested cultures match supported cultures.</param>
        /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
        public static IApplicationBuilder UseRequestLocalization(
            this IApplicationBuilder builder,
            RequestCulture defaultRequestCulture)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (defaultRequestCulture == null)
            {
                throw new ArgumentNullException(nameof(defaultRequestCulture));
            }

            var options = new RequestLocalizationOptions();

            return UseRequestLocalization(builder, options, defaultRequestCulture);
        }

        /// <summary>
        /// Adds the <see cref="RequestLocalizationMiddleware"/> to automatically set culture information for
        /// requests based on information provided by the client.
        /// </summary>
        /// <param name="builder">The <see cref="IApplicationBuilder"/>.</param>
        /// <param name="options">The options to configure the middleware with.</param>
        /// <param name="defaultRequestCulture">The default <see cref="RequestCulture"/> to use if none of the
        /// requested cultures match supported cultures.</param>
        /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
        public static IApplicationBuilder UseRequestLocalization(
            this IApplicationBuilder builder,
            RequestLocalizationOptions options,
            RequestCulture defaultRequestCulture)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (defaultRequestCulture == null)
            {
                throw new ArgumentNullException(nameof(defaultRequestCulture));
            }

            return builder.UseMiddleware<RequestLocalizationMiddleware>(options, defaultRequestCulture);
        }
    }
}