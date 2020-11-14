using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Backlight.Sample.Web.Api.Entities;
using FluentAssertions;
using NUnit.Framework;

namespace Backlight.Web.Api.e2e.Test {
    public class ApiTest {
        private HttpClient client;
        private string AEntityName = typeof(User).FullName;
        private string AnEntityId = Guid.NewGuid().ToString();

        [SetUp]
        public void SetUp() {
            client = TestFixture.serverClient;
        }

        [Test]
        public async Task create() {
            var requestUri = $"/back/api/types/{AEntityName}";
            var content = AContentWith(new User {
                Name = "Freddie Mercury"
            });

            var response = await client.PutAsync(requestUri, content);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseBody = await ReadBodyFrom(response.Content);
            responseBody.Should().Contain("Enity created with id: ");
        }

        [Test]
        public async Task read() {
            var requestUri = $"/back/api/types/{AEntityName}/entities/{AnEntityId}";

            var response = await client.GetAsync(requestUri);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseBody = await ReadBodyFrom(response.Content);
            var entity = JsonSerializer.Deserialize<User>(responseBody);
            entity.Name.Should().Be("George Lucas");
        }


        [Test]
        public async Task update() {
            var requestUri = $"/back/api/types/{AEntityName}/entities/{AnEntityId}";
            var content = AContentWith(new User {
                Name = "Ellen DeGeneres"
            });

            var response = await client.PostAsync(requestUri, content);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseBody = await ReadBodyFrom(response.Content);
            responseBody.Should().Be("Enity updated");
        }

        [Test]
        public async Task delete() {
            var requestUri = $"/back/api/types/{AEntityName}/entities/{AnEntityId}";

            var response = await client.DeleteAsync(requestUri);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseBody = await ReadBodyFrom(response.Content);
            responseBody.Should().Be("Enity deleted");
        }
        
        [Test]
        public async Task read_all_ids() {
            var requestUri = $"/back/api/types/{AEntityName}/all";

            var response = await client.GetAsync(requestUri);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseBody = await ReadBodyFrom(response.Content);
            responseBody.Should().Be("[\"00000000\",\"11111111\",\"22222222\",\"33333333\",\"44444444\",\"55555555\",\"66666666\",\"77777777\",\"88888888\",\"99999999\",\"AAAAAAAA\",\"BBBBBBBB\",\"CCCCCCCC\",\"DDDDDDDD\",\"EEEEEEEE\",\"FFFFFFFF\"]");
       }


        private ByteArrayContent AContentWith(User entity) {
            return new ByteArrayContent(System.Text.Encoding.UTF8.GetBytes(JsonSerializer.Serialize(entity)));
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