using System;
using System.Net.Http;
using Backlight.Sample.Web.Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using NUnit.Framework;

namespace Backlight.Web.Api.e2e.Test {

    [SetUpFixture]
    public class TestFixture {

        public static HttpClient serverClient;


        [OneTimeSetUp]
        public void SetUp() {
            StartTestApiServer();
        }

        private void StartTestApiServer() {
            const string testServerUrl = "http://localhost:50351";
            var webHostBuilder = new WebHostBuilder()
                .UseUrls(testServerUrl)
                .UseStartup<Startup>();
            var testServer = new TestServer(webHostBuilder);
            serverClient = testServer.CreateClient();
            serverClient.BaseAddress = new Uri(testServerUrl);
        }

    }
}