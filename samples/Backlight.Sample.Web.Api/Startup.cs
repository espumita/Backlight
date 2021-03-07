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
            var examsProvider = new ExamsProvider();
            var usersProvider = new UsersProvider();
            var notesProvider = new NotesProvider();

            services.AddBacklight(configuration => {
                configuration.For<Exam>()
                    .AddCreate(examsProvider)
                    .AddRead(examsProvider)
                    .AddUpdate(examsProvider);
                configuration.For<User>()
                    .AddCRUD(usersProvider);
                configuration.For<Note>()
                    .AddRead(notesProvider);
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
                configuration.UrlPath = "back";
            });
        }
    }

}
