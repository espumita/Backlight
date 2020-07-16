using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Backlight.Middleware;
using Backlight.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using NUnit.Framework;

namespace Backlight.Test {
    public class ApiRunnerTests {
        private IApplicationBuilder applicationBuilder;
        private ApiRunner runner;
        private DefaultHttpContext httpContext;
        private IServiceProvider serviceProvider;
        private BacklightProvidersService backlightProvidersService;

        [SetUp]
        public void SetUp() {
            applicationBuilder = Substitute.For<IApplicationBuilder>();
            serviceProvider = applicationBuilder.ApplicationServices = Substitute.For<IServiceProvider>();
            backlightProvidersService = Substitute.For<BacklightProvidersService>(new object[] { null });
            httpContext = new DefaultHttpContext();
            runner = new ApiRunner(applicationBuilder, httpContext);

            serviceProvider.GetService(Arg.Is(typeof(BacklightProvidersService))).Returns(backlightProvidersService);
        }

        [Test, TestCaseSource("NotAllowedMethods")]
        public async Task get_method_not_allowed_response_when_try_to_run_under_a_not_allowed_http_method(string httpMethod) {
            httpContext.Request.Method = httpMethod;

            await runner.Run();

            httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.MethodNotAllowed);
            var responseBody = await ReadBodyFrom(httpContext.Response.Body);
            responseBody.Should().Be("Http method is not allowed");
        }

        [Test, TestCaseSource("AllowedMethods")]
        public async Task get_bad_request_when_the_entity_deserialization_has_an_error(string httpMethod) {
            httpContext.Request.Method = httpMethod;

            await runner.Run();

            httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            var responseBody = await ReadBodyFrom(httpContext.Response.Body);
            responseBody.Should().Be("Entity deserialization error");
        }

        [Test, TestCaseSource("AllowedMethods")]
        public async Task get_bad_request_when_entity_type_is_not_configured(string httpMethod) {
            httpContext.Request.Method = httpMethod;
            var requestBodyStream = await RequestBodyStreamWith(JsonSerializer.Serialize(new BacklightApiRequest { Entity = "Test" }));
            httpContext.Request.Body = requestBodyStream;

            backlightProvidersService.IsEntityConfiguredFor(Arg.Any<string>()).Returns(false);

            await runner.Run();

            httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            var responseBody = await ReadBodyFrom(httpContext.Response.Body);
            responseBody.Should().Be("Entity is not configured");
        }

        public static IEnumerable<string> NotAllowedMethods() {
            yield return HttpMethods.Connect;
            yield return HttpMethods.Head;
            yield return HttpMethods.Options;
            yield return HttpMethods.Patch;
            yield return HttpMethods.Trace;
        }

        public static IEnumerable<string> AllowedMethods() {
            yield return HttpMethods.Put;
            yield return HttpMethods.Get;
            yield return HttpMethods.Post;
            yield return HttpMethods.Delete;
        }

        private static async Task<string> ReadBodyFrom(Stream bodyStream) {
            var memoryStream = new MemoryStream();
            await bodyStream.CopyToAsync(memoryStream);
            var streamReader = new StreamReader(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
            var readToEndAsync = await streamReader.ReadToEndAsync();
            streamReader.Close();
            return readToEndAsync;
        }

        private async Task<Stream> RequestBodyStreamWith(string responseBody) {
            var bodyStream = new MemoryStream();
            var streamWriter = new StreamWriter(bodyStream);
            await streamWriter.WriteAsync(responseBody);
            await streamWriter.FlushAsync();
            bodyStream.Seek(0, SeekOrigin.Begin);
            return bodyStream;
        }
    }
}