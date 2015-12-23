﻿// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNet.Server.Testing;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Microsoft.AspNet.Localization.FunctionalTests
{
    public class TestRunner
    {
        private string _applicationPath;
        
        public TestRunner(string applicationPath){
            _applicationPath = applicationPath;
        }
        
        public async Task RunTestAndVerifyResponse(
            RuntimeFlavor runtimeFlavor,
            RuntimeArchitecture runtimeArchitechture,
            string applicationBaseUrl,
            string environmentName,
            string locale,
            string expectedText)
        {
            var logger = new LoggerFactory()
                            .AddConsole()
                            .CreateLogger(string.Format("Localization Test Site:{0}:{1}:{2}", ServerType.Kestrel, runtimeFlavor, runtimeArchitechture));

            using (logger.BeginScope("LocalizationTest"))
            {
                var deploymentParameters = new DeploymentParameters(_applicationPath, ServerType.Kestrel, runtimeFlavor, runtimeArchitechture)
                {
                    ApplicationBaseUriHint = applicationBaseUrl,
                    Command = "web",
                    EnvironmentName = environmentName
                };

                using (var deployer = ApplicationDeployerFactory.Create(deploymentParameters, logger))
                {
                    var deploymentResult = deployer.Deploy();

                    var cookie = new Cookie("ASPNET_CULTURE", "c=" + locale + "|uic=" + locale);
                    var cookieContainer = new CookieContainer();
                    cookieContainer.Add(new Uri(deploymentResult.ApplicationBaseUri), cookie);

                    var httpClientHandler = new HttpClientHandler();
                    httpClientHandler.CookieContainer = cookieContainer;

                    var httpClient = new HttpClient(httpClientHandler) { BaseAddress = new Uri(deploymentResult.ApplicationBaseUri) };

                    // Request to base address and check if various parts of the body are rendered & measure the cold startup time.
                    var response = await RetryHelper.RetryRequest(() =>
                    {
                        return httpClient.GetAsync(string.Empty);
                    }, logger, deploymentResult.HostShutdownToken);

                    var responseText = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Response Text " + responseText);
                    Assert.Equal(expectedText, responseText);
                }
            }
        }
        
        public async Task RunTestAndVerifyResponseHeading(
            RuntimeFlavor runtimeFlavor,
            RuntimeArchitecture runtimeArchitechture,
            string applicationBaseUrl,
            string environmentName,
            string locale,
            string expectedHeadingText)
        {
            var logger = new LoggerFactory()
                            .AddConsole()
                            .CreateLogger(string.Format("Localization Test Site:{0}:{1}:{2}", ServerType.Kestrel, runtimeFlavor, runtimeArchitechture));

            using (logger.BeginScope("LocalizationTest"))
            {
                var deploymentParameters = new DeploymentParameters(_applicationPath, ServerType.Kestrel, runtimeFlavor, runtimeArchitechture)
                {
                    ApplicationBaseUriHint = applicationBaseUrl,
                    Command = "web",
                    EnvironmentName = environmentName
                };

                using (var deployer = ApplicationDeployerFactory.Create(deploymentParameters, logger))
                {
                    var deploymentResult = deployer.Deploy();

                    var cookie = new Cookie("ASPNET_CULTURE", "c=" + locale + "|uic=" + locale);
                    var cookieContainer = new CookieContainer();
                    cookieContainer.Add(new Uri(deploymentResult.ApplicationBaseUri), cookie);

                    var httpClientHandler = new HttpClientHandler();
                    httpClientHandler.CookieContainer = cookieContainer;

                    var httpClient = new HttpClient(httpClientHandler) { BaseAddress = new Uri(deploymentResult.ApplicationBaseUri) };

                    var response = await RetryHelper.RetryRequest(() =>
                    {
                        return httpClient.GetAsync(string.Empty);
                    }, logger, deploymentResult.HostShutdownToken);

                    var responseText = await response.Content.ReadAsStringAsync();
                    var headingIndex = responseText.IndexOf(expectedHeadingText);
                    Console.WriteLine("Response Header " + responseText);
                    Assert.True(headingIndex >= 0);
                }
            }
        }
    }
}
