using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BackLight.Sample.Web.Api {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services) {
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseBackLight();
            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
        }
    }

    public class BackLightMiddleware {
        private readonly RequestDelegate next;
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly ILoggerFactory loggerFactory;
        private readonly BackLightOptions backLightOptions;

        public BackLightMiddleware(RequestDelegate next, IWebHostEnvironment hostingEnvironment, ILoggerFactory loggerFactory, BackLightOptions backLightOptions) {
            this.next = next;
            this.hostingEnvironment = hostingEnvironment;
            this.loggerFactory = loggerFactory;
            this.backLightOptions = backLightOptions;
        }

        public async Task Invoke(HttpContext httpContext) {
            
            await next.Invoke(httpContext);
        }

    }

    public static class BackLightBuilderExtensions {
            public static IApplicationBuilder UseBackLight(this IApplicationBuilder applicationBuilder,  Action<BackLightOptions> setupAction = null) {
                var options = new BackLightOptions();
                if (setupAction != null) {
                    setupAction(options);
                } else {
                    options = applicationBuilder.ApplicationServices.GetRequiredService<IOptions<BackLightOptions>>().Value;
                }

                applicationBuilder.UseMiddleware<BackLightMiddleware>(options);

                return applicationBuilder;
            }
    }

    public class BackLightOptions { }
}
