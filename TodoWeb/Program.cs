#region Copyright
//=======================================================================================
// Microsoft 
//
// This sample is supplemental to the technical guidance published on my personal
// blog at https://github.com/paolosalvatori. 
// 
// Author: Paolo Salvatori
//=======================================================================================
// Copyright (c) Microsoft Corporation. All rights reserved.
// 
// LICENSED UNDER THE APACHE LICENSE, VERSION 2.0 (THE "LICENSE"); YOU MAY NOT USE THESE 
// FILES EXCEPT IN COMPLIANCE WITH THE LICENSE. YOU MAY OBTAIN A COPY OF THE LICENSE AT 
// http://www.apache.org/licenses/LICENSE-2.0
// UNLESS REQUIRED BY APPLICABLE LAW OR AGREED TO IN WRITING, SOFTWARE DISTRIBUTED UNDER THE 
// LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY 
// KIND, EITHER EXPRESS OR IMPLIED. SEE THE LICENSE FOR THE SPECIFIC LANGUAGE GOVERNING 
// PERMISSIONS AND LIMITATIONS UNDER THE LICENSE.
//=======================================================================================
#endregion

#region Using Directives
using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
#endregion

namespace TodoWeb
{
    public class Program
    {
        #region Public Static Methods
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            var builder = WebHost.CreateDefaultBuilder(args)
                .UseApplicationInsights()
                .CaptureStartupErrors(true)
                .UseSetting(WebHostDefaults.DetailedErrorsKey, "true")
                .ConfigureAppConfiguration(ConfigConfiguration)
                .UseStartup<Startup>()
                .Build();

            return builder;
        }
        #endregion

        #region Private Static Methods
        private static void ConfigConfiguration(WebHostBuilderContext webHostBuilderContext, IConfigurationBuilder configurationBuilder)
        {
            var configuration = configurationBuilder.Build();

            // Read the name of the environment variable set by Service Fabric that contain the location of the PEM file
            var certificateEnvironmentVariable = configuration["AzureKeyVault:Certificate:CertificateEnvironmentVariable"];
            if (string.IsNullOrWhiteSpace(certificateEnvironmentVariable))
            {
                return;
            }

            // Read the name of the environment variable set by Service Fabric that contain the location of the KEY file
            var keyEnvironmentVariable = configuration["AzureKeyVault:Certificate:KeyEnvironmentVariable"];
            if (string.IsNullOrWhiteSpace(keyEnvironmentVariable))
            {
                return;
            }

            // Read the client ID
            var clientId = configuration["AzureKeyVault:ClientId"];
            if (string.IsNullOrWhiteSpace(clientId))
            {
                return;
            }

            // Read the name of the Azure Key Vault
            var name = configuration["AzureKeyVault:Name"];
            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            // Read the location of the certificate file from the environment variable
            var certificateFilePath = Environment.GetEnvironmentVariable(certificateEnvironmentVariable);
            if (string.IsNullOrWhiteSpace(certificateFilePath))
            {
                return;
            }

            // Read the location of the key file from the environment variable
            var keyFilePath = Environment.GetEnvironmentVariable(keyEnvironmentVariable);
            if (string.IsNullOrWhiteSpace(keyFilePath))
            {
                return;
            }

            // Read the certificate used to authenticate against Azure Key Vault
            var certificate = Helpers.CertificateHelper.GetCertificateAsync(certificateFilePath, keyFilePath).Result;
            if (certificate == null)
            {
                return;
            }

            // Configure the application to read settings from Azure Key Vault
            configurationBuilder.AddAzureKeyVault($"https://{name}.vault.azure.net/",
                                                  clientId,
                                                  certificate);
        }
        #endregion
    }
}
