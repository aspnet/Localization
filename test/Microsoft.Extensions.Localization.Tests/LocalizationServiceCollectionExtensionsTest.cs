// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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
            LocalizationServiceCollectionExtensions.AddLocalizationServices(collection);

            // Assert
            var services = collection.ToList();
            Assert.Equal(4, services.Count);

            Assert.Collection(services,
                service =>
                {
                    Assert.Equal(typeof(IOptions<>), service.ServiceType);
                    Assert.Equal(typeof(OptionsManager<>), service.ImplementationType);
                    Assert.Equal(ServiceLifetime.Singleton, service.Lifetime);
                },
                service =>
                {
                    Assert.Equal(typeof(IOptionsMonitor<>), service.ServiceType);
                    Assert.Equal(typeof(OptionsMonitor<>), service.ImplementationType);
                    Assert.Equal(ServiceLifetime.Singleton, service.Lifetime);
                },
                service =>
                {
                    Assert.Equal(typeof(IStringLocalizerFactory), service.ServiceType);
                    Assert.Equal(typeof(ResourceManagerStringLocalizerFactory), service.ImplementationType);
                    Assert.Equal(ServiceLifetime.Singleton, service.Lifetime);
                },
                service =>
                {
                    Assert.Equal(typeof(IStringLocalizer<>), service.ServiceType);
                    Assert.Equal(typeof(StringLocalizer<>), service.ImplementationType);
                    Assert.Equal(ServiceLifetime.Transient, service.Lifetime);
                });
        }

        [Fact]
        public void AddLocalizationWithLocalizationOptions_AddsNeededServices()
        {
            // Arrange
            var collection = new ServiceCollection();

            // Act
            LocalizationServiceCollectionExtensions.AddLocalizationServices(
                collection,
                options => options.ResourcesPath = "Resources");

            // Assert
            var services = collection.ToList();
            Assert.Equal(5, services.Count);

            Assert.Collection(services,
                 service =>
                 {
                     Assert.Equal(typeof(IOptions<>), service.ServiceType);
                     Assert.Equal(typeof(OptionsManager<>), service.ImplementationType);
                     Assert.Equal(ServiceLifetime.Singleton, service.Lifetime);
                 },
                 service =>
                 {
                     Assert.Equal(typeof(IOptionsMonitor<>), service.ServiceType);
                     Assert.Equal(typeof(OptionsMonitor<>), service.ImplementationType);
                     Assert.Equal(ServiceLifetime.Singleton, service.Lifetime);
                 },
                 service =>
                 {
                     Assert.Equal(typeof(IStringLocalizerFactory), service.ServiceType);
                     Assert.Equal(typeof(ResourceManagerStringLocalizerFactory), service.ImplementationType);
                     Assert.Equal(ServiceLifetime.Singleton, service.Lifetime);
                 },
                 service =>
                 {
                     Assert.Equal(typeof(IStringLocalizer<>), service.ServiceType);
                     Assert.Equal(typeof(StringLocalizer<>), service.ImplementationType);
                     Assert.Equal(ServiceLifetime.Transient, service.Lifetime);
                 },
                 service =>
                 {
                     Assert.Equal(typeof(IConfigureOptions<LocalizationOptions>), service.ServiceType);
                     Assert.Equal(ServiceLifetime.Singleton, service.Lifetime);
                 });

        }
    }
}
