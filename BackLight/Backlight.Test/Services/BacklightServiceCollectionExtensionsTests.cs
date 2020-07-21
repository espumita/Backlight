using System.Linq;
using System.Text.Json;
using Backlight.Providers;
using Backlight.Services;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NUnit.Framework;

namespace Backlight.Test.Services {
    public class BacklightServiceCollectionExtensionsTests {
        private const string AnEntityId = "anEntityId";
        private ServiceCollection collection;
        private UserEntity AUserEntity = new UserEntity{ Name = "aName", Age = 23};

        [SetUp]
        public void SetUp() {
            collection = new ServiceCollection();
        }

        [Test]
        public void be_not_configured_when_there_is_no_configuration() {

            collection.AddBacklight();

            var options = VerifyServiceTypeAndGetServiceOptions();
            options.ProvidersOptions.Should().BeEmpty();
        }

        [Test]
        public void be_configured_with_a_create_provider() {
            var createProvider = Substitute.For<CreateProvider>();

            collection.AddBacklight(configuration => {
                configuration.For<UserEntity>()
                    .AddCreate(createProvider);
            });

            var serviceConfiguration = VerifyServiceTypeAndGetServiceOptions();
            var providersConfiguration = serviceConfiguration.ProvidersOptions.Single();
            providersConfiguration.Key.Should().Be<UserEntity>();
            VerifyOnlyCan<UserEntity>(serviceConfiguration, create: true);
            providersConfiguration.Value.Create(JsonSerializer.Serialize(AUserEntity));
            createProvider.Received().Create(AUserEntity);
        }

        [Test]
        public void be_configured_with_a_read_provider() {
            var readProvider = Substitute.For<ReadProvider>();
            readProvider.Read<UserEntity>(AnEntityId).Returns(AUserEntity);
            collection.AddBacklight(configuration => {
                configuration.For<UserEntity>()
                    .AddRead(readProvider);
            });

            var serviceConfiguration = VerifyServiceTypeAndGetServiceOptions();
            var providersConfiguration = serviceConfiguration.ProvidersOptions.Single();
            providersConfiguration.Key.Should().Be<UserEntity>();
            VerifyOnlyCan<UserEntity>(serviceConfiguration, read: true);
            var readedValue = providersConfiguration.Value.Read(AnEntityId);
            readedValue.Should().Be(JsonSerializer.Serialize(AUserEntity));
        }

        [Test]
        public void be_configured_with_a_update_provider() {
            var updateProvider = Substitute.For<UpdateProvider>();
            collection.AddBacklight(configuration => {
                configuration.For<UserEntity>()
                    .AddUpdate(updateProvider);
            });

            var serviceConfiguration = VerifyServiceTypeAndGetServiceOptions();
            var providersConfiguration = serviceConfiguration.ProvidersOptions.Single();
            providersConfiguration.Key.Should().Be<UserEntity>();
            VerifyOnlyCan<UserEntity>(serviceConfiguration, update: true);
            providersConfiguration.Value.Update(AnEntityId, JsonSerializer.Serialize(AUserEntity));
            updateProvider.Received().Update(AnEntityId, AUserEntity);
        }

        [Test]
        public void be_configured_with_a_delete_provider() {
            var deleteProvider = Substitute.For<DeleteProvider>();
            collection.AddBacklight(configuration => {
                configuration.For<UserEntity>()
                    .AddDelete(deleteProvider);
            });

            var serviceConfiguration = VerifyServiceTypeAndGetServiceOptions();
            var providersConfiguration = serviceConfiguration.ProvidersOptions.Single();
            providersConfiguration.Key.Should().Be<UserEntity>();
            VerifyOnlyCan<UserEntity>(serviceConfiguration, delete: true);
            providersConfiguration.Value.Delete(AnEntityId);
            deleteProvider.Received().Delete<UserEntity>(AnEntityId);
        }

        [Test]
        public void be_configured_with_a_crud_provider() {
            var crudProvider = Substitute.For<CRUDProvider>();
            collection.AddBacklight(configuration => {
                configuration.For<UserEntity>()
                    .AddCRUD(crudProvider);
            });

            var serviceConfiguration = VerifyServiceTypeAndGetServiceOptions();
            var providersConfiguration = serviceConfiguration.ProvidersOptions.Single();
            providersConfiguration.Key.Should().Be<UserEntity>();
            VerifyOnlyCan<UserEntity>(serviceConfiguration, true, true, true, true);
        }

        private static void VerifyOnlyCan<T>(ServiceOptions serviceOptions, bool create = false, bool read = false, bool update = false, bool delete = false) {
            serviceOptions.CanCreate(typeof(T).FullName).Should().Be(create);
            serviceOptions.CanRead(typeof(T).FullName).Should().Be(read);
            serviceOptions.CanUpdate(typeof(T).FullName).Should().Be(update);
            serviceOptions.CanDelete(typeof(T).FullName).Should().Be(delete);
        }

        private ServiceOptions VerifyServiceTypeAndGetServiceOptions() {
            var serviceDescriptor = collection.Single();
            serviceDescriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
            serviceDescriptor.ServiceType.Should().Be(typeof(BacklightProvidersService));
            var backlightProvidersService = (BacklightProvidersService) serviceDescriptor.ImplementationInstance;
            return backlightProvidersService.Options;
        }
    }
}