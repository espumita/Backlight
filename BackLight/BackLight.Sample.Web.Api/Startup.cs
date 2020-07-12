using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Backlight.Sample.Web.Api {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services) {
            services.AddControllers();
            var exampleEntityProvider = new ExampleEntityProvider();
            var exampleEntit2Provider = new ExampleEntity2Provider();
            var exampleEntity3Provider = new ExampleEntity3Provider();

            services.AddBacklight(options => {
                options.For<ExampleEntity>()
                    .AddCreate(exampleEntityProvider)
                    .AddRead(exampleEntityProvider)
                    .AddUpdate(exampleEntityProvider);
                options.For<ExampleEntity2>()
                    .AddCRUD(exampleEntit2Provider);
                options.For<ExampleEntity3>()
                    .AddRead(exampleEntity3Provider);
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseBacklight(configuration => {
                configuration.RoutePrefix = "back";
            });
            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
        }
    }

}
