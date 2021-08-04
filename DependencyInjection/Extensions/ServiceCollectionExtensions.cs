using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        private static object BuildInstance(IServiceProvider serviceProvider, Type implementationType)
        {
            var constructor = implementationType.GetConstructors().SingleOrDefault();
            if (constructor == null)
                throw new InvalidOperationException($"Implementation type {implementationType.FullName} must have single constructor");
            var parameres = constructor.GetParameters();
            var parameterValues = parameres.Select(p => serviceProvider.GetService(p.ParameterType)).ToArray();
            var serviceInstance = Activator.CreateInstance(implementationType, parameterValues);
            return serviceInstance;
        }

        public static IServiceCollection AddSingleton<TService, TImplementation>(this IServiceCollection services) where TImplementation : TService
            => services.AddDefinition<TService>(sp => BuildInstance(sp, typeof(TImplementation)), Lifetime.Singleton);

        public static IServiceCollection AddSingleton<TService>(this IServiceCollection services, Func<IServiceProvider, TService> serviceFactory)
            => services.AddDefinition<TService>(sp => serviceFactory.Invoke(sp), Lifetime.Singleton);

        public static IServiceCollection AddDefinition<TService>(this IServiceCollection services, Func<IServiceProvider, object> serviceFactory, Lifetime lifetime)
        {
            var serviceDefinition = new ServiceDefinition
            {
                Lifetime = lifetime,
                ServiceType = typeof(TService),
                ServiceFactory = serviceFactory
            };
            services.Add(serviceDefinition);
            return services;
        }

        public static IServiceCollection AddTransient<TService>(this IServiceCollection services) => services.AddTransient<TService, TService>();

        public static IServiceCollection AddTransient<TService, TImplementation>(this IServiceCollection services) where TImplementation : TService
            => services.AddDefinition<TService>(sp => BuildInstance(sp, typeof(TImplementation)), Lifetime.Transient);

        public static IServiceCollection AddTransient<TService>(this IServiceCollection services, Func<IServiceProvider, TService> serviceFactory)
            => services.AddDefinition<TService>(sp => serviceFactory.Invoke(sp), Lifetime.Transient);

        public static IServiceProvider BuildServiceProvider(this IServiceCollection services)
        {
            var serviceDictionary = new Dictionary<Type, ServiceDefinition>();
            var serviceCollectionsDictionary = new Dictionary<Type, ICollection<ServiceDefinition>>();
            foreach (var service in services)
            {
                var serviceType = typeof(IEnumerable<>).MakeGenericType(service.ServiceType);

                if (serviceDictionary.ContainsKey(serviceType))
                {
                    serviceCollectionsDictionary[serviceType].Add(service);
                }
                else
                {
                    var serviceDefinitions = new List<ServiceDefinition> { service };
                    serviceCollectionsDictionary[serviceType] = serviceDefinitions;
                    var serviceCollectionDefinition = new ServiceDefinition
                    {
                        Lifetime = service.Lifetime,
                        ServiceType = serviceType,
                        ServiceFactory = sp =>
                        {
                            var constructedListType = typeof(List<>).MakeGenericType(service.ServiceType);
                            var list = (IList)Activator.CreateInstance(constructedListType);
                            foreach (var serviceDefinition in serviceDefinitions)
                            {
                                var value = serviceDefinition.ServiceFactory.Invoke(sp);
                                list.Add(value);
                            }
                            return list;
                        }
                    };
                    serviceDictionary[serviceType] = serviceCollectionDefinition;
                }

                serviceDictionary[service.ServiceType] = service;
            }
            var serviceProvider = new ServiceProvider(serviceDictionary);
            return serviceProvider;
        }

        public static T GetService<T>(this IServiceProvider serviceProvider) => (T)serviceProvider.GetService(typeof(T));
    }
}
