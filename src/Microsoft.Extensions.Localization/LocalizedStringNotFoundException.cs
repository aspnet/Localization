// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.Extensions.Localization
{
    /// <summary>
    /// The <see cref="Exception"/> that is thrown when a localized string cannot be found.
    /// </summary>
    public class LocalizedStringNotFoundException : Exception
    {
		/// <summary> 
        /// Creates a new instance of <see cref="LocalizedStringNotFoundException"/> with the specified 
        /// exception <paramref name="message"/>. 
        /// </summary> 
        /// <param name="message">The message that describes the error.</param> 
        public LocalizedStringNotFoundException(string message) 
            : base(message) 
        { 
            if (message == null) 
            { 
                throw new ArgumentNullException(nameof(message)); 
            } 
        } 
    }
}
