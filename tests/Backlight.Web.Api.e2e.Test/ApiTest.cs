using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Backlight.Web.Api.e2e.Test {
    public class ApiTest {
        private HttpClient client;

        [SetUp]
        public void SetUp() {
            client = TestFixture.serverClient;
        }


        [Test]
        public async Task read() {
            var response = await client.GetAsync("/back/api/entity/2");

        }
    }
}