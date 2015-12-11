// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.OptionsModel;
using Xunit;

namespace Microsoft.Extensions.Localization.Test
{
    public class LocalizationServiceCollectionExtensionsTest
    {
        [Fact]
        public void AddLocalization_AddsNeededServices()
        {
            // Arrange
            var collection = new ServiceCollection();

            // Act
            collection.AddLocalization();

            // Assert
            var services = collection.ToList();
            Assert.Collection(
                services,
                element =>
                {
                    Assert.Equal(typeof(IStringLocalizerFactory), element.ServiceType);
                    Assert.Equal(typeof(ResourceManagerStringLocalizerFactory), element.ImplementationType);
                    Assert.Equal(ServiceLifetime.Singleton, element.Lifetime);
                },
                element =>
                {
                    Assert.Equal(typeof(IStringLocalizer<>), element.ServiceType);
                    Assert.Equal(typeof(StringLocalizer<>), element.ImplementationType);
                    Assert.Equal(ServiceLifetime.Transient, element.Lifetime);
                },
                element =>
                {
                    Assert.Equal(typeof(IOptions<>), element.ServiceType);
                    Assert.Equal(ServiceLifetime.Singleton, element.Lifetime);
                },
                element =>
                {
                    Assert.Equal(typeof(IOptionsMonitor<>), element.ServiceType);
                    Assert.Equal(ServiceLifetime.Singleton, element.Lifetime);
                });
        }

        [Fact]
        public void AddLocalizationWithLocalizationOptions_AddsNeededServices()
        {
            // Arrange
            var collection = new ServiceCollection();

            // Act
            collection.AddLocalization(options => options.ResourcesPath = "Resources");

            // Assert
            var services = collection.ToList();
            Assert.Collection(
                services,
                element =>
                {
                    Assert.Equal(typeof(IStringLocalizerFactory), element.ServiceType);
                    Assert.Equal(typeof(ResourceManagerStringLocalizerFactory), element.ImplementationType);
                    Assert.Equal(ServiceLifetime.Singleton, element.Lifetime);
                },
                element =>
                {
                    Assert.Equal(typeof(IStringLocalizer<>), element.ServiceType);
                    Assert.Equal(typeof(StringLocalizer<>), element.ImplementationType);
                    Assert.Equal(ServiceLifetime.Transient, element.Lifetime);
                },
                element =>
                {
                    Assert.Equal(typeof(IConfigureOptions<LocalizationOptions>), element.ServiceType);
                    Assert.Equal(ServiceLifetime.Singleton, element.Lifetime);
                },
                element =>
                {
                    Assert.Equal(typeof(IOptions<>), element.ServiceType);
                    Assert.Equal(ServiceLifetime.Singleton, element.Lifetime);
                },
                element =>
                {
                    Assert.Equal(typeof(IOptionsMonitor<>), element.ServiceType);
                    Assert.Equal(ServiceLifetime.Singleton, element.Lifetime);
                });
        }
    }
}
