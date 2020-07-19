using System;
using System.Collections.Generic;
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

            var configuration = VerifyServiceTypeAndGetConfiguration();
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

            var configuration = VerifyServiceTypeAndGetConfiguration();
            var keyValuePair = configuration.Providers.Single();
            keyValuePair.Key.Should().Be<UserEntity>();
            VerifyOnlyCan(keyValuePair, create: true);
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
        public void be_configued_with_a_read_provider() {
            var readProvider = Substitute.For<ReadProvider>();
            var userEntity = new UserEntity { Name = "aName", Age = 23 };
            const string AnEntityId = "anEntityId";
            readProvider.Read<UserEntity>(AnEntityId).Returns(userEntity);
            collection.AddBacklight(configuration => {
                configuration.For<UserEntity>()
                    .AddRead(readProvider);
            });

            var configuration = VerifyServiceTypeAndGetConfiguration();
            var keyValuePair = configuration.Providers.Single();
            keyValuePair.Key.Should().Be<UserEntity>();
            VerifyOnlyCan(keyValuePair, read: true);
            var valuePair = configuration.ReadProvidersDelegates.Single();
            valuePair.Key.Should().Be<UserEntity>();
            var readedValue = valuePair.Value(AnEntityId);
            readedValue.Should().Be(JsonSerializer.Serialize(userEntity));
            //configuration.UpdateProvidersDelegates.Should().BeEmpty();
            //configuration.DeleteProvidersDelegates.Should().BeEmpty();
            //configuration.CreateProvidersDelegates.Should().BeEmpty();
        }

        [Test]
        public void be_configued_with_a_update_provider() {
            var updateProvider = Substitute.For<UpdateProvider>();
            collection.AddBacklight(configuration => {
                configuration.For<UserEntity>()
                    .AddUpdate(updateProvider);
            });

            var configuration = VerifyServiceTypeAndGetConfiguration();
            var keyValuePair = configuration.Providers.Single();
            keyValuePair.Key.Should().Be<UserEntity>();
            VerifyOnlyCan(keyValuePair, update: true);
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

            var configuration = VerifyServiceTypeAndGetConfiguration();
            var keyValuePair = configuration.Providers.Single();
            keyValuePair.Key.Should().Be<UserEntity>();
            VerifyOnlyCan(keyValuePair, delete: true);
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
        public void be_configued_with_a_crud_provider() {
            var crudProvider = Substitute.For<CRUDProvider>();
            var userEntity = new UserEntity { Name = "aName", Age = 23 };
            const string AnEntityId = "anEntityId";
            collection.AddBacklight(configuration => {
                configuration.For<UserEntity>()
                    .AddCRUD(crudProvider);
            });

            var configuration = VerifyServiceTypeAndGetConfiguration();
            var keyValuePair = configuration.Providers.Single();
            keyValuePair.Key.Should().Be<UserEntity>();
            VerifyOnlyCan(keyValuePair, true, true, true, true);
            //Todo test 4
        }

        private static void VerifyOnlyCan(KeyValuePair<Type, ProvidersConfiguration> prividersConfiguration, bool create = false, bool read = false, bool update = false, bool delete = false) {
            prividersConfiguration.Value.CanCreate().Should().Be(create);
            prividersConfiguration.Value.CanRead().Should().Be(read);
            prividersConfiguration.Value.CanUpdate().Should().Be(update);
            prividersConfiguration.Value.CanDelete().Should().Be(delete);
        }

        private ServiceConfiguration VerifyServiceTypeAndGetConfiguration() {
            var serviceDescriptor = collection.Single();
            serviceDescriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
            serviceDescriptor.ServiceType.Should().Be(typeof(BacklightProvidersService));
            var backlightProvidersService = (BacklightProvidersService) serviceDescriptor.ImplementationInstance;
            return backlightProvidersService.Configuration;
        }
    }
}