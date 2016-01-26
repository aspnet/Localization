// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.Extensions.Localization
{
    /// <summary>
    /// Provides programmatic configuration for localization.
    /// </summary>
    public class LocalizationOptions
    {
        /// <summary>
        /// The relative path under application root where resource files are located.
        /// </summary>
        public string ResourcesPath { get; set; } = string.Empty;

        /// <summary>
        /// The lookup behavior to use when a given resource key is not found.
        /// </summary>
        public ResourceLookupBehavior ResourceLookupBehavior { get; set; } = ResourceLookupBehavior.UseNameIfNotFound;
    }
}
