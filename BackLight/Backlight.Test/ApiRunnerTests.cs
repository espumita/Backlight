using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Backlight.Middleware;
using Backlight.Providers;
using Backlight.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using NUnit.Framework;

namespace Backlight.Test {
    public class ApiRunnerTests {
        private const string AEntityName = nameof(UserEntity);
        private const string ANewEntityId = "aNewEntityId";
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
            var requestBodyStream = await RequestBodyStreamWith(JsonSerializer.Serialize(new BacklightApiRequest { Entity = AEntityName }));
            httpContext.Request.Body = requestBodyStream;
            backlightProvidersService.IsEntityConfiguredFor(AEntityName).Returns(false);

            await runner.Run();

            httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            var responseBody = await ReadBodyFrom(httpContext.Response.Body);
            responseBody.Should().Be("Entity is not configured");
        }

        [Test, TestCaseSource("AllowedMethods")]
        public async Task get_bad_request_when_entity_provider_available(string httpMethod) {
            httpContext.Request.Method = httpMethod;
            var requestBodyStream = await RequestBodyStreamWith(JsonSerializer.Serialize(new BacklightApiRequest { Entity = AEntityName }));
            httpContext.Request.Body = requestBodyStream;
            backlightProvidersService.IsEntityConfiguredFor(AEntityName).Returns(true);
            backlightProvidersService.IsProviderAvailableFor(AEntityName, httpMethod).Returns(false);

            await runner.Run();

            httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            var responseBody = await ReadBodyFrom(httpContext.Response.Body);
            responseBody.Should().Be("Entity provider is not available");
        }

        [Test]
        public async Task execute_create_entity_provider() {
            var httpMethod = HttpMethods.Put;
            httpContext.Request.Method = httpMethod;
            var aUserEntity = new UserEntity{ Name = "aName", Age = 23 };
            var requestBodyStream = await RequestBodyStreamWith(JsonSerializer.Serialize(new BacklightApiRequest {
                Entity = AEntityName,
                PayLoad = JsonSerializer.Serialize(aUserEntity)
            }));
            httpContext.Request.Body = requestBodyStream;
            backlightProvidersService.IsEntityConfiguredFor(AEntityName).Returns(true);
            backlightProvidersService.IsProviderAvailableFor(AEntityName, httpMethod).Returns(true);
            var createProvider = Substitute.For<CreateProvider>();
            backlightProvidersService.ProviderFor(AEntityName, httpMethod).Returns((entityPayload) => {
                var entity = JsonSerializer.Deserialize<UserEntity>(entityPayload);
                createProvider.Create(entity);
            });

            await runner.Run();

            createProvider.Received().Create(aUserEntity);
            httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var responseBody = await ReadBodyFrom(httpContext.Response.Body);
            responseBody.Should().Be("Entity created");
        }

        [Test]
        public async Task execute_read_entity_provider() {
            var httpMethod = HttpMethods.Get;
            httpContext.Request.Method = httpMethod;
            var aUserEntity = new UserEntity { Name = "aName", Age = 23 };
            var requestBodyStream = await RequestBodyStreamWith(JsonSerializer.Serialize(new BacklightApiRequest {
                Entity = AEntityName,
                PayLoad = ANewEntityId
            }));
            httpContext.Request.Body = requestBodyStream;
            backlightProvidersService.IsEntityConfiguredFor(AEntityName).Returns(true);
            backlightProvidersService.IsProviderAvailableFor(AEntityName, httpMethod).Returns(true);
            var readProvider = Substitute.For<ReadProvider>();
            readProvider.Read<UserEntity>(ANewEntityId).Returns(aUserEntity);
            backlightProvidersService.ReaderProviderFor(AEntityName, httpMethod).Returns((entityId) => {
                var entity = readProvider.Read<UserEntity>(entityId);
                return JsonSerializer.Serialize(entity);
            });

            await runner.Run();

            httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var responseBody = await ReadBodyFrom(httpContext.Response.Body);
            responseBody.Should().Be(JsonSerializer.Serialize(aUserEntity));
        }

        [Test]
        public async Task execute_update_entity_provider() {
            var httpMethod = HttpMethods.Post;
            httpContext.Request.Method = httpMethod;
            var aUserEntity = new UserEntity { Name = "aName", Age = 23 };
            var requestBodyStream = await RequestBodyStreamWith(JsonSerializer.Serialize(new BacklightApiRequest {
                Entity = AEntityName,
                PayLoad = JsonSerializer.Serialize(aUserEntity)
            }));
            httpContext.Request.Body = requestBodyStream;
            backlightProvidersService.IsEntityConfiguredFor(AEntityName).Returns(true);
            backlightProvidersService.IsProviderAvailableFor(AEntityName, httpMethod).Returns(true);
            var updateProvider = Substitute.For<UpdateProvider>();
            backlightProvidersService.UpdateProviderFor(AEntityName, httpMethod).Returns((entityId, entityPayload) => {
                var entity = JsonSerializer.Deserialize<UserEntity>(entityPayload);
                updateProvider.Update(entityId, entity);
            });

            await runner.Run();

            updateProvider.Received().Update(ANewEntityId, aUserEntity);
            httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var responseBody = await ReadBodyFrom(httpContext.Response.Body);
            responseBody.Should().Be("Entity updated");
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