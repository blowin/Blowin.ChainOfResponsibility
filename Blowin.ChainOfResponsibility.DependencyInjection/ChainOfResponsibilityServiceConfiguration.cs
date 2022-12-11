using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Blowin.ChainOfResponsibility.DependencyInjection
{
    /// <summary>
    /// Configuration for ChainOfResponsibility registration.
    /// </summary>
    public sealed class ChainOfResponsibilityServiceConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChainOfResponsibilityServiceConfiguration"/> class.
        /// </summary>
        /// <param name="assemblies">
        /// Assemblies analyzed.
        /// </param>
        public ChainOfResponsibilityServiceConfiguration(Assembly[] assemblies)
        {
            if (assemblies == null)
            {
                throw new ArgumentNullException(nameof(assemblies));
            }

            if (assemblies.Length == 0)
            {
                throw new ArgumentException("Assemblies should not be empty");
            }

            Assemblies = assemblies;
        }

        /// <summary>
        /// Gets the lifetime with which ChainOfResponsibility will be registered.
        /// </summary>
        public ServiceLifetime ChainOfResponsibilityLifetime { get; private set; } = ServiceLifetime.Scoped;

        /// <summary>
        /// Gets the delegate who will determine with what level of life will be registered IFinally or IMiddleware.
        ///
        /// Where Type = IMiddleware or IFinally.
        /// </summary>
        public Func<Type, ServiceLifetime> LifetimeResolver { get; private set; } = type => ServiceLifetime.Scoped;

        /// <summary>
        /// Gets the delegate who will indicates whether IMiddleware or IFinally should be registered.
        ///
        /// By default, it includes all.
        /// </summary>
        public Func<Type, bool> TypeEvaluator { get; private set; } = t => true;

        /// <summary>
        /// Gets assemblies for analysis.
        /// </summary>
        public Assembly[] Assemblies { get; }

        /// <summary>
        /// Sets ChainOfResponsibilityLifetime.
        /// </summary>
        /// <param name="lifetime">
        /// <see cref="ChainOfResponsibilityLifetime"/>.
        /// </param>
        /// <returns>
        /// Configuration.
        /// </returns>
        public ChainOfResponsibilityServiceConfiguration WithChainOfResponsibilityLifetime(ServiceLifetime lifetime)
        {
            ChainOfResponsibilityLifetime = lifetime;
            return this;
        }

        /// <summary>
        /// Sets LifetimeResolver.
        /// </summary>
        /// <param name="lifetimeResolver">.
        /// <see cref="LifetimeResolver"/>.
        /// </param>
        /// <returns>
        /// Configuration.
        /// </returns>
        public ChainOfResponsibilityServiceConfiguration WithMiddlewareLifetimeResolver(Func<Type, ServiceLifetime> lifetimeResolver)
        {
            LifetimeResolver = lifetimeResolver;
            return this;
        }

        /// <summary>
        /// Sets LifetimeResolver = <see cref="lifetime"/>.
        /// </summary>
        /// <param name="lifetime">
        /// Lifetime.
        /// </param>
        /// <returns>
        /// Configuration.
        /// </returns>
        public ChainOfResponsibilityServiceConfiguration WithMiddlewareLifetime(ServiceLifetime lifetime) => WithMiddlewareLifetimeResolver(type => lifetime);

        /// <summary>
        /// Sets LifetimeResolver = Transient.
        /// </summary>
        /// <returns>
        /// Configuration.
        /// </returns>
        public ChainOfResponsibilityServiceConfiguration WithMiddlewareTransientLifetime() => WithMiddlewareLifetime(ServiceLifetime.Transient);

        /// <summary>
        /// Sets LifetimeResolver = Scoped.
        /// </summary>
        /// <returns>
        /// Configuration.
        /// </returns>
        public ChainOfResponsibilityServiceConfiguration WithMiddlewareScopedLifetime() => WithMiddlewareLifetime(ServiceLifetime.Scoped);

        /// <summary>
        /// Sets LifetimeResolver = Singleton.
        /// </summary>
        /// <returns>
        /// Configuration.
        /// </returns>
        public ChainOfResponsibilityServiceConfiguration WithMiddlewareSingletonLifetime() => WithMiddlewareLifetime(ServiceLifetime.Singleton);

        /// <summary>
        /// Sets TypeEvaluator.
        /// </summary>
        /// <param name="typeEvaluator">.
        /// <see cref="TypeEvaluator"/>.
        /// </param>
        /// <returns>
        /// Configuration.
        /// </returns>
        public ChainOfResponsibilityServiceConfiguration WithTypeEvaluator(Func<Type, bool> typeEvaluator)
        {
            TypeEvaluator = typeEvaluator;
            return this;
        }
    }
}
