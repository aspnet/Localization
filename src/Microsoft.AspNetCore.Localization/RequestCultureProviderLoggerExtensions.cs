// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Localization
{
    internal static class LocalizationLoggerExtensions
    {
        private static readonly Action<ILogger, string, string, Exception> _parsedCulture;

        static LocalizationLoggerExtensions()
        {
            _parsedCulture = LoggerMessage.Define<string, string>(
                LogLevel.Warning,
                1,
                $"{{requestCultureProvider}} contains invalid culture '{{cultureName}}'.");
        }

        public static void InvalidCultureName(this ILogger logger, string requestCultureProvider, string cultureName)
        {
            _parsedCulture(logger, requestCultureProvider, cultureName, null);
        }
    }
}
