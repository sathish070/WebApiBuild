using Core_Arca.Services.Interface;
using Microsoft.OpenApi.Models;
using ScantronInterfaceBuild.Services;
using System.Text.Json.Serialization;
using System.Text.Json;
using Core_Arca.Helpers;

namespace Core_Arca
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            builder.WebHost.ConfigureKestrel(c =>
            {
                c.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(5);
            });
            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });

            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddHttpClient("ServiceNowAuthClient", client =>
            {
                client.BaseAddress = new Uri("https://securservdev.service-now.com");
            });
            builder.Services.AddTransient<ServiceNowHelper>();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Arca API", Version = "v2" });
                c.AddSecurityDefinition("basic", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "basic",
                    In = ParameterLocation.Header,
                    Description = "Basic Authorization header [Use the given username and password given by Scantron team]"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "basic"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
                c.EnableAnnotations();
            });

            builder.Services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
            });

            var app = builder.Build();

            if (app.Environment.IsEnvironment("Dev") || app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Test"))
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Arca API - v2");
                });

                app.UseDefaultFiles(new DefaultFilesOptions
                {
                    DefaultFileNames = new List<string> { "index.html" }
                });
                app.UseStaticFiles();
            }
            else
            {
                app.UseDefaultFiles(new DefaultFilesOptions
                {
                    DefaultFileNames = new List<string> { "index.html" }
                });
                app.UseStaticFiles();
            }

            app.UseResponseCompression();
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseAuthorization();

            app.MapControllers();

            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/" || context.Request.Path.ToString().Contains("swagger"))
                {
                    if (!app.Environment.IsProduction())
                    {
                        if (!context.Request.Path.StartsWithSegments("/swagger"))
                        {
                            context.Response.Redirect("/swagger");
                            return;
                        }
                    }
                    else
                    {
                        context.Response.Redirect("/index.html");
                        return;
                    }
                }
                await next();
            });

            app.Run();
        }
    }
}
    