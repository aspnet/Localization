// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

namespace Microsoft.Extensions.Localization
{
    /// <summary>
    /// Defines the lookup behavior to use when a given resource key is not found.
    /// </summary>
    public enum ResourceLookupBehavior
    {
        /// <summary>
        /// Use the resource key as the string if a localized string is not found.
        /// </summary>
        UseNameIfNotFound,
		
        /// <summary>
        /// Throw an exception if a localized string is not found
        /// </summary>
        ThrowIfNotFound
    }
}
