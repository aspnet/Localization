// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using Microsoft.AspNetCore.Testing;
using Microsoft.Extensions.Localization.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using Xunit;

namespace Microsoft.Extensions.Localization.Tests
{
    public class ResourceManagerStringLocalizerTest
    {
        [Fact]
        public void EnumeratorCachesCultureWalkForSameAssembly()
        {
            // Arrange        
            var baseName = "Microsoft.Extensions.Localization.Tests.Resources.Test";
            var resourceNamesCache = new ResourceNamesCache();
            var resourceAssembly = new TestAssemblyWrapper();
            var resourceManager = new TestResourceManager(baseName, resourceAssembly.Assembly);
            var logger = Logger;
            var localizer1 = new ResourceManagerStringLocalizer(resourceManager, resourceAssembly, baseName, resourceNamesCache, logger);
            var localizer2 = new ResourceManagerStringLocalizer(resourceManager, resourceAssembly, baseName, resourceNamesCache, logger);

            // Act
            for (int i = 0; i < 5; i++)
            {
                localizer1.GetAllStrings().ToList();
                localizer2.GetAllStrings().ToList();
            }

            // Assert
            var expectedCallCount = GetCultureInfoDepth(CultureInfo.CurrentUICulture);
            Assert.Equal(expectedCallCount, resourceNamesCache.Count);
        }

        [Fact]
        public void EnumeratorCacheIsScopedByAssembly()
        {
            // Arrange
            var baseName = "Microsoft.Extensions.Localization.Tests.Resources.Test";
            var resourceNamesCache1 = new ResourceNamesCache();
            var resourceNamesCache2 = new ResourceNamesCache();
            var resourceAssembly1 = new TestAssemblyWrapper("Assembly1");
            var resourceAssembly2 = new TestAssemblyWrapper("Assembly2");
            var resourceManager1 = new TestResourceManager(baseName, resourceAssembly1.Assembly);
            var resourceManager2 = new TestResourceManager(baseName, resourceAssembly2.Assembly);
            var logger = Logger;
            var localizer1 = new ResourceManagerStringLocalizer(
                resourceManager1,
                resourceAssembly1,
                baseName,
                resourceNamesCache1,
                logger);
            var localizer2 = new ResourceManagerStringLocalizer(
                resourceManager2,
                resourceAssembly2,
                baseName,
                resourceNamesCache2,
                logger);

            // Act
            localizer1.GetAllStrings().ToList();
            localizer2.GetAllStrings().ToList();

            // Assert
            var expectedCallCount = GetCultureInfoDepth(CultureInfo.CurrentUICulture);
            Assert.Equal(expectedCallCount, resourceNamesCache1.Count);
            Assert.Equal(expectedCallCount, resourceNamesCache2.Count);
        }

        [Fact]
        public void GetString_PopulatesSearchedLocationOnLocalizedString()
        {
            // Arrange
            var baseName = "Resources.TestResource";
            var resourceNamesCache = new ResourceNamesCache();
            var resourceAssembly = new TestAssemblyWrapper();
            var resourceManager = new TestResourceManager(baseName, resourceAssembly.Assembly);
            var logger = Logger;
            var localizer = new ResourceManagerStringLocalizer(
                resourceManager,
                resourceAssembly,
                baseName,
                resourceNamesCache,
                logger);

            // Act
            var value = localizer["name"];

            // Assert
            Assert.Equal("Resources.TestResource", value.SearchedLocation);
        }

        [Fact]
        [ReplaceCulture("en-US", "en-US")]
        public void GetString_LogsLocationSearched()
        {
            // Arrange
            var baseName = "Resources.TestResource";
            var resourceNamesCache = new ResourceNamesCache();
            var resourceAssembly = new TestAssemblyWrapper();
            var resourceManager = new TestResourceManager(baseName, resourceAssembly.Assembly);
            var logger = Logger;
            var localizer = new ResourceManagerStringLocalizer(
                resourceManager,
                resourceAssembly,
                baseName,
                resourceNamesCache,
                logger);

            // Act
            var value = localizer["a key!"];

            // Assert
            Assert.Equal(1, Sink.Writes.Count);
            Assert.Equal("ResourceManagerStringLocalizer searched for 'a key!' in 'Resources.TestResource' with culture 'en-US'.", Sink.Writes.First().State.ToString());
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ResourceManagerStringLocalizer_GetAllStrings_ReturnsExpectedValue(bool includeParentCultures)
        {
            // Arrange
            var baseName = "Microsoft.Extensions.Localization.Tests.Resources.Test";
            var resourceNamesCache = new ResourceNamesCache();
            var resourceAssembly = new TestAssemblyWrapper();
            var resourceManager = new TestResourceManager(baseName, resourceAssembly.Assembly);
            var logger = Logger;
            var localizer = new ResourceManagerStringLocalizer(
                resourceManager,
                resourceAssembly,
                baseName,
                resourceNamesCache,
                logger);

            // Act
            var strings = localizer.GetAllStrings(includeParentCultures).ToList();

            // Assert
            Assert.Equal(4, strings.Count);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ResourceManagerStringLocalizer_GetAllStrings_MissingResourceThrows(bool includeParentCultures)
        {
            // Arrange
            var baseName = "Microsoft.Extensions.Localization.Tests.Resources.Testington";
            var resourceNamesCache = new ResourceNamesCache();
            var resourceAssembly = new TestAssemblyWrapper("Assembly1");
            var resourceManager = new TestResourceManager(baseName, resourceAssembly.Assembly);
            var logger = Logger;
            var localizer = new ResourceManagerWithCultureStringLocalizer(
                resourceManager,
                resourceAssembly.Assembly,
                baseName,
                resourceNamesCache,
                CultureInfo.CurrentCulture,
                logger);

            // Act & Assert
            var exception = Assert.Throws<MissingManifestResourceException>(() =>
            {
                localizer.GetAllStrings(includeParentCultures).ToList();
            });
            var expected = includeParentCultures
                ? "No manifests exist for the current culture."
                : $"Could not find any resources appropriate for the specified culture or the neutral culture.  Make sure \"{baseName}.resources\" was correctly embedded or linked into assembly \"{GetType().Assembly.GetName().Name}\" at compile time, or that all the satellite assemblies required are loadable and fully signed.";
            Assert.Equal(expected, exception.Message);
        }

        private static int GetCultureInfoDepth(CultureInfo culture)
        {
            var result = 0;
            var currentCulture = culture;

            while (true)
            {
                result++;

                if (currentCulture == currentCulture.Parent)
                {
                    break;
                }

                currentCulture = currentCulture.Parent;
            }

            return result;
        }


        private TestSink Sink { get; } = new TestSink();

        private ILogger Logger
        {
            get
            {
                return new TestLoggerFactory(Sink, true).CreateLogger<ResourceManagerStringLocalizer>();
            }
        }

        public class TestResourceManager : ResourceManager
        {
            public TestResourceManager(string baseName, Assembly assembly)
                : base(baseName, assembly)
            {
            }

            public override string GetString(string name, CultureInfo culture) => null;
        }

        public class TestAssemblyWrapper : AssemblyWrapper
        {
            public TestAssemblyWrapper(string name = nameof(TestAssemblyWrapper))
                : base(typeof(TestAssemblyWrapper).GetTypeInfo().Assembly)
            {
                FullName = name;
            }

            //public int GetManifestResourceStreamCallCount { get; private set; }

            public override string FullName { get; }

            //public override Stream GetManifestResourceStream(string name)
            //{
            //    GetManifestResourceStreamCallCount++;
            //    return MakeResourceStream();
            //}
        }
    }
}
