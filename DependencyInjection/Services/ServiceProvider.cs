using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DependencyInjection
{
    public class ServiceProvider : IServiceProvider
    {
        private readonly IDictionary<Type, ServiceDefinition> serviceDictionary;
        private readonly ConcurrentDictionary<Type, object> singletons = new ConcurrentDictionary<Type, object>();

        public ServiceProvider(IDictionary<Type, ServiceDefinition> serviceDictionary)
        {
            this.serviceDictionary = serviceDictionary;
            serviceDictionary.Add(typeof(IServiceProvider), new ServiceDefinition
            {
                Lifetime = Lifetime.Singleton,
                ServiceType = typeof(IServiceProvider),
                ServiceFactory = sp => this
            });
        }

        public object GetService(Type serviceType)
        {
            if (!serviceDictionary.TryGetValue(serviceType, out var serviceDefinition))
                throw new InvalidOperationException($"Service {serviceType.FullName} not registered");

            switch (serviceDefinition.Lifetime)
            {
                case Lifetime.Singleton:
                    if (!singletons.TryGetValue(serviceType, out var value))
                    {
                        value = serviceDefinition.ServiceFactory.Invoke(this);
                        singletons[serviceType] = value;
                    }

                    return value;
                case Lifetime.Transient: return serviceDefinition.ServiceFactory.Invoke(this);
                default: throw new NotSupportedException($"Lifetime {serviceDefinition.Lifetime} not supported");
            };
        }
    }
}
