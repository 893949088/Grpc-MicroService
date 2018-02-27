﻿
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Grpc.Hosting.Internal
{
    public class StartupLoader
    {
        // Creates an <see cref="StartupMethods"/> instance with the actions to run for configuring the application services and the
        // request pipeline of the application.
        // When using convention based startup, the process for initializing the services is as follows:
        // The host looks for a method with the signature <see cref="IServiceProvider"/> ConfigureServices(<see cref="IServiceCollection"/> services).
        // If it can't find one, it looks for a method with the signature <see cref="void"/> ConfigureServices(<see cref="IServiceCollection"/> services).
        // When the configure services method is void returning, the host builds a services configuration function that runs all the <see cref="IStartupConfigureServicesFilter"/>
        // instances registered on the host, along with the ConfigureServices method following a decorator pattern.
        // Additionally to the ConfigureServices method, the Startup class can define a <see cref="void"/> ConfigureContainer&lt;TContainerBuilder&gt;(TContainerBuilder builder)
        // method that further configures services into the container. If the ConfigureContainer method is defined, the services configuration function
        // creates a TContainerBuilder <see cref="IServiceProviderFactory{TContainerBuilder}"/> and runs all the <see cref="IStartupConfigureContainerFilter{TContainerBuilder}"/>
        // instances registered on the host, along with the ConfigureContainer method following a decorator pattern.
        // For example:
        // StartupFilter1
        //   StartupFilter2
        //     ConfigureServices
        //   StartupFilter2
        // StartupFilter1
        // ConfigureContainerFilter1
        //   ConfigureContainerFilter2
        //     ConfigureContainer
        //   ConfigureContainerFilter2
        // ConfigureContainerFilter1
        // 
        // If the Startup class ConfigureServices returns an <see cref="IServiceProvider"/> and there is at least an <see cref="IStartupConfigureServicesFilter"/> registered we
        // throw as the filters can't be applied.
        public static StartupMethods LoadMethods(IServiceProvider hostingServiceProvider, Type startupType)
        {
            var configureMethod = FindConfigureDelegate(startupType);

            var servicesMethod = FindConfigureServicesDelegate(startupType);
            var configureContainerMethod = FindConfigureContainerDelegate(startupType);

            object instance = null;
            if (!configureMethod.MethodInfo.IsStatic || (servicesMethod != null && !servicesMethod.MethodInfo.IsStatic))
            {
                instance = ActivatorUtilities.GetServiceOrCreateInstance(hostingServiceProvider, startupType);
            }

            // The type of the TContainerBuilder. If there is no ConfigureContainer method we can just use object as it's not
            // going to be used for anything.
            var type = configureContainerMethod.MethodInfo != null ? configureContainerMethod.GetContainerType() : typeof(object);

            var builder = (ConfigureServicesDelegateBuilder)Activator.CreateInstance(
                typeof(ConfigureServicesDelegateBuilder<>).MakeGenericType(type),
                hostingServiceProvider,
                servicesMethod,
                configureContainerMethod,
                instance);

            return new StartupMethods(instance, configureMethod.Build(instance), builder.Build());
        }

        private abstract class ConfigureServicesDelegateBuilder
        {
            public abstract Func<IServiceCollection, IServiceProvider> Build();
        }

        private class ConfigureServicesDelegateBuilder<TContainerBuilder> : ConfigureServicesDelegateBuilder
        {
            public ConfigureServicesDelegateBuilder(
                IServiceProvider hostingServiceProvider,
                ConfigureServicesBuilder configureServicesBuilder,
                ConfigureContainerBuilder configureContainerBuilder,
                object instance)
            {
                HostingServiceProvider = hostingServiceProvider;
                ConfigureServicesBuilder = configureServicesBuilder;
                ConfigureContainerBuilder = configureContainerBuilder;
                Instance = instance;
            }

            public IServiceProvider HostingServiceProvider { get; }
            public ConfigureServicesBuilder ConfigureServicesBuilder { get; }
            public ConfigureContainerBuilder ConfigureContainerBuilder { get; }
            public object Instance { get; }

            public override Func<IServiceCollection, IServiceProvider> Build()
            {
                ConfigureServicesBuilder.StartupServiceFilters = BuildStartupServicesFilterPipeline;
                var configureServicesCallback = ConfigureServicesBuilder.Build(Instance);

                ConfigureContainerBuilder.ConfigureContainerFilters = ConfigureContainerPipeline;
                var configureContainerCallback = ConfigureContainerBuilder.Build(Instance);

                return ConfigureServices(configureServicesCallback, configureContainerCallback);

                Action<object> ConfigureContainerPipeline(Action<object> action)
                {
                    return Target;

                    // The ConfigureContainer pipeline needs an Action<TContainerBuilder> as source, so we just adapt the
                    // signature with this function.
                    void Source(TContainerBuilder containerBuilder) =>
                        action(containerBuilder);

                    // The ConfigureContainerBuilder.ConfigureContainerFilters expects an Action<object> as value, but our pipeline
                    // produces an Action<TContainerBuilder> given a source, so we wrap it on an Action<object> that internally casts
                    // the object containerBuilder to TContainerBuilder to match the expected signature of our ConfigureContainer pipeline.
                    void Target(object containerBuilder) =>
                        BuildStartupConfigureContainerFiltersPipeline(Source)((TContainerBuilder)containerBuilder);
                }
            }

            Func<IServiceCollection, IServiceProvider> ConfigureServices(
                Func<IServiceCollection, IServiceProvider> configureServicesCallback,
                Action<object> configureContainerCallback)
            {
                return ConfigureServicesWithContainerConfiguration;

                IServiceProvider ConfigureServicesWithContainerConfiguration(IServiceCollection services)
                {
                    // Call ConfigureServices, if that returned an IServiceProvider, we're done
                    IServiceProvider applicationServiceProvider = configureServicesCallback.Invoke(services);

                    if (applicationServiceProvider != null)
                    {
                        return applicationServiceProvider;
                    }

                    // If there's a ConfigureContainer method
                    if (ConfigureContainerBuilder.MethodInfo != null)
                    {
                        var serviceProviderFactory = HostingServiceProvider.GetRequiredService<IServiceProviderFactory<TContainerBuilder>>();
                        var builder = serviceProviderFactory.CreateBuilder(services);
                        configureContainerCallback(builder);
                        applicationServiceProvider = serviceProviderFactory.CreateServiceProvider(builder);
                    }
                    else
                    {
                        // Get the default factory
                        var serviceProviderFactory = HostingServiceProvider.GetRequiredService<IServiceProviderFactory<IServiceCollection>>();
                        var builder = serviceProviderFactory.CreateBuilder(services);
                        applicationServiceProvider = serviceProviderFactory.CreateServiceProvider(builder);
                    }
                    
                    return applicationServiceProvider ?? services.BuildServiceProvider();
                }
            }

            private Func<IServiceCollection, IServiceProvider> BuildStartupServicesFilterPipeline(Func<IServiceCollection, IServiceProvider> startup)
            {
                return RunPipeline;

                IServiceProvider RunPipeline(IServiceCollection services)
                {
                    var filters = HostingServiceProvider.GetRequiredService<IEnumerable<IStartupConfigureServicesFilter>>().Reverse().ToArray();

                    // If there are no filters just run startup (makes IServiceProvider ConfigureServices(IServiceCollection services) work.
                    if (filters.Length == 0)
                    {
                        return startup(services);
                    }

                    Action<IServiceCollection> pipeline = InvokeStartup;
                    for (int i = 0; i < filters.Length; i++)
                    {
                        pipeline = filters[i].ConfigureServices(pipeline);
                    }

                    pipeline(services);

                    // We return null so that the host here builds the container (same result as void ConfigureServices(IServiceCollection services);
                    return null;

                    void InvokeStartup(IServiceCollection serviceCollection)
                    {
                        var result = startup(serviceCollection);
                        if (filters.Length > 0 && result != null)
                        {
                            // public IServiceProvider ConfigureServices(IServiceCollection serviceCollection) is not compatible with IStartupServicesFilter;
                            var message = $"A ConfigureServices method that returns an {nameof(IServiceProvider)} is " +
                                $"not compatible with the use of one or more {nameof(IStartupConfigureServicesFilter)}. " +
                                $"Use a void returning ConfigureServices method instead or a ConfigureContainer method.";
                            throw new InvalidOperationException(message);
                        };
                    }
                }
            }

            private Action<TContainerBuilder> BuildStartupConfigureContainerFiltersPipeline(Action<TContainerBuilder> configureContainer)
            {
                return RunPipeline;

                void RunPipeline(TContainerBuilder containerBuilder)
                {
                    var filters = HostingServiceProvider
                        .GetRequiredService<IEnumerable<IStartupConfigureContainerFilter<TContainerBuilder>>>()
                        .Reverse()
                        .ToArray();

                    Action<TContainerBuilder> pipeline = InvokeConfigureContainer;
                    for (int i = 0; i < filters.Length; i++)
                    {
                        pipeline = filters[i].ConfigureContainer(pipeline);
                    }

                    pipeline(containerBuilder);

                    void InvokeConfigureContainer(TContainerBuilder builder) => configureContainer(builder);
                }
            }
        }

        public static Type FindStartupType(string startupAssemblyName)
        {
            if (string.IsNullOrEmpty(startupAssemblyName))
            {
                throw new ArgumentException(
                    string.Format("A startup method, startup type or startup assembly is required. If specifying an assembly, '{0}' cannot be null or empty.",
                    nameof(startupAssemblyName)),
                    nameof(startupAssemblyName));
            }

            var assembly = Assembly.Load(new AssemblyName(startupAssemblyName));
            if (assembly == null)
            {
                throw new InvalidOperationException(String.Format("The assembly '{0}' failed to load.", startupAssemblyName));
            }
            
            var startupNameWithoutEnv = "Startup";

            // Check the most likely places first
            var type =
                assembly.GetType(startupNameWithoutEnv) ??
                assembly.GetType(startupAssemblyName + "." + startupNameWithoutEnv);

            if (type == null)
            {
                // Full scan
                var definedTypes = assembly.DefinedTypes.ToList();
                
                var startupType = definedTypes.Where(info => info.Name.Equals(startupNameWithoutEnv, StringComparison.OrdinalIgnoreCase));

                var typeInfo = startupType.FirstOrDefault();
                if (typeInfo != null)
                {
                    type = typeInfo.AsType();
                }
            }

            if (type == null)
            {
                throw new InvalidOperationException(String.Format("A type named '{1}' could not be found in assembly '{2}'.",
                    startupNameWithoutEnv,
                    startupAssemblyName));
            }

            return type;
        }

        private static ConfigureBuilder FindConfigureDelegate(Type startupType)
        {
            var configureMethod = FindMethod(startupType, "Configure", typeof(void), required: true);
            return new ConfigureBuilder(configureMethod);
        }

        private static ConfigureContainerBuilder FindConfigureContainerDelegate(Type startupType)
        {
            var configureMethod = FindMethod(startupType, "ConfigureContainer", typeof(void), required: false);
            return new ConfigureContainerBuilder(configureMethod);
        }

        private static ConfigureServicesBuilder FindConfigureServicesDelegate(Type startupType)
        {
            var servicesMethod = FindMethod(startupType, "ConfigureServices", typeof(IServiceProvider), required: false)
                ?? FindMethod(startupType, "ConfigureServices", typeof(void), required: false);
            return new ConfigureServicesBuilder(servicesMethod);
        }

        private static MethodInfo FindMethod(Type startupType, string methodName, Type returnType = null, bool required = true)
        {
            var methodNameWithNoEnv = string.Format(CultureInfo.InvariantCulture, methodName, "");

            var methods = startupType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            var selectedMethods = methods.Where(method => method.Name.Equals(methodNameWithNoEnv, StringComparison.OrdinalIgnoreCase)).ToList();
            if (selectedMethods.Count > 1)
            {
                throw new InvalidOperationException(string.Format("Having multiple overloads of method '{0}' is not supported.", methodNameWithNoEnv));
            }


            var methodInfo = selectedMethods.FirstOrDefault();
            if (methodInfo == null)
            {
                if (required)
                {
                    throw new InvalidOperationException(string.Format("A public method named '{0}' could not be found in the '{1}' type.",
                        methodNameWithNoEnv,
                        startupType.FullName));
                }
                return null;
            }
            if (returnType != null && methodInfo.ReturnType != returnType)
            {
                if (required)
                {
                    throw new InvalidOperationException(string.Format("The '{0}' method in the type '{1}' must have a return type of '{2}'.",
                        methodInfo.Name,
                        startupType.FullName,
                        returnType.Name));
                }
                return null;
            }
            return methodInfo;
        }
    }
}
