using System.Linq;
using Backlight.Services;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Backlight.Test {
    public class BacklightServiceCollectionExtensionsTests {
        private ServiceCollection collection;

        [SetUp]
        public void SetUp() {
            collection = new ServiceCollection();
        }

        [Test]
        public void be_not_configured_when_there_is_no_configuration() {
            collection.AddBacklight();

            var serviceDescriptor = collection.Single();
            serviceDescriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
            serviceDescriptor.ServiceType.Should().Be(typeof(BacklightProvidersService));
            var backlightProvidersService = (BacklightProvidersService)serviceDescriptor.ImplementationInstance;
            var configuration = backlightProvidersService.Configuration;
            configuration.Providers.Should().BeEmpty();
            configuration.CreateProvidersDelegates.Should().BeEmpty();
            configuration.ReadProvidersDelegates.Should().BeEmpty();
            configuration.UpdateProvidersDelegates.Should().BeEmpty();
            configuration.DeleteProvidersDelegates.Should().BeEmpty();
        }
    }
}