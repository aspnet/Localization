﻿// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System.Threading.Tasks;
using Microsoft.AspNet.Server.Testing;
using Microsoft.AspNet.Testing.xunit;
using Xunit;

namespace Microsoft.AspNet.Localization.FunctionalTests
{
    public class LocalizationTest
    {
        [ConditionalTheory]
        [OSSkipCondition(OperatingSystems.Linux | OperatingSystems.MacOSX)]
        [InlineData(RuntimeFlavor.Clr, "http://localhost:5070/", RuntimeArchitecture.x86)]
        [InlineData(RuntimeFlavor.CoreClr, "http://localhost:5071/", RuntimeArchitecture.x86)]
        public Task Localization_ResourcesInFolder_ReturnLocalizedValue_Windows(
            RuntimeFlavor runtimeFlavor,
            string applicationBaseUrl,
            string architecture)
        {
            var testRunner = new TestRunner();
            return testRunner.RunTestAndVerifyResponse(
                runtimeFlavor,
                applicationBaseUrl,
                "ResourcesInFolder",
                "fr-FR",
                "Bonjour from StartupResourcesInFolder Bonjour from Test in resources folder");
        }

        [ConditionalTheory]
        [OSSkipCondition(OperatingSystems.Win7And2008R2)]
        [FrameworkSkipCondition(RuntimeFrameworks.CLR)]
        [InlineData(RuntimeFlavor.Mono, "http://localhost:5072/", RuntimeArchitecture.x86)]
        [InlineData(RuntimeFlavor.CoreClr, "http://localhost:5073/", RuntimeArchitecture.x64)]
        public Task Localization_ResourcesInFolder_ReturnLocalizedValue_Mono(
            RuntimeFlavor runtimeFlavor,
            string applicationBaseUrl,
            string architecture)
        {
            var testRunner = new TestRunner();
            return testRunner.RunTestAndVerifyResponse(
                runtimeFlavor,
                applicationBaseUrl,
                "ResourcesInFolder",
                "fr-FR",
                "Bonjour from StartupResourcesInFolder Bonjour from Test in resources folder");
        }

        [ConditionalTheory]
        [OSSkipCondition(OperatingSystems.Linux | OperatingSystems.MacOSX)]
        [InlineData(RuntimeFlavor.Clr, "http://localhost:5074/", RuntimeArchitecture.x86)]
        [InlineData(RuntimeFlavor.CoreClr, "http://localhost:5075/", RuntimeArchitecture.x86)]
        public Task Localization_ResourcesAtRootFolder_ReturnLocalizedValue_Windows(
            RuntimeFlavor runtimeFlavor,
            string applicationBaseUrl,
            string architecture)
        {
            var testRunner = new TestRunner();
            return testRunner.RunTestAndVerifyResponse(
                runtimeFlavor,
                applicationBaseUrl,
                "ResourcesAtRootFolder",
                "fr-FR",
                "Bonjour from StartupResourcesAtRootFolder Bonjour from Test in root folder");
        }

        [ConditionalTheory]
        [OSSkipCondition(OperatingSystems.Win7And2008R2)]
        [FrameworkSkipCondition(RuntimeFrameworks.CLR)]
        [InlineData(RuntimeFlavor.Mono, "http://localhost:5076/", RuntimeArchitecture.x86)]
        [InlineData(RuntimeFlavor.CoreClr, "http://localhost:5077/", RuntimeArchitecture.x64)]
        public Task Localization_ResourcesAtRootFolder_ReturnLocalizedValue_Mono(
            RuntimeFlavor runtimeFlavor,
            string applicationBaseUrl,
            string architecture)
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
