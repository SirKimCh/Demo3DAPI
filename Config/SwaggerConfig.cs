using Microsoft.OpenApi.Models;

namespace Demo3DAPI.Config
{
    public static class SwaggerConfig
    {
        public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Demo3D API",
                    Version = "v1",
                    Description = "API for managing Player Accounts and Characters"
                });

                c.EnableAnnotations();
            });

            return services;
        }

        public static IApplicationBuilder UseSwaggerConfiguration(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Demo3D API V1");
                c.RoutePrefix = "swagger";
                c.DocumentTitle = "Demo3D API Documentation";
            });

            return app;
        }
    }
}

