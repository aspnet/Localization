// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

namespace Microsoft.Extensions.Localization
{
    /// <summary>
    /// Behaviors to be used when a resource key appears to be missing
    /// </summary>
    public enum ResourceLookupBehavior
    {
        /// <summary>
        /// Use the resource key as a fallback
        /// </summary>
        UseNameIfNotFound,
        /// <summary>
        /// An exception is thrown to the caller
        /// </summary>
        ThrowIfNotFound
    }
}
