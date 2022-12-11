using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Blowin.ChainOfResponsibility.Finally;
using Blowin.ChainOfResponsibility.Middleware;

namespace Blowin.ChainOfResponsibility
{
    /// <summary>
    /// A class for creating ChainOfResponsibility.
    /// </summary>
    /// <typeparam name="T">
    /// parameter.
    /// </typeparam>
    /// <typeparam name="TRes">
    /// result.
    /// </typeparam>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "The naming follows the rules for naming generic types.")]
    public sealed class ChainOfResponsibilityBuilder<T, TRes>
    {
        private readonly List<IMiddleware<T, TRes>> _middlewares = new List<IMiddleware<T, TRes>>();
        private IFinally<T, TRes> _finally;

        /// <summary>
        /// Adds finally block to builder.
        /// </summary>
        /// <param name="finally">
        /// Finally block.
        /// </param>
        /// <returns>
        /// Builder.
        /// </returns>
        public ChainOfResponsibilityBuilder<T, TRes> WithFinally(IFinally<T, TRes> @finally)
        {
            _finally = @finally;
            return this;
        }

        /// <summary>
        /// Adds middleware to builder.
        /// </summary>
        /// <param name="middleware">
        /// Middleware.
        /// </param>
        /// <returns>
        /// Builder.
        /// </returns>
        public ChainOfResponsibilityBuilder<T, TRes> WithMiddleware(IMiddleware<T, TRes> middleware)
        {
            _middlewares.Add(middleware);
            return this;
        }

        /// <summary>
        /// Adds finally block (delegate) to builder.
        /// </summary>
        /// <param name="finally">
        /// Finally block as delegate.
        /// </param>
        /// <returns>
        /// Builder.
        /// </returns>
        public ChainOfResponsibilityBuilder<T, TRes> WithFinally(Func<T, TRes> @finally)
        {
            return WithFinally(new FuncFinally<T, TRes>(@finally));
        }

        /// <summary>
        /// Adds middleware (delegate) to builder.
        /// </summary>
        /// <param name="middleware">
        /// Middleware as delegate.
        /// </param>
        /// <returns>
        /// Builder.
        /// </returns>
        public ChainOfResponsibilityBuilder<T, TRes> WithMiddleware(Func<T, Func<T, TRes>, TRes> middleware)
        {
            return WithMiddleware(new FuncMiddleware<T, TRes>(middleware));
        }

        /// <summary>
        /// Build <see cref="ChainOfResponsibility"/>.
        /// </summary>
        /// <returns>
        /// Create <see cref="ChainOfResponsibility"/>.
        /// </returns>
        public ChainOfResponsibility<T, TRes> Build()
        {
            return new ChainOfResponsibility<T, TRes>(_middlewares, _finally);
        }
    }
}
