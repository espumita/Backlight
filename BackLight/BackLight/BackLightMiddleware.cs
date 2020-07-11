using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BackLight {
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
}