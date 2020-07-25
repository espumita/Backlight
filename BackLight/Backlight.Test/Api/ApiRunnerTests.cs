using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Backlight.Api;
using Backlight.Api.Serialization;
using Backlight.Exceptions;
using Backlight.Providers;
using Backlight.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace Backlight.Test.Api {
    public class ApiRunnerTests {
        private const string AEntityName = nameof(UserEntity);
        private const string ANewEntityId = "aNewEntityId";
        private const string ASerializedEntity = "aSerializedEntity";
        private IApplicationBuilder applicationBuilder;
        private ApiRunner runner;
        private DefaultHttpContext httpContext;
        private IServiceProvider serviceProvider;
        private BacklightService backlightService;
        private StreamSerializer streamSerializer;

        [SetUp]
        public void SetUp() {
            applicationBuilder = Substitute.For<IApplicationBuilder>();
            serviceProvider = applicationBuilder.ApplicationServices = Substitute.For<IServiceProvider>();
            backlightService = Substitute.For<BacklightService>(new object[] { null });
            httpContext = new DefaultHttpContext();
            httpContext.Response.Body = new MemoryStream();
            streamSerializer = Substitute.For<StreamSerializer>();

            runner = new ApiRunner(applicationBuilder, streamSerializer);

            serviceProvider.GetService(Arg.Is(typeof(BacklightService))).Returns(backlightService);
            backlightService.GetsAssemblyFor(AEntityName).Returns(typeof(UserEntity).Assembly);
        }

        [Test, TestCaseSource("NotAllowedMethods")]
        public async Task get_method_not_allowed_response_when_try_to_run_under_a_not_allowed_http_method(string httpMethod) {
            httpContext.Request.Method = httpMethod;

            await runner.Run(httpContext);

            httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.MethodNotAllowed);
            var responseBody = await ReadBodyFrom(httpContext.Response.Body);
            responseBody.Should().Be("Http method is not allowed");
        }

        [Test, TestCaseSource("AllowedMethods")]
        public async Task get_bad_request_when_the_entity_deserialization_has_an_error(string httpMethod) {
            httpContext.Request.Method = httpMethod;
            streamSerializer.EntityPayloadFrom(Arg.Any<Stream>()).Throws(new EntityDeserializationException());

            await runner.Run(httpContext);

            httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            var responseBody = await ReadBodyFrom(httpContext.Response.Body);
            responseBody.Should().Be("Enity deserialization error");
        }

        [Test, TestCaseSource("AllowedMethods")]
        public async Task get_bad_request_when_entity_type_is_not_configured(string httpMethod) {
            httpContext.Request.Method = httpMethod;
            GivenARequestBodyWith(AEntityName, string.Empty);
            backlightService.CreateProviderFor(Arg.Any<Type>()).Throws<EntityIsNotConfiguredException>();
            backlightService.ReaderProviderFor(Arg.Any<Type>()).Throws<EntityIsNotConfiguredException>();
            backlightService.UpdateProviderFor(Arg.Any<Type>()).Throws<EntityIsNotConfiguredException>();
            backlightService.DeleteProviderFor(Arg.Any<Type>()).Throws<EntityIsNotConfiguredException>();

            await runner.Run(httpContext);

            httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            var responseBody = await ReadBodyFrom(httpContext.Response.Body);
            responseBody.Should().Be("Enity is not configured");
        }

        [Test, TestCaseSource("AllowedMethods")]
        public async Task get_bad_request_when_entity_provider_available(string httpMethod) {
            httpContext.Request.Method = httpMethod;
            GivenARequestBodyWith(AEntityName, "");
            backlightService.CreateProviderFor(Arg.Any<Type>()).Throws<EntityProviderIsNotAvailableException>();
            backlightService.ReaderProviderFor(Arg.Any<Type>()).Throws<EntityProviderIsNotAvailableException>();
            backlightService.UpdateProviderFor(Arg.Any<Type>()).Throws<EntityProviderIsNotAvailableException>();
            backlightService.DeleteProviderFor(Arg.Any<Type>()).Throws<EntityProviderIsNotAvailableException>();

            await runner.Run(httpContext);

            httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            var responseBody = await ReadBodyFrom(httpContext.Response.Body);
            responseBody.Should().Be("Enity provider is not available");
        }

        [Test]
        public async Task execute_create_entity_provider() {
            var httpMethod = HttpMethods.Put;
            httpContext.Request.Method = httpMethod;
            GivenARequestBodyWith(AEntityName, ASerializedEntity);
            var createProviderDelegate = Substitute.For<Action<string>>();
            backlightService.CreateProviderFor(Arg.Any<Type>()).Returns(createProviderDelegate);

            await runner.Run(httpContext);

            createProviderDelegate.Received().Invoke(ASerializedEntity);
            httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var responseBody = await ReadBodyFrom(httpContext.Response.Body);
            responseBody.Should().Be("Enity created");
        }

        [Test]
        public async Task execute_read_entity_provider() {
            var httpMethod = HttpMethods.Get;
            httpContext.Request.Method = httpMethod;
            GivenARequestBodyWith(AEntityName, ANewEntityId);
            var readProviderDelegate = Substitute.For<Func<string, string>>();
            readProviderDelegate.Invoke(ANewEntityId).Returns(ASerializedEntity);
            backlightService.ReaderProviderFor(Arg.Any<Type>()).Returns(readProviderDelegate);

            await runner.Run(httpContext);

            httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var responseBody = await ReadBodyFrom(httpContext.Response.Body);
            responseBody.Should().Be(ASerializedEntity);
        }

        [Test]
        public async Task execute_update_entity_provider() {
            var httpMethod = HttpMethods.Post;
            httpContext.Request.Method = httpMethod;
            GivenARequestBodyWith(AEntityName, ASerializedEntity);
            var updateProviderDelegate = Substitute.For<Action<string, string>>();
            backlightService.UpdateProviderFor(Arg.Any<Type>()).Returns(updateProviderDelegate);

            await runner.Run(httpContext);

            updateProviderDelegate.Received().Invoke("TODOEntityId", ASerializedEntity);
            httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var responseBody = await ReadBodyFrom(httpContext.Response.Body);
            responseBody.Should().Be("Enity updated");
        }

        [Test]
        public async Task execute_delete_entity_provider() {
            var httpMethod = HttpMethods.Delete;
            httpContext.Request.Method = httpMethod;
            var aUserEntity = new UserEntity { Name = "aName", Age = 23 };
            GivenARequestBodyWith(AEntityName, ANewEntityId);
            var deleteProvider = Substitute.For<DeleteProvider>();
            backlightService.DeleteProviderFor(Arg.Any<Type>()).Returns((entityId) => {
                deleteProvider.Delete<UserEntity>(entityId);
            });

            await runner.Run(httpContext);

            deleteProvider.Received().Delete<UserEntity>(ANewEntityId);
            httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var responseBody = await ReadBodyFrom(httpContext.Response.Body);
            responseBody.Should().Be("Enity deleted");
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
            bodyStream.CanSeek.Should().BeTrue();
            bodyStream.Seek(0, SeekOrigin.Begin);
            var streamReader = new StreamReader(bodyStream);
            return await streamReader.ReadToEndAsync();
        }
        private void GivenARequestBodyWith(string entityTypeName, string entityPayloadValue) {
            streamSerializer.EntityPayloadFrom(Arg.Any<Stream>()).Returns(new EntityPayload {
                TypeName = entityTypeName,
                Value = entityPayloadValue
            });
        }
    }

}