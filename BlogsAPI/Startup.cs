using BlogsAPI.Helpers;
using Lynear.API.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogsAPI
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
            services.AddCors(options =>
            {
                options.AddPolicy(name: "BlogsAPIPolicy",
                        builder =>
                        {
                            builder.AllowAnyOrigin()
                                       .AllowAnyMethod()
                                       .AllowAnyHeader();

                        });
            });

            services.AddControllers();

            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            // configure jwt authentication
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            object p = services.AddSwaggerGen(c =>
            {


                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "BLOGS APIs",
                    Description = "BLOGS APIs",

                });

                c.OperationFilter<AuthResponsesOperationFilter>();
                var securitySchema = new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },

                };

                var filePath = Path.Combine(Directory.GetCurrentDirectory(), Path.Combine(Directory.GetCurrentDirectory(), Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "XML"), "API.xml");

                c.IncludeXmlComments(filePath);
                c.AddSecurityDefinition("Bearer", securitySchema);
                c.EnableAnnotations();
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseHttpsRedirection();

            app.UseRouting();

            // global cors policy
            app.UseCors("BlogsAPIPolicy");

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), Path.Combine(Directory.GetCurrentDirectory(), Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "swagger-ui"), "custom.css");

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(Configuration["Swagger:Path"], Configuration["Swagger:Title"]);
                c.RoutePrefix = string.Empty;
                c.InjectStylesheet(filePath);

            });
        }
    }
}
