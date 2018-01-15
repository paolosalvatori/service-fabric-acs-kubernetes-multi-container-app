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

#region MyRegion Using References
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using Swashbuckle.AspNetCore.Swagger;
using TodoApi.Models;
using TodoApi.Services;
#endregion

namespace TodoApi
{
    /// <summary>
    /// Startup class
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Creates an instance of the Startup class
        /// </summary>
        /// <param name="configuration">The configuration created by the CreateDefaultBuilder.</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Gets or sets the Configuration property.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Get or sets the CloudConfigurationManager
        /// </summary>
        public static object CloudConfigurationManager { get; private set; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">The services collection.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.Configure<RepositoryServiceOptions>(Configuration.GetSection("RepositoryService"));
            services.Configure<NotificationServiceOptions>(Configuration.GetSection("NotificationService"));
            services.Configure<Models.DataProtectionOptions>(Configuration.GetSection("DataProtectionService"));
            services.AddDataProtection().PersistKeysToAzureBlobStorage(GetKeyBlob());
            services.AddApplicationInsightsTelemetry(Configuration);
            services.AddMvc();
            services.AddDbContext<TodoContext>(opt => opt.UseInMemoryDatabase("TodoList"));
            services.AddSingleton<INotificationService, ServiceBusNotificationService>();
            services.AddSingleton<IRepositoryService<TodoItem>, CosmosDbRepositoryService<TodoItem>>();

            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "ToDo API",
                    Description = "A simple example ASP.NET Core Web API",
                    TermsOfService = "None",
                    Contact = new Contact { Name = "Paolo Salvatori", Email = "paolos@microsoft.com", Url = "https://github.com/paolosalvatori" },
                    License = new License { Name = "Use under license", Url = "https://github.com/paolosalvatori/ServiceBusExplorer/blob/develop/LICENSE.txt" }
                });

                // Set the comments path for the Swagger JSON and UI.
                //var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                //var xmlPath = Path.Combine(basePath, "TodoApi.xml");
                //c.IncludeXmlComments(xmlPath);
            });
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="applicationBuilder">ApplicationBuilder paremeter.</param>
        /// <param name="hostingEnvironment">HostingEnvironment parameter.</param>
        /// <param name="loggerFactory">loggerFactory parameter.</param>
        /// <param name="serviceProvider">serviceProvider parameter.</param>
        public void Configure(IApplicationBuilder applicationBuilder,
                              IHostingEnvironment hostingEnvironment,
                              ILoggerFactory loggerFactory,
                              IServiceProvider serviceProvider)
        {
            // Add EventSource logger
            loggerFactory.AddEventSourceLogger();

            // Add Application Insights logger
            loggerFactory.AddApplicationInsights(serviceProvider, Microsoft.Extensions.Logging.LogLevel.Information);

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            applicationBuilder.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            applicationBuilder.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ToDo API V1");
            });

            applicationBuilder.UseMvc();
        }

        /// <summary>
        /// Get the blob reference where to store the data protection key
        /// </summary>
        /// <returns>blob used for the data protection key</returns>
        private CloudBlockBlob GetKeyBlob()
        {
            // Validation
            var connectionString = Configuration["DataProtection:BlobStorage:ConnectionString"];

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException("No connection string is defined in the configuration of the Data Protection service in the appsettings.json.");
            }

            var containerName = Configuration["DataProtection:BlobStorage:ContainerName"];

            if (string.IsNullOrWhiteSpace(containerName))
            {
                throw new ArgumentNullException("No container name is defined in the configuration of the Data Protection service in the appsettings.json.");
            }

            //Parse the connection string and return a reference to the storage account.
            var storageAccount = CloudStorageAccount.Parse(connectionString);

            //Create the blob client object.
            var blobClient = storageAccount.CreateCloudBlobClient();

            //Get a reference to a container to use for the sample code, and create it if it does not exist.
            var container = blobClient.GetContainerReference(containerName);
            container.CreateIfNotExistsAsync();

            //Get a reference to a blob within the container.
            return container.GetBlockBlobReference("todoapikey");
        }
    }
}
