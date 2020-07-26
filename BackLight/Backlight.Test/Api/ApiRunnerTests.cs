using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
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
        private BacklightService backlightService;
        private StreamSerializer streamSerializer;

        [SetUp]
        public void SetUp() {
            applicationBuilder = Substitute.For<IApplicationBuilder>();
            backlightService = Substitute.For<BacklightService>(new object[] { null });
            httpContext = new DefaultHttpContext();
            httpContext.Response.Body = new MemoryStream();
            streamSerializer = Substitute.For<StreamSerializer>();

            runner = new ApiRunner(applicationBuilder, backlightService, streamSerializer);
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
            string entityPayloadValue = string.Empty;
            GivenARequestBodyWith(new EntityPayload {
                TypeName = AEntityName,
                Value = entityPayloadValue
            });
            backlightService.Create(Arg.Any<EntityPayload>()).Throws<EntityIsNotConfiguredException>();
            backlightService.Read(Arg.Any<EntityPayload>()).Throws<EntityIsNotConfiguredException>();
            backlightService.UpdateProviderFor(Arg.Any<EntityPayload>()).Throws<EntityIsNotConfiguredException>();
            backlightService.DeleteProviderFor(Arg.Any<EntityPayload>()).Throws<EntityIsNotConfiguredException>();

            await runner.Run(httpContext);

            httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            var responseBody = await ReadBodyFrom(httpContext.Response.Body);
            responseBody.Should().Be("Enity is not configured");
        }

        [Test, TestCaseSource("AllowedMethods")]
        public async Task get_bad_request_when_entity_provider_available(string httpMethod) {
            httpContext.Request.Method = httpMethod;
            GivenARequestBodyWith(new EntityPayload {
                TypeName = AEntityName,
                Value = ""
            });
            backlightService.Create(Arg.Any<EntityPayload>()).Throws<EntityProviderIsNotAvailableException>();
            backlightService.Read(Arg.Any<EntityPayload>()).Throws<EntityProviderIsNotAvailableException>();
            backlightService.UpdateProviderFor(Arg.Any<EntityPayload>()).Throws<EntityProviderIsNotAvailableException>();
            backlightService.DeleteProviderFor(Arg.Any<EntityPayload>()).Throws<EntityProviderIsNotAvailableException>();

            await runner.Run(httpContext);

            httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            var responseBody = await ReadBodyFrom(httpContext.Response.Body);
            responseBody.Should().Be("Enity provider is not available");
        }

        [Test]
        public async Task execute_service_create_entity() {
            var httpMethod = HttpMethods.Put;
            httpContext.Request.Method = httpMethod;
            var anEntityPayLoad = new EntityPayload {
                TypeName = AEntityName,
                Value = ASerializedEntity
            };
            GivenARequestBodyWith(anEntityPayLoad);

            await runner.Run(httpContext);

            await backlightService.Received().Create(anEntityPayLoad);
            httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var responseBody = await ReadBodyFrom(httpContext.Response.Body);
            responseBody.Should().Be("Enity created");
        }

        [Test]
        public async Task execute_service_read_entity() {
            var httpMethod = HttpMethods.Get;
            httpContext.Request.Method = httpMethod;
            var anEntityPayLoad = new EntityPayload {
                TypeName = AEntityName,
                Value = ANewEntityId
            };
            GivenARequestBodyWith(anEntityPayLoad);
            backlightService.Read(anEntityPayLoad).Returns(ASerializedEntity);

            await runner.Run(httpContext);

            httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var responseBody = await ReadBodyFrom(httpContext.Response.Body);
            responseBody.Should().Be(ASerializedEntity);
        }

        [Test]
        public async Task execute_update_entity_provider() {
            var httpMethod = HttpMethods.Post;
            httpContext.Request.Method = httpMethod;
            GivenARequestBodyWith(new EntityPayload {
                TypeName = AEntityName,
                Value = ASerializedEntity
            });
            var updateProviderDelegate = Substitute.For<Action<string, string>>();
            backlightService.UpdateProviderFor(Arg.Any<EntityPayload>()).Returns(updateProviderDelegate);

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
            GivenARequestBodyWith(new EntityPayload {
                TypeName = AEntityName,
                Value = ANewEntityId
            });
            var deleteProvider = Substitute.For<DeleteProvider>();
            backlightService.DeleteProviderFor(Arg.Any<EntityPayload>()).Returns((entityId) => {
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
        private void GivenARequestBodyWith(EntityPayload entityPayload) {
            streamSerializer.EntityPayloadFrom(Arg.Any<Stream>())
                .Returns(entityPayload);
        }
    }

}