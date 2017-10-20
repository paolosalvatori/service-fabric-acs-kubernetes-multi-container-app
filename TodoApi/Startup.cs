using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using TodoApi.Models;
using TodoApi.Services;
using Microsoft.Extensions.PlatformAbstractions;
using System.IO;
using Microsoft.Extensions.Logging;
using System;

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
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">The services collection.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.Configure<RepositoryServiceOptions>(Configuration.GetSection("RepositoryService"));
            services.Configure<NotificationServiceOptions>(Configuration.GetSection("NotificationService"));
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
            loggerFactory.AddApplicationInsights(serviceProvider, LogLevel.Information);

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            applicationBuilder.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            applicationBuilder.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ToDo API V1");
            });

            applicationBuilder.UseMvc();
        }
    }
}
