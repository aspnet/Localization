// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Xunit;

namespace Microsoft.Extensions.Localization.Tests
{
    public class ResourceLocalizationAttributeTest
    {
        [Fact]
        public void InvalidResourceLocation_ThrowsArgumentException()
        {
            // Arrange
            var resourceLocation = "<InvalidResourceLocation>";
            var expectedMessage = Resources.Exception_InvalidResourceLocation + Environment.NewLine + "Parameter name: resourceLocation";

            // Assert
            var exception = Assert.Throws<ArgumentException>(() => {
                // Act
                var attribute = new ResourceLocationAttribute(resourceLocation);
            });
            Assert.Equal(expectedMessage, exception.Message);
        }
    }
}
