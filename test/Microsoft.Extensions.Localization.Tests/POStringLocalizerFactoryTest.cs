// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System.Reflection;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Microsoft.Extensions.Localization
{
    public class POStringLocalizerFactoryTest
    {
        [Fact]
        public void Create_Type()
        {
            // Arrange
            var options = new LocalizationOptions();
            options.ResourcesPath = "POFiles";
            var localizationOptions = new Mock<IOptions<LocalizationOptions>>();
            localizationOptions.Setup(o => o.Value).Returns(options);

            var factory = new POStringLocalizerFactory(localizationOptions.Object);

            // Act
            var localizer = factory.Create(typeof(POStringLocalizerFactoryTest));

            // Assert
            Assert.Equal("value", localizer["key"]);
        }

        [Fact]
        public void Create_BaseName()
        {
            // Arrange
            var options = new LocalizationOptions();
            options.ResourcesPath = "POFiles";
            var localizationOptions = new Mock<IOptions<LocalizationOptions>>();
            localizationOptions.Setup(o => o.Value).Returns(options);

            var factory = new POStringLocalizerFactory(localizationOptions.Object);

            // Act
            var localizer = factory.Create(
                nameof(POStringLocalizerFactoryTest),
                typeof(POStringLocalizerFactoryTest).GetTypeInfo().Assembly.GetName().Name);

            // Assert
            Assert.Equal("value", localizer["key"]);
        }
    }
}
