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
        /// The behavior to apply when a given resource key appears to be missing
        /// </summary>
        public ResourceLookupBehavior ResourceLookupBehavior { get; set; } = ResourceLookupBehavior.UseNameIfNotFound;
    }
}
