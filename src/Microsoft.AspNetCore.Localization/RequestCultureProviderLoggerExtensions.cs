﻿// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace Microsoft.AspNetCore.Localization
{
    internal static class RequestCultureProviderLoggerExtensions
    {
        private static readonly Action<ILogger, string, IList<StringSegment>, Exception> _unsupportedCulture;
        private static readonly Action<ILogger, string, IList<StringSegment>, Exception> _unsupportedUICulture;

        static RequestCultureProviderLoggerExtensions()
        {
            _unsupportedCulture = LoggerMessage.Define<string, IList<StringSegment>>(
                LogLevel.Warning,
                1,
                "{requestCultureProvider} returned the following unsupported cultures '{cultures}'.");
            _unsupportedUICulture = LoggerMessage.Define<string, IList<StringSegment>>(
                LogLevel.Warning,
                2,
                "{requestCultureProvider} returned the following unsupported cultures '{cultures}'.");
        }

        public static void UnsupportedCulture(this ILogger logger, string requestCultureProvider, IList<StringSegment> cultures)
        {
            _unsupportedCulture(logger, requestCultureProvider, cultures, null);
        }

        public static void UnsupportedUICulture(this ILogger logger, string requestCultureProvider, IList<StringSegment> uiCultures)
        {
            _unsupportedUICulture(logger, requestCultureProvider, uiCultures, null);
        }
    }
}
