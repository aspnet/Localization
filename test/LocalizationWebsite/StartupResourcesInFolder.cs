﻿// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System.Collections.Generic;
using System.Globalization;
using LocalizationWebsite.Models;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;

namespace LocalizationWebsite
{
    public class StartupResourcesInFolder
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLocalization(options => options.ResourcesPath = "Resources");
        }

        public void Configure(
            IApplicationBuilder app,
            ILoggerFactory loggerFactory,
            IStringLocalizerFactory stringLocalizerFactory,
            IStringLocalizer<StartupResourcesInFolder> startupStringLocalizer,
            IStringLocalizer<Customer> custromerStringLocalizer)
        {
            loggerFactory.AddConsole(minLevel: LogLevel.Warning);

            app.UseRequestLocalization(options =>
            {
                options.DefaultRequestCulture = new RequestCulture("en-US", TimeZoneInfo.Local.Id);
                options.SupportedCultures = new List<CultureInfo>()
                {
                    new CultureInfo("fr-FR")
                };
                options.SupportedUICultures = new List<CultureInfo>()
                {
                    new CultureInfo("fr-FR")
                };
            });

            var stringLocalizer = stringLocalizerFactory.Create("Test", location: null);

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync(startupStringLocalizer["Hello"]);
                await context.Response.WriteAsync(" ");
                await context.Response.WriteAsync(stringLocalizer["Hello"]);
                await context.Response.WriteAsync(" ");
                await context.Response.WriteAsync(custromerStringLocalizer["Hello"]);
            });
        }
    }
}
