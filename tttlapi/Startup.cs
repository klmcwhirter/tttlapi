#pragma warning disable CS1572, CS1573, CS1591
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using Swashbuckle.AspNetCore.Swagger;
using tttlapi.Repositories;
using tttlapi.Services;
using tttlapi.Strategies;
using tttlapi.Utils;

namespace tttlapi
{
    public class Startup
    {

        public IConfigurationRoot Configuration { get; }
        public IContainer Container { get; private set; }

        ILogger Logger { get; set; }

        public Startup(IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            Logger = loggerFactory.CreateLogger<Startup>();

            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // Adds services required for using options.
            services.AddOptions();

            // Add CORS support
            services.AddCors();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "Tic Tac Toe learning API",
                    Description = "An API to support Tic Tac Toe learning written in ASP.NET Core",
                    TermsOfService = "None",
                    Contact = new Contact { Name = "Kevin McWhirter", Email = "", Url = "https://github.com/klmcwhirter/tttlapi" },
                    License = new License { Name = "Use under MIT", Url = "https://github.com/klmcwhirter/tttlapi/blob/master/LICENSE" }
                });

                // Set the comments path for the Swagger JSON and UI.
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var xmlPath = Path.Combine(basePath, "tttlapi.xml");
                c.IncludeXmlComments(xmlPath);
            });

            Container = AddToAutofac(services);

            var rc = new AutofacServiceProvider(Container);
            return rc;
        }

        protected IContainer AddToAutofac(IServiceCollection services)
        {
            var builder = new ContainerBuilder();

            builder.RegisterRedis(Configuration);
            builder.RegisterRepositories(Configuration);
            builder.RegisterServices();
            builder.RegisterStrategies();

            builder.Populate(services);
            var rc = builder.Build();
            return rc;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            app.UseCors(builder =>
            {
                var originsString = Configuration.GetValue<string>("TTTLAPI_SERVICE_ORIGINS") ?? "http://tictactoelearning";
                Logger.LogInformation($"Configured to use these CORS origins={originsString}");

                var origins = originsString.Split(',', StringSplitOptions.RemoveEmptyEntries);
                builder.WithOrigins(origins);

                builder.WithMethods("GET", "POST", "PUT", "DELETE", "PATCH");

                builder.AllowAnyHeader();

                builder.AllowCredentials();
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();


            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tic Tac Toe learning API V1");
                c.EnableDeepLinking();
            });
        }
    }
}
