using System;

namespace DependencyInjection
{
    public class ServiceDefinition
    {
        public Type ServiceType { get; set; }
        public Func<IServiceProvider, object> ServiceFactory { get; set; }
        public Lifetime Lifetime { get; set; }
    }
}
