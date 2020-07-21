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

            var service = VerifyServiceOfType<BacklightProvidersService>();
            service.Options.ProvidersOptions.Should().BeEmpty();
        }

        [Test]
        public void be_configured_with_a_create_provider() {
            var createProvider = Substitute.For<CreateProvider>();

            collection.AddBacklight(configuration => {
                configuration.For<UserEntity>()
                    .AddCreate(createProvider);
            });

            var service = VerifyServiceOfType<BacklightProvidersService>();
            var providersOptions = service.Options.ProvidersOptions.Single();
            providersOptions.Key.Should().Be<UserEntity>();
            VerifyOnlyCan<UserEntity>(service.Options, create: true);
            providersOptions.Value.CreateDelegate(JsonSerializer.Serialize(AUserEntity));
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

            var service = VerifyServiceOfType<BacklightProvidersService>();
            var providersOptions = service.Options.ProvidersOptions.Single();
            providersOptions.Key.Should().Be<UserEntity>();
            VerifyOnlyCan<UserEntity>(service.Options, read: true);
            var readedValue = providersOptions.Value.ReadDelegate(AnEntityId);
            readedValue.Should().Be(JsonSerializer.Serialize(AUserEntity));
        }

        [Test]
        public void be_configured_with_a_update_provider() {
            var updateProvider = Substitute.For<UpdateProvider>();
            collection.AddBacklight(configuration => {
                configuration.For<UserEntity>()
                    .AddUpdate(updateProvider);
            });

            var service = VerifyServiceOfType<BacklightProvidersService>();
            var providersOptions = service.Options.ProvidersOptions.Single();
            providersOptions.Key.Should().Be<UserEntity>();
            VerifyOnlyCan<UserEntity>(service.Options, update: true);
            providersOptions.Value.UpdateDelegate(AnEntityId, JsonSerializer.Serialize(AUserEntity));
            updateProvider.Received().Update(AnEntityId, AUserEntity);
        }

        [Test]
        public void be_configured_with_a_delete_provider() {
            var deleteProvider = Substitute.For<DeleteProvider>();
            collection.AddBacklight(configuration => {
                configuration.For<UserEntity>()
                    .AddDelete(deleteProvider);
            });

            var service = VerifyServiceOfType<BacklightProvidersService>();
            var providersOptions = service.Options.ProvidersOptions.Single();
            providersOptions.Key.Should().Be<UserEntity>();
            VerifyOnlyCan<UserEntity>(service.Options, delete: true);
            providersOptions.Value.DeleteDelegate(AnEntityId);
            deleteProvider.Received().Delete<UserEntity>(AnEntityId);
        }

        [Test]
        public void be_configured_with_a_crud_provider() {
            var crudProvider = Substitute.For<CRUDProvider>();
            collection.AddBacklight(configuration => {
                configuration.For<UserEntity>()
                    .AddCRUD(crudProvider);
            });

            var service = VerifyServiceOfType<BacklightProvidersService>();
            var providersOptions = service.Options.ProvidersOptions.Single();
            providersOptions.Key.Should().Be<UserEntity>();
            VerifyOnlyCan<UserEntity>(service.Options, true, true, true, true);
        }

        private static void VerifyOnlyCan<T>(ServiceOptions serviceOptions, bool create = false, bool read = false, bool update = false, bool delete = false) {
            serviceOptions.CanCreate(typeof(T).FullName).Should().Be(create);
            serviceOptions.CanRead(typeof(T).FullName).Should().Be(read);
            serviceOptions.CanUpdate(typeof(T).FullName).Should().Be(update);
            serviceOptions.CanDelete(typeof(T).FullName).Should().Be(delete);
        }

        private T VerifyServiceOfType<T>() {
            var serviceDescriptor = collection.Single();
            serviceDescriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
            serviceDescriptor.ServiceType.Should().Be(typeof(T));
            return (T) serviceDescriptor.ImplementationInstance;
        }
    }
}