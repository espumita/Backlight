using System.Linq;
using System.Text.Json;
using Backlight.Providers;
using Backlight.Services;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NUnit.Framework;

namespace Backlight.Test {
    public class BacklightServiceCollectionExtensionsTests {
        private ServiceCollection collection;
        private UserEntity AUserEntity = new UserEntity{ Name = "aName", Age = 23};

        [SetUp]
        public void SetUp() {
            collection = new ServiceCollection();
        }

        [Test]
        public void be_not_configured_when_there_is_no_configuration() {

            collection.AddBacklight();

            var configuration = VerifyServiceTypeAndGetServiceConfiguration();
            configuration.ProvidersConfiguration.Should().BeEmpty();
            configuration.Read.Should().BeEmpty();
            configuration.Update.Should().BeEmpty();
            configuration.Delete.Should().BeEmpty();
        }

        [Test]
        public void be_configured_with_a_create_provider() {
            var createProvider = Substitute.For<CreateProvider>();

            collection.AddBacklight(configuration => {
                configuration.For<UserEntity>()
                    .AddCreate(createProvider);
            });

            var serviceConfiguration = VerifyServiceTypeAndGetServiceConfiguration();
            var providersConfiguration = serviceConfiguration.ProvidersConfiguration.Single();
            providersConfiguration.Key.Should().Be<UserEntity>();
            VerifyOnlyCan<UserEntity>(serviceConfiguration, create: true);
            providersConfiguration.Value.Create(JsonSerializer.Serialize(AUserEntity));
            createProvider.Received().Create(AUserEntity);
        }

        [Test]
        public void be_configured_with_a_read_provider() {
            var readProvider = Substitute.For<ReadProvider>();
            var userEntity = new UserEntity { Name = "aName", Age = 23 };
            const string AnEntityId = "anEntityId";
            readProvider.Read<UserEntity>(AnEntityId).Returns(userEntity);
            collection.AddBacklight(configuration => {
                configuration.For<UserEntity>()
                    .AddRead(readProvider);
            });

            var serviceConfiguration = VerifyServiceTypeAndGetServiceConfiguration();
            var providersConfiguration = serviceConfiguration.ProvidersConfiguration.Single();
            providersConfiguration.Key.Should().Be<UserEntity>();
            VerifyOnlyCan<UserEntity>(serviceConfiguration, read: true);
            var read = serviceConfiguration.Read.Single();
            read.Key.Should().Be<UserEntity>();
            var readedValue = read.Value(AnEntityId);
            readedValue.Should().Be(JsonSerializer.Serialize(userEntity));
            //configuration.Update.Should().BeEmpty();
            //configuration.Delete.Should().BeEmpty();
            //configuration.Create.Should().BeEmpty();
        }

        [Test]
        public void be_configured_with_a_update_provider() {
            var updateProvider = Substitute.For<UpdateProvider>();
            collection.AddBacklight(configuration => {
                configuration.For<UserEntity>()
                    .AddUpdate(updateProvider);
            });

            var serviceConfiguration = VerifyServiceTypeAndGetServiceConfiguration();
            var providersConfiguration = serviceConfiguration.ProvidersConfiguration.Single();
            providersConfiguration.Key.Should().Be<UserEntity>();
            VerifyOnlyCan<UserEntity>(serviceConfiguration, update: true);
            var update = serviceConfiguration.Update.Single();
            update.Key.Should().Be<UserEntity>();
            var userEntity = new UserEntity { Name = "aName", Age = 23 };
            const string AnEntityId = "anEntityId";
            update.Value(AnEntityId, JsonSerializer.Serialize(userEntity));
            updateProvider.Received().Update(AnEntityId, userEntity);
            //configuration.Read.Should().BeEmpty();
            //configuration.Create.Should().BeEmpty();
            //configuration.Delete.Should().BeEmpty();
        }

        [Test]
        public void be_configured_with_a_delete_provider() {
            var deleteProvider = Substitute.For<DeleteProvider>();
            collection.AddBacklight(configuration => {
                configuration.For<UserEntity>()
                    .AddDelete(deleteProvider);
            });

            var serviceConfiguration = VerifyServiceTypeAndGetServiceConfiguration();
            var providersConfiguration = serviceConfiguration.ProvidersConfiguration.Single();
            providersConfiguration.Key.Should().Be<UserEntity>();
            VerifyOnlyCan<UserEntity>(serviceConfiguration, delete: true);
            var delete = serviceConfiguration.Delete.Single();
            delete.Key.Should().Be<UserEntity>();
            const string AnEntityId = "anEntityId";
            delete.Value(AnEntityId);
            deleteProvider.Received().Delete<UserEntity>(AnEntityId);
            //configuration.Read.Should().BeEmpty();
            //configuration.Update.Should().BeEmpty();
            //configuration.Create.Should().BeEmpty();
        }

        [Test]
        public void be_configured_with_a_crud_provider() {
            var crudProvider = Substitute.For<CRUDProvider>();
            var userEntity = new UserEntity { Name = "aName", Age = 23 };
            const string AnEntityId = "anEntityId";
            collection.AddBacklight(configuration => {
                configuration.For<UserEntity>()
                    .AddCRUD(crudProvider);
            });

            var serviceConfiguration = VerifyServiceTypeAndGetServiceConfiguration();
            var providersConfiguration = serviceConfiguration.ProvidersConfiguration.Single();
            providersConfiguration.Key.Should().Be<UserEntity>();
            VerifyOnlyCan<UserEntity>(serviceConfiguration, true, true, true, true);
            //Todo test 4
        }

        private static void VerifyOnlyCan<T>(ServiceConfiguration serviceConfiguration, bool create = false, bool read = false, bool update = false, bool delete = false) {
            serviceConfiguration.CanCreate(typeof(T).FullName).Should().Be(create);
            serviceConfiguration.CanRead(typeof(T).FullName).Should().Be(read);
            serviceConfiguration.CanUpdate(typeof(T).FullName).Should().Be(update);
            serviceConfiguration.CanDelete(typeof(T).FullName).Should().Be(delete);
        }

        private ServiceConfiguration VerifyServiceTypeAndGetServiceConfiguration() {
            var serviceDescriptor = collection.Single();
            serviceDescriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
            serviceDescriptor.ServiceType.Should().Be(typeof(BacklightProvidersService));
            var backlightProvidersService = (BacklightProvidersService) serviceDescriptor.ImplementationInstance;
            return backlightProvidersService.Configuration;
        }
    }
}