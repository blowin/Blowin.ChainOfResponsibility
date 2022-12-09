using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Blowin.ChainOfResponsibility_DependencyInjection
{
    public sealed class ChainOfResponsibilityServiceConfiguration
    {
        public ServiceLifetime ChainOfResponsibilityLifetime { get; private set; } = ServiceLifetime.Scoped;
        public Func<Type, ServiceLifetime> LifetimeResolver { get; private set; } = type => ServiceLifetime.Scoped;
        public Func<Type, bool> TypeEvaluator { get; private set; } = t => true;
        public Assembly[] Assemblies { get; }

        public ChainOfResponsibilityServiceConfiguration(Assembly[] assemblies)
        {
            if (assemblies == null)
                throw new ArgumentNullException(nameof(assemblies));
            if (assemblies.Length == 0)
                throw new ArgumentException("Assemblies should not be empty");

            Assemblies = assemblies;
        }

        public ChainOfResponsibilityServiceConfiguration WithChainOfResponsibilityLifetime(ServiceLifetime lifetime)
        {
            ChainOfResponsibilityLifetime = lifetime;
            return this;
        }

        public ChainOfResponsibilityServiceConfiguration WithMiddlewareLifetimeResolver(Func<Type, ServiceLifetime> lifetimeResolver)
        {
            LifetimeResolver = lifetimeResolver;
            return this;
        }

        public ChainOfResponsibilityServiceConfiguration WithMiddlewareLifetime(ServiceLifetime lifetime) => WithMiddlewareLifetimeResolver(type => lifetime);
        public ChainOfResponsibilityServiceConfiguration WithMiddlewareTransientLifetime() => WithMiddlewareLifetime(ServiceLifetime.Transient);
        public ChainOfResponsibilityServiceConfiguration WithMiddlewareScopedLifetime() => WithMiddlewareLifetime(ServiceLifetime.Scoped);
        public ChainOfResponsibilityServiceConfiguration WithMiddlewareSingletonLifetime() => WithMiddlewareLifetime(ServiceLifetime.Singleton);

        public ChainOfResponsibilityServiceConfiguration WithTypeEvaluator(Func<Type, bool> typeEvaluator)
        {
            TypeEvaluator = typeEvaluator;
            return this;
        }
    }
}
