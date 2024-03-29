using System;
using System.IO;
using System.Reflection;
using Archive.WebUI.Services;
using Archive.Application;
using Archive.Application.Common.Interfaces;
using Archive.Application.Common.Options.MongoDb;
using Archive.Core.Collections.Identity;
using AspNetCore.Identity.Mongo;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace Archive.WebUI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddApplication();
            // services.AddInfrastructure(Configuration);

            services.AddControllers().AddNewtonsoftJson(options => 
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);
            
            services.Configure<MongoDbOptions>(Configuration.GetSection("MongoDb"));

            services.AddIdentityMongoDbProvider<ApplicationUser, ApplicationRole,string>(identity =>
                {
                    identity.Password.RequireDigit = true;
                    identity.Password.RequireNonAlphanumeric = false;
                    identity.Password.RequireLowercase = true;
                    identity.Password.RequireUppercase = false;

                    identity.Lockout.AllowedForNewUsers = true;
                    identity.Lockout.MaxFailedAccessAttempts = 3;

                    identity.SignIn.RequireConfirmedAccount = true;
                },
                mongo =>
                {
                    mongo.ConnectionString = Configuration.GetConnectionString("DefaultConnection");
                    mongo.UsersCollection = "users";
                    mongo.RolesCollection = "roles";
                });
            
            services.AddSingleton<ICurrentUserService, CurrentUserService>();

            services.AddSwaggerGen(c =>
            {
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "archive API");
                });
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "client-app")),
                RequestPath = "/client-app"
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });
        }
    }
}