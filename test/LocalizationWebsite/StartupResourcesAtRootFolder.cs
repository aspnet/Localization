// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System.Collections.Generic;
using System.Globalization;
using LocalizationWebsite.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace LocalizationWebsite
{
    public class StartupResourcesAtRootFolder
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLocalization();
        }

        public void Configure(
            IApplicationBuilder app,
            ILoggerFactory loggerFactory,
            IStringLocalizerFactory stringLocalizerFactory,
            IStringLocalizer<StartupResourcesAtRootFolder> startupStringLocalizer,
            IStringLocalizer<Customer> customerStringLocalizer)
        {
            loggerFactory.AddConsole(minLevel: LogLevel.Warning);

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-US"),
                SupportedCultures = new List<CultureInfo>()
                {
                    new CultureInfo("fr-FR")
                },
                SupportedUICultures = new List<CultureInfo>()
                {
                    new CultureInfo("fr-FR")
                }
            });

            var stringLocalizer = stringLocalizerFactory.Create("Test", location: typeof(LocalizationWebsite.StartupResourcesAtRootFolder).Assembly.GetName().Name);

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync(startupStringLocalizer["Hello"]);
                await context.Response.WriteAsync(" ");
                await context.Response.WriteAsync(stringLocalizer["Hello"]);
                await context.Response.WriteAsync(" ");
                await context.Response.WriteAsync(customerStringLocalizer["Hello"]);
            });
        }
    }
}
