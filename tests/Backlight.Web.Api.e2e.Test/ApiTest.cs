using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Backlight.Sample.Web.Api.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;

namespace Backlight.Web.Api.e2e.Test {
    public class ApiTest {
        private HttpClient client;
        private string AEntityName = typeof(ExampleEntity2).FullName;
        private string AnEntityId = Guid.NewGuid().ToString();

        [SetUp]
        public void SetUp() {
            client = TestFixture.serverClient;
        }

        [Test]
        public async Task create() {
            var requestUri = $"/back/api/type/{AEntityName}";
            var content = AContentWith(new ExampleEntity2 {
                Name = "Freddie Mercury"
            });

            var response = await client.PutAsync(requestUri, content);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseBody = await ReadBodyFrom(response.Content);
            responseBody.Should().Contain("Enity created with id: ");
        }

        private ByteArrayContent AContentWith(ExampleEntity2 entity) {
            return new ByteArrayContent(System.Text.Encoding.UTF8.GetBytes(JsonSerializer.Serialize(entity)));
        }

        [Test]
        public async Task read() {
            var requestUri = $"/back/api/type/{AEntityName}/entity/{AnEntityId}";

            var response = await client.GetAsync(requestUri);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseBody = await ReadBodyFrom(response.Content);
            var entity = JsonSerializer.Deserialize<ExampleEntity2>(responseBody);
            entity.Name.Should().Be("George Lucas");
        }

        private static async Task<string> ReadBodyFrom(HttpContent httpContent) {
            var memoryStream = new MemoryStream();
            await httpContent.CopyToAsync(memoryStream);
            memoryStream.CanSeek.Should().BeTrue();
            memoryStream.Seek(0, SeekOrigin.Begin);
            var streamReader = new StreamReader(memoryStream);
            return await streamReader.ReadToEndAsync();
        }
    }
}