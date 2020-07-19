﻿using System;
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
            configuration.Create.Should().BeEmpty();
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
            VerifyOnlyCan(providersConfiguration, create: true);
            var create = serviceConfiguration.Create.Single();
            create.Key.Should().Be<UserEntity>();
            create.Value(JsonSerializer.Serialize(AUserEntity));
            createProvider.Received().Create(AUserEntity);
            //configuration.Read.Should().BeEmpty();
            //configuration.Update.Should().BeEmpty();
            //configuration.Delete.Should().BeEmpty();
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

            var configuration = VerifyServiceTypeAndGetServiceConfiguration();
            var keyValuePair = configuration.ProvidersConfiguration.Single();
            keyValuePair.Key.Should().Be<UserEntity>();
            VerifyOnlyCan(keyValuePair, read: true);
            var valuePair = configuration.Read.Single();
            valuePair.Key.Should().Be<UserEntity>();
            var readedValue = valuePair.Value(AnEntityId);
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

            var configuration = VerifyServiceTypeAndGetServiceConfiguration();
            var keyValuePair = configuration.ProvidersConfiguration.Single();
            keyValuePair.Key.Should().Be<UserEntity>();
            VerifyOnlyCan(keyValuePair, update: true);
            var valuePair = configuration.Update.Single();
            valuePair.Key.Should().Be<UserEntity>();
            var userEntity = new UserEntity { Name = "aName", Age = 23 };
            const string AnEntityId = "anEntityId";
            valuePair.Value(AnEntityId, JsonSerializer.Serialize(userEntity));
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

            var configuration = VerifyServiceTypeAndGetServiceConfiguration();
            var keyValuePair = configuration.ProvidersConfiguration.Single();
            keyValuePair.Key.Should().Be<UserEntity>();
            VerifyOnlyCan(keyValuePair, delete: true);
            var valuePair = configuration.Delete.Single();
            valuePair.Key.Should().Be<UserEntity>();
            const string AnEntityId = "anEntityId";
            valuePair.Value(AnEntityId);
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

            var configuration = VerifyServiceTypeAndGetServiceConfiguration();
            var keyValuePair = configuration.ProvidersConfiguration.Single();
            keyValuePair.Key.Should().Be<UserEntity>();
            VerifyOnlyCan(keyValuePair, true, true, true, true);
            //Todo test 4
        }

        private static void VerifyOnlyCan(KeyValuePair<Type, ProvidersConfiguration> providersConfiguration, bool create = false, bool read = false, bool update = false, bool delete = false) {
            providersConfiguration.Value.CanCreate().Should().Be(create);
            providersConfiguration.Value.CanRead().Should().Be(read);
            providersConfiguration.Value.CanUpdate().Should().Be(update);
            providersConfiguration.Value.CanDelete().Should().Be(delete);
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