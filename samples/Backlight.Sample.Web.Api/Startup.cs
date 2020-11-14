using Backlight.Middleware;
using Backlight.Sample.Web.Api.Entities;
using Backlight.Sample.Web.Api.Providers;
using Backlight.Services;
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
            var exampleEntityProvider = new ExamProvider();
            var exampleEntity2Provider = new UserProvider();
            var exampleEntity3Provider = new NotesProvider();

            services.AddBacklight(configuration => {
                configuration.For<Exam>()
                    .AddCreate(exampleEntityProvider)
                    .AddRead(exampleEntityProvider)
                    .AddUpdate(exampleEntityProvider);
                configuration.For<User>()
                    .AddCRUD(exampleEntity2Provider);
                configuration.For<Note>()
                    .AddRead(exampleEntity3Provider);
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors(builder => {
                builder.AllowAnyHeader();
                builder.AllowAnyMethod();
                builder.AllowAnyOrigin();
            });
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
            app.UseBacklight(configuration => {
                configuration.RoutePrefix = "back";
            });
        }
    }

}
