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

        [Test]
        public void be_configued_with_a_create_provider() {
            var createProvider = Substitute.For<CreateProvider>();
            collection.AddBacklight(configuration => {
                configuration.For<UserEntity>()
                    .AddCreate(createProvider);
            });

            var serviceDescriptor = collection.Single();
            serviceDescriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
            serviceDescriptor.ServiceType.Should().Be(typeof(BacklightProvidersService));
            var backlightProvidersService = (BacklightProvidersService)serviceDescriptor.ImplementationInstance;
            var configuration = backlightProvidersService.Configuration;
            var keyValuePair = configuration.Providers.Single();
            keyValuePair.Key.Should().Be<UserEntity>();
            keyValuePair.Value.CanCreate().Should().BeTrue();
            var valuePair = configuration.CreateProvidersDelegates.Single();
            valuePair.Key.Should().Be<UserEntity>();
            var userEntity = new UserEntity{ Name = "aName", Age = 23};
            valuePair.Value(JsonSerializer.Serialize(userEntity));
            createProvider.Received().Create(userEntity);
            //configuration.ReadProvidersDelegates.Should().BeEmpty();
            //configuration.UpdateProvidersDelegates.Should().BeEmpty();
            //configuration.DeleteProvidersDelegates.Should().BeEmpty();
        }

        [Test]
        public void be_configued_with_a_update_provider() {
            var updateProvider = Substitute.For<UpdateProvider>();
            collection.AddBacklight(configuration => {
                configuration.For<UserEntity>()
                    .AddUpdate(updateProvider);
            });

            var serviceDescriptor = collection.Single();
            serviceDescriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
            serviceDescriptor.ServiceType.Should().Be(typeof(BacklightProvidersService));
            var backlightProvidersService = (BacklightProvidersService)serviceDescriptor.ImplementationInstance;
            var configuration = backlightProvidersService.Configuration;
            var keyValuePair = configuration.Providers.Single();
            keyValuePair.Key.Should().Be<UserEntity>();
            keyValuePair.Value.CanUpdate().Should().BeTrue();
            var valuePair = configuration.UpdateProvidersDelegates.Single();
            valuePair.Key.Should().Be<UserEntity>();
            var userEntity = new UserEntity { Name = "aName", Age = 23 };
            const string AnEntityId = "anEntityId";
            valuePair.Value(AnEntityId, JsonSerializer.Serialize(userEntity));
            updateProvider.Received().Update(AnEntityId, userEntity);
            //configuration.ReadProvidersDelegates.Should().BeEmpty();
            //configuration.CreateProvidersDelegates.Should().BeEmpty();
            //configuration.DeleteProvidersDelegates.Should().BeEmpty();
        }

        [Test]
        public void be_configued_with_a_delete_provider() {
            var deleteProvider = Substitute.For<DeleteProvider>();
            collection.AddBacklight(configuration => {
                configuration.For<UserEntity>()
                    .AddDelete(deleteProvider);
            });

            var serviceDescriptor = collection.Single();
            serviceDescriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
            serviceDescriptor.ServiceType.Should().Be(typeof(BacklightProvidersService));
            var backlightProvidersService = (BacklightProvidersService)serviceDescriptor.ImplementationInstance;
            var configuration = backlightProvidersService.Configuration;
            var keyValuePair = configuration.Providers.Single();
            keyValuePair.Key.Should().Be<UserEntity>();
            keyValuePair.Value.CanDelete().Should().BeTrue();
            var valuePair = configuration.DeleteProvidersDelegates.Single();
            valuePair.Key.Should().Be<UserEntity>();
            const string AnEntityId = "anEntityId";
            valuePair.Value(AnEntityId);
            deleteProvider.Received().Delete<UserEntity>(AnEntityId);
            //configuration.ReadProvidersDelegates.Should().BeEmpty();
            //configuration.UpdateProvidersDelegates.Should().BeEmpty();
            //configuration.CreateProvidersDelegates.Should().BeEmpty();
        }

        [Test]
        public void be_configued_with_a_read_provider() {
            var readProvider = Substitute.For<ReadProvider>();
            var userEntity = new UserEntity { Name = "aName", Age = 23 };
            const string AnEntityId = "anEntityId";
            readProvider.Read<UserEntity>(AnEntityId).Returns(userEntity);
            collection.AddBacklight(configuration => {
                configuration.For<UserEntity>()
                    .AddRead(readProvider);
            });

            var serviceDescriptor = collection.Single();
            serviceDescriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
            serviceDescriptor.ServiceType.Should().Be(typeof(BacklightProvidersService));
            var backlightProvidersService = (BacklightProvidersService)serviceDescriptor.ImplementationInstance;
            var configuration = backlightProvidersService.Configuration;
            var keyValuePair = configuration.Providers.Single();
            keyValuePair.Key.Should().Be<UserEntity>();
            keyValuePair.Value.CanRead().Should().BeTrue();
            var valuePair = configuration.ReadProvidersDelegates.Single();
            valuePair.Key.Should().Be<UserEntity>();
            var readedValue = valuePair.Value(AnEntityId);
            readedValue.Should().Be(JsonSerializer.Serialize(userEntity));
            //configuration.UpdateProvidersDelegates.Should().BeEmpty();
            //configuration.DeleteProvidersDelegates.Should().BeEmpty();
            //configuration.CreateProvidersDelegates.Should().BeEmpty();
        }

        [Test]
        public void be_configued_with_a_crud_provider() {
            var crudProvider = Substitute.For<CRUDProvider>();
            var userEntity = new UserEntity { Name = "aName", Age = 23 };
            const string AnEntityId = "anEntityId";
            collection.AddBacklight(configuration => {
                configuration.For<UserEntity>()
                    .AddCRUD(crudProvider);
            });

            var serviceDescriptor = collection.Single();
            serviceDescriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
            serviceDescriptor.ServiceType.Should().Be(typeof(BacklightProvidersService));
            var backlightProvidersService = (BacklightProvidersService)serviceDescriptor.ImplementationInstance;
            var configuration = backlightProvidersService.Configuration;
            var keyValuePair = configuration.Providers.Single();
            keyValuePair.Key.Should().Be<UserEntity>();
            keyValuePair.Value.CanCreate().Should().BeTrue();
            keyValuePair.Value.CanRead().Should().BeTrue();
            keyValuePair.Value.CanUpdate().Should().BeTrue();
            keyValuePair.Value.CanDelete().Should().BeTrue();
            //Todo test 4
        }
    }
}