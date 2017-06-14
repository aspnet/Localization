// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using Microsoft.Net.Http.Headers;
using Xunit;

namespace Microsoft.Extensions.Localization.Tests
{
    public class CookieRequestCultureProviderTest
    {
        [Fact]
        public async Task GetCultureInfoFromPersistentCookie()
        {
            var builder = new WebHostBuilder()
                .Configure(app =>
                {
                    var options = new RequestLocalizationOptions
                    {
                        DefaultRequestCulture = new RequestCulture("en-US"),
                        SupportedCultures = new List<CultureInfo>
                        {
                            new CultureInfo("ar-SA")
                        },
                        SupportedUICultures = new List<CultureInfo>
                        {
                            new CultureInfo("ar-SA")
                        }
                    };
                    var provider = new CookieRequestCultureProvider
                    {
                        CookieName = "Preferences"
                    };
                    options.RequestCultureProviders.Insert(0, provider);

                    app.UseRequestLocalization(options);
                    app.Run(context =>
                    {
                        var requestCultureFeature = context.Features.Get<IRequestCultureFeature>();
                        var requestCulture = requestCultureFeature.RequestCulture;
                        Assert.Equal("ar-SA", requestCulture.Culture.Name);
                        return Task.FromResult(0);
                    });
                });

            using (var server = new TestServer(builder))
            {
                var client = server.CreateClient();
                var culture = new CultureInfo("ar-SA");
                var requestCulture = new RequestCulture(culture);
                var value = CookieRequestCultureProvider.MakeCookieValue(requestCulture);
                client.DefaultRequestHeaders.Add("Cookie", new CookieHeaderValue("Preferences", value).ToString());
                var response = await client.GetAsync(string.Empty);
                Assert.Equal("c=ar-SA|uic=ar-SA", value);
            }
        }

        [Fact]
        public async Task GetDefaultCultureInfoIfCultureKeysAreMissingOrInvalid()
        {
            var builder = new WebHostBuilder()
                .Configure(app =>
                {
                    var options = new RequestLocalizationOptions
                    {
                        DefaultRequestCulture = new RequestCulture("en-US"),
                        SupportedCultures = new List<CultureInfo>
                        {
                            new CultureInfo("ar-SA")
                        },
                        SupportedUICultures = new List<CultureInfo>
                        {
                            new CultureInfo("ar-SA")
                        }
                    };
                    var provider = new CookieRequestCultureProvider
                    {
                        CookieName = "Preferences"
                    };
                    options.RequestCultureProviders.Insert(0, provider);
                    app.UseRequestLocalization(options);
                    app.Run(context =>
                    {
                        var requestCultureFeature = context.Features.Get<IRequestCultureFeature>();
                        var requestCulture = requestCultureFeature.RequestCulture;
                        Assert.Equal("en-US", requestCulture.Culture.Name);
                        return Task.FromResult(0);
                    });
                });

            using (var server = new TestServer(builder))
            {
                var client = server.CreateClient();

                client.DefaultRequestHeaders.Add("Cookie", new CookieHeaderValue("Preferences", "uic=ar-SA").ToString());
                var response = await client.GetAsync(string.Empty);
            }
        }

        [Fact]
        public async Task GetDefaultCultureInfoIfCookieDoesNotExist()
        {
            var builder = new WebHostBuilder()
                .Configure(app =>
                {
                    var options = new RequestLocalizationOptions
                    {
                        DefaultRequestCulture = new RequestCulture("en-US"),
                        SupportedCultures = new List<CultureInfo>
                        {
                            new CultureInfo("ar-SA")
                        },
                        SupportedUICultures = new List<CultureInfo>
                        {
                            new CultureInfo("ar-SA")
                        }
                    };
                    var provider = new CookieRequestCultureProvider
                    {
                        CookieName = "Preferences"
                    };
                    options.RequestCultureProviders.Insert(0, provider);
                    app.UseRequestLocalization(options);
                    app.Run(context =>
                    {
                        var requestCultureFeature = context.Features.Get<IRequestCultureFeature>();
                        var requestCulture = requestCultureFeature.RequestCulture;
                        Assert.Equal("en-US", requestCulture.Culture.Name);
                        return Task.FromResult(0);
                    });
                });

            using (var server = new TestServer(builder))
            {
                var client = server.CreateClient();
                var response = await client.GetAsync(string.Empty);
            }
        }

        [Theory]
        [InlineData("??", "en-US", "CookieRequestCultureProvider contains invalid culture '??'.")]
        [InlineData("en-US", "??", "CookieRequestCultureProvider contains invalid ui-culture '??'.")]
        public async Task ParseInvalidCookieShouldLogWarning(string culture, string uiCulture, string expectedMessage)
        {
            var sink = new TestSink(
                TestSink.EnableWithTypeName<CookieRequestCultureProvider>,
                TestSink.EnableWithTypeName<CookieRequestCultureProvider>);
            var loggerFactory = new TestLoggerFactory(sink, enabled: true);
            var builder = new WebHostBuilder()
                .Configure(app =>
                {
                    var options = new RequestLocalizationOptions
                    {
                        DefaultRequestCulture = new RequestCulture("en-US"),
                        SupportedCultures = new List<CultureInfo>
                        {
                            new CultureInfo("ar-YE")
                        },
                        SupportedUICultures = new List<CultureInfo>
                        {
                            new CultureInfo("ar-YE")
                        }
                    };
                    var provider = new CookieRequestCultureProvider
                    {
                        CookieName = "Preferences"
                    };
                    options.RequestCultureProviders.Insert(0, provider);
                    app.UseRequestLocalization(options);
                    app.Run(context =>
                    {
                        var requestCultureFeature = context.Features.Get<IRequestCultureFeature>();
                        var requestCulture = requestCultureFeature.RequestCulture;
                        Assert.Equal("en-US", requestCulture.Culture.Name);
                        return Task.FromResult(0);
                    });
                })
                .ConfigureServices(services =>
                {
                    services.AddSingleton(typeof(ILoggerFactory), loggerFactory);
                    services.AddSingleton(typeof(ILogger), loggerFactory.CreateLogger<CookieRequestCultureProvider>());
                });

            using (var server = new TestServer(builder))
            {
                var client = server.CreateClient();
                client.DefaultRequestHeaders.Add("Cookie", new CookieHeaderValue("Preferences", $"c={culture}|uic={uiCulture}").ToString());

                var response = await client.GetAsync(string.Empty);
            }

            var logMessages = sink.Writes;

            Assert.Equal(1, logMessages.Count);
            Assert.Equal(LogLevel.Warning, logMessages[0].LogLevel);
            Assert.Equal(expectedMessage, logMessages[0].State.ToString());
        }
    }
}