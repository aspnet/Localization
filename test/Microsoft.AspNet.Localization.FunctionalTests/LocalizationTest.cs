// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System.Threading.Tasks;
using Microsoft.AspNet.Server.Testing;
using Xunit;

namespace Microsoft.AspNet.Localization.FunctionalTests
{
    public class LocalizationTest
    {
        [Theory]
        [InlineData(RuntimeFlavor.Clr, "http://localhost:5070/")]
        [InlineData(RuntimeFlavor.CoreClr, "http://localhost:5071/")]
        public Task Localization_ResourcesInFolder_ReturnLocalizedValue(
            RuntimeFlavor runtimeFlavor,
            string applicationBaseUrl)
        {
            var testRunner = new TestRunner();
            return testRunner.RunTestAndVerifyResponse(
                runtimeFlavor,
                applicationBaseUrl,
                "ResourcesInFolder",
                "fr-FR",
                "Bonjour from StartupResourcesInFolder Bonjour from Test in resources folder");
        }

        [Theory]
        [InlineData(RuntimeFlavor.Clr, "http://localhost:5072/")]
        [InlineData(RuntimeFlavor.CoreClr, "http://localhost:5073/")]
        public Task Localization_ResourcesAtRootFolder_ReturnLocalizedValue(
            RuntimeFlavor runtimeFlavor,
            string applicationBaseUrl)
        {
            var testRunner = new TestRunner();
            return testRunner.RunTestAndVerifyResponse(
                runtimeFlavor,
                applicationBaseUrl,
                "ResourcesAtRootFolder",
                "fr-FR",
                "Bonjour from StartupResourcesAtRootFolder Bonjour from Test in root folder");
        }
    }
}
