// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http.Features;
using Microsoft.AspNet.Localization;
using Microsoft.AspNet.TestHost;
using Xunit;

namespace Microsoft.Extensions.Localization.Tests
{
    public class AccessLanguageHeaderRequestCultureProviderTest
    {
        [Fact]
        public async void GetFallbackLanguage_ReturnsFirstNonNullCultureFromSupportedCultureList()
        {
            using (var server = TestServer.Create(app =>
            {
                var options = new RequestLocalizationOptions
                {
                    SupportedCultures = new List<CultureInfo>
                    {
                        new CultureInfo("ar-SA"),
                        new CultureInfo("en-US")
                    }
                };
                app.UseRequestLocalization(options);
                app.Run(context =>
                {
                    var requestCultureFeature = context.Features.Get<IRequestCultureFeature>();
                    var requestCulture = requestCultureFeature.RequestCulture;
                    Assert.Equal("ar-SA", requestCulture.Culture.Name);
                    return Task.FromResult(0);
                });
            }))
            {
                var client = server.CreateClient();
                client.DefaultRequestHeaders.AcceptLanguage.ParseAdd("jp,ar-SA,en-US");
                var count = client.DefaultRequestHeaders.AcceptLanguage.Count;
                var response = await client.GetAsync(string.Empty);
                Assert.Equal(3, count);
            }
        }

        [Fact]
        public async void GetFallbackLanguage_ReturnsFromSupportedCulture_AcceptLanguageListContainsSupportedCultures()
        {
            using (var server = TestServer.Create(app =>
            {
                var options = new RequestLocalizationOptions
                {
                    DefaultRequestCulture = new RequestCulture(new CultureInfo("fr-FR")),
                    SupportedCultures = new List<CultureInfo>
                    {
                        new CultureInfo("ar-SA"),
                        new CultureInfo("en-US")
                    }
                };
                app.UseRequestLocalization(options);
                app.Run(context =>
                {
                    var requestCultureFeature = context.Features.Get<IRequestCultureFeature>();
                    var requestCulture = requestCultureFeature.RequestCulture;
                    Assert.Equal("ar-SA", requestCulture.Culture.Name);
                    return Task.FromResult(0);
                });
            }))
            {
                var client = server.CreateClient();
                client.DefaultRequestHeaders.AcceptLanguage.ParseAdd("en-GB,ar-SA,en-US");
                var count = client.DefaultRequestHeaders.AcceptLanguage.Count;
                var response = await client.GetAsync(string.Empty);
            }
        }

        [Fact]
        public async void GetFallbackLanguage_ReturnsDefault_AcceptLanguageListDoesnotContainSupportedCultures()
        {
            using (var server = TestServer.Create(app =>
            {
                var options = new RequestLocalizationOptions
                {
                    DefaultRequestCulture = new RequestCulture(new CultureInfo("fr-FR")),
                    SupportedCultures = new List<CultureInfo>
                    {
                        new CultureInfo("ar-SA"),
                        new CultureInfo("af-ZA")
                    }
                };
                app.UseRequestLocalization(options);
                app.Run(context =>
                {
                    var requestCultureFeature = context.Features.Get<IRequestCultureFeature>();
                    var requestCulture = requestCultureFeature.RequestCulture;
                    Assert.Equal("fr-FR", requestCulture.Culture.Name);
                    return Task.FromResult(0);
                });
            }))
            {
                var client = server.CreateClient();
                client.DefaultRequestHeaders.AcceptLanguage.ParseAdd("en-GB,ar-MA,en-US");
                var count = client.DefaultRequestHeaders.AcceptLanguage.Count;
                var response = await client.GetAsync(string.Empty);
                Assert.Equal(3, count);
            }
        }
        
        [Fact]
        public async void OmitDefaultRequestCultureShouldNotThrowNullReferenceException_And_ShouldGetTheRightCulture()
        {
            using (var server = TestServer.Create(app =>
            {
                var options = new RequestLocalizationOptions()
                {
                    SupportedCultures = new List<CultureInfo>
                    {
                        new CultureInfo("ar-YE")
                    },
                    SupportedUICultures = new List<CultureInfo>
                    {
                        new CultureInfo("ar-YE")
                    }
                };
                app.UseRequestLocalization(options);
                app.Run(context =>
                {
                    var requestCultureFeature = context.Features.Get<IRequestCultureFeature>();
                    var requestCulture = requestCultureFeature.RequestCulture;
                    Assert.Equal("ar-YE", requestCulture.Culture.Name);
                    Assert.Equal("ar-YE", requestCulture.UICulture.Name);
                    return Task.FromResult(0);
                });
            }))
            {
                var client = server.CreateClient();
                client.DefaultRequestHeaders.AcceptLanguage.ParseAdd("en-GB,ar-YE,en-US");
                var count = client.DefaultRequestHeaders.AcceptLanguage.Count;
                var response = await client.GetAsync(string.Empty);
                Assert.Equal(3, count);
            }
        }
        
        [Fact]
        public async void GetRightCultureWhenRequestIsNotSupported_And_DefaultRequestCultureIsNotDefined()
        {
            using (var server = TestServer.Create(app =>
            {
                var options = new RequestLocalizationOptions
                {
                    SupportedCultures = new List<CultureInfo>
                    {
                        new CultureInfo("nl-NL")
                    },
                    SupportedUICultures = new List<CultureInfo>
                    {
                        new CultureInfo("nl-NL")
                    }
                };
                app.UseRequestLocalization(options);
                app.Run(context =>
                {
                    var requestCultureFeature = context.Features.Get<IRequestCultureFeature>();
                    var requestCulture = requestCultureFeature.RequestCulture;
                    Assert.Equal("nl-NL", requestCulture.Culture.Name);
                    Assert.Equal("nl-NL", requestCulture.Culture.Name);
                    return Task.FromResult(0);
                });
            }))
            {
                var client = server.CreateClient();
                client.DefaultRequestHeaders.AcceptLanguage.ParseAdd("ar-SA,nl-NL,en-US");
                var count = client.DefaultRequestHeaders.AcceptLanguage.Count;
                var response = await client.GetAsync(string.Empty);
                Assert.Equal(3, count);
            }
        }
        
        [Fact]
        public async void GetRightCultureWhenRequestIsNotSupported_And_DefaultRequestCultureIsNotSupported()
        {
            using (var server = TestServer.Create(app =>
            {
                var options = new RequestLocalizationOptions
                {
                    SupportedCultures = new List<CultureInfo>
                    {
                        new CultureInfo("nl-NL")
                    },
                    SupportedUICultures = new List<CultureInfo>
                    {
                        new CultureInfo("nl-NL")
                    }
                    , DefaultRequestCulture = new RequestCulture(new CultureInfo("en-AU"), new CultureInfo("en-AU"))
                };
                app.UseRequestLocalization(options);
                app.Run(context =>
                {
                    var requestCultureFeature = context.Features.Get<IRequestCultureFeature>();
                    var requestCulture = requestCultureFeature.RequestCulture;
                    Assert.Equal("nl-NL", requestCulture.Culture.Name);
                    Assert.Equal("nl-NL", requestCulture.Culture.Name);
                    return Task.FromResult(0);
                });
            }))
            {
                var client = server.CreateClient();
                client.DefaultRequestHeaders.AcceptLanguage.ParseAdd("ar-SA,nl-NL,en-US");
                var count = client.DefaultRequestHeaders.AcceptLanguage.Count;
                var response = await client.GetAsync(string.Empty);
                Assert.Equal(3, count);
            }
        }
               
        [Fact]
        public async void GetRightCultureWhenRequestIsNotSupported_And_SupportedCulturesAreNotDefined()
        {
            using (var server = TestServer.Create(app =>
            {
                var options = new RequestLocalizationOptions();
                app.UseRequestLocalization(options);
                app.Run(context =>
                {
                    var requestCultureFeature = context.Features.Get<IRequestCultureFeature>();
                    var requestCulture = requestCultureFeature.RequestCulture;
                    Assert.Equal("ar-SA", requestCulture.Culture.Name);
                    Assert.Equal("ar-SA", requestCulture.Culture.Name);
                    return Task.FromResult(0);
                });
            }))
            {
                var client = server.CreateClient();
                client.DefaultRequestHeaders.AcceptLanguage.ParseAdd("ar-SA,nl-NL,en-US");
                var count = client.DefaultRequestHeaders.AcceptLanguage.Count;
                var response = await client.GetAsync(string.Empty);
                Assert.Equal(3, count);
            }
        }
               
        [Fact]
        public async void GetRightCultureWhenRequestIsNotSupported_And_SupportedCulturesAreEmpty()
        {
            using (var server = TestServer.Create(app =>
            {
                var options = new RequestLocalizationOptions
                {
                    SupportedCultures = new List<CultureInfo>
                    {
                        
                    },
                    SupportedUICultures = new List<CultureInfo>
                    {
                        
                    }
                };
                app.UseRequestLocalization(options);
                app.Run(context =>
                {
                    var requestCultureFeature = context.Features.Get<IRequestCultureFeature>();
                    var requestCulture = requestCultureFeature.RequestCulture;
                    Assert.Equal("en-US", requestCulture.Culture.Name);
                    Assert.Equal("en-US", requestCulture.Culture.Name);
                    return Task.FromResult(0);
                });
            }))
            {
                var client = server.CreateClient();
                client.DefaultRequestHeaders.AcceptLanguage.ParseAdd("ar-SA,nl-NL,en-US");
                var count = client.DefaultRequestHeaders.AcceptLanguage.Count;
                var response = await client.GetAsync(string.Empty);
                Assert.Equal(3, count);
            }
        }
    }
}