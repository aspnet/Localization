// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Localization.Internal
{
    internal static class RequestCultureProviderLoggerExtensions
    {
        private static readonly Action<ILogger, string, string, Exception> _invalidCulture;
        private static readonly Action<ILogger, string, string, Exception> _invalidUICulture;

        static RequestCultureProviderLoggerExtensions()
        {
            _invalidCulture = LoggerMessage.Define<string, string>(
                LogLevel.Warning,
                1,
                "{requestCultureProvider} contains invalid culture '{cultureName}'.");
            _invalidUICulture = LoggerMessage.Define<string, string>(
                LogLevel.Warning,
                1,
                "{requestCultureProvider} contains invalid ui-culture '{cultureName}'.");
        }

        public static void InvalidCultureName(this ILogger logger, string requestCultureProvider, string cultureName)
        {
            _invalidCulture(logger, requestCultureProvider, cultureName, null);
        }

        public static void InvalidUICultureName(this ILogger logger, string requestCultureProvider, string cultureName)
        {
            _invalidUICulture(logger, requestCultureProvider, cultureName, null);
        }
    }
}
