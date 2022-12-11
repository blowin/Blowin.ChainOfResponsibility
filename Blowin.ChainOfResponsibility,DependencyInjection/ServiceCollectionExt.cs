using System;
using System.Linq;
using System.Reflection;
using Blowin.ChainOfResponsibility;
using Blowin.ChainOfResponsibility.Finally;
using Blowin.ChainOfResponsibility.Middleware;
using Microsoft.Extensions.DependencyInjection;

namespace Blowin.ChainOfResponsibility_DependencyInjection
{
    /// <summary>
    /// Extension for IServiceCollection.
    /// </summary>
    public static class ServiceCollectionExt
    {
        /// <summary>
        /// Adds ChainOfResponsibility with <see cref="IMiddleware{TIn,TOut}"/> and <see cref="IFinally{TIn,TOut}"/> in <param name="assembly"></param> to IServiceCollection.
        /// </summary>
        /// <param name="self">
        /// IServiceCollection.
        /// </param>
        /// <param name="assembly">
        /// Assembly for analysis.
        /// </param>
        /// <param name="configurationAction">
        /// Delegate which allows you to configure the registration.
        /// </param>
        /// <returns>
        /// Configured IServiceCollection.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// When assembly = null.
        /// </exception>
        public static IServiceCollection AddChainOfResponsibility(
            this IServiceCollection self,
            Assembly assembly,
            Action<ChainOfResponsibilityServiceConfiguration> configurationAction = null)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            return self.AddChainOfResponsibility(new[] { assembly }, configurationAction);
        }

        /// <summary>
        /// Adds ChainOfResponsibility with <see cref="IMiddleware{TIn,TOut}"/> and <see cref="IFinally{TIn,TOut}"/> in <param name="assemblies"></param> to IServiceCollection.
        /// </summary>
        /// <param name="self">
        /// IServiceCollection.
        /// </param>
        /// <param name="assemblies">
        /// Assemblies for analysis.
        /// </param>
        /// <param name="configurationAction">
        /// Delegate which allows you to configure the registration.
        /// </param>
        /// <returns>
        /// Configured IServiceCollection.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// When assembly = null.
        /// </exception>
        public static IServiceCollection AddChainOfResponsibility(this IServiceCollection self, Assembly[] assemblies, Action<ChainOfResponsibilityServiceConfiguration> configurationAction = null)
        {
            var configuration = new ChainOfResponsibilityServiceConfiguration(assemblies);
            configurationAction?.Invoke(configuration);

            foreach (var assembly in configuration.Assemblies)
            {
                self.AddChainOfResponsibility(assembly.GetTypes(), configuration);
            }

            self.Add(new ServiceDescriptor(typeof(ChainOfResponsibility<,>), typeof(ChainOfResponsibility<,>), configuration.ChainOfResponsibilityLifetime));

            return self;
        }

        private static void AddChainOfResponsibility(this IServiceCollection self, Type[] types, ChainOfResponsibilityServiceConfiguration configuration)
        {
            var middlewareType = typeof(IMiddleware<,>);
            var finallyType = typeof(IFinally<,>);

            foreach (var asmType in types)
            {
                if (asmType.IsAbstract || asmType.IsInterface || (!asmType.IsPublic && !asmType.IsNestedPublic))
                {
                    continue;
                }

                var interfaces = FilterInterfaces(asmType, middlewareType, finallyType);

                if (!interfaces.Any(i => i.Finally || i.Middleware) || !configuration.TypeEvaluator(asmType))
                {
                    continue;
                }

                foreach (var (iType, _, _) in interfaces)
                {
                    var lifetime = configuration.LifetimeResolver(asmType);
                    self.Add(new ServiceDescriptor(iType, asmType, lifetime));
                }
            }
        }

        private static (Type Type, bool Middleware, bool Finally)[] FilterInterfaces(this Type asmType, Type middlewareType1, Type finallyType1)
        {
            return asmType.GetInterfaces().Where(i => i.IsGenericType)
                .Select(i =>
                {
                    var type = i.GetGenericTypeDefinition();
                    return (Type: i, Middleware: type == middlewareType1, Finally: type == finallyType1);
                })
                .Where(i => i.Middleware || i.Finally)
                .ToArray();
        }
    }
}
