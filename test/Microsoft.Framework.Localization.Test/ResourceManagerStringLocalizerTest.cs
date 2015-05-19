// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using Moq;
using Xunit;

namespace Microsoft.Framework.Localization.Test
{
    public class ResourceManagerStringLocalizerTest
    {
        [Fact]
        public void EnumeratorCachesCultureWalk()
        {
            // Arrange
            var resourceManager = new Mock<ResourceManager>();
            var resourceAssembly = new Mock<TestAssembly>();
            resourceAssembly.Setup(rm => rm.GetManifestResourceStream(It.IsAny<string>()))
                .Returns(() => MakeResourceStream());
            var baseName = "test";
            var localizer1 = new ResourceManagerStringLocalizer(
                resourceManager.Object,
                resourceAssembly.Object,
                baseName);
            var localizer2 = new ResourceManagerStringLocalizer(
                resourceManager.Object,
                resourceAssembly.Object,
                baseName);

            // Act
            for (int i = 0; i < 5; i++)
            {
                localizer1.ToList();
                localizer2.ToList();
            }

            // Assert
            var expectedCallCount = GetCultureInfoDepth(CultureInfo.CurrentUICulture);
            resourceAssembly.Verify(
                rm => rm.GetManifestResourceStream(It.IsAny<string>()),
                Times.Exactly(expectedCallCount));
        }

        private static Stream MakeResourceStream()
        {
            var stream = new MemoryStream();
            var resourceWriter = new ResourceWriter(stream);            
            resourceWriter.AddResource("TestName", "value");
            resourceWriter.Generate();
            stream.Position = 0;
            return stream;
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

        public class TestAssembly : Assembly
        {

        }
    }
}
