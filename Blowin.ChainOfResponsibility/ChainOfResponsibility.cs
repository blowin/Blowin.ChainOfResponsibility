using System;
using System.Collections.Generic;
using System.Linq;
using Blowin.ChainOfResponsibility.Finally;
using Blowin.ChainOfResponsibility.Middleware;

namespace Blowin.ChainOfResponsibility
{
    /// <summary>
    /// Chain of responsibility where T is the parameter and TRes is the result of processing.
    /// </summary>
    /// <typeparam name="T">
    /// parameter.
    /// </typeparam>
    /// <typeparam name="TRes">
    /// result.
    /// </typeparam>
    public class ChainOfResponsibility<T, TRes>
    {
        private readonly Lazy<Func<T, TRes>> _lazyHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChainOfResponsibility{T, TRes}"/> class.
        /// </summary>
        /// <param name="middlewares">
        /// Sequence of middlewares.
        /// </param>
        /// <param name="finallyBlocks">
        /// Sequence with one block or empty.
        /// </param>
        public ChainOfResponsibility(IEnumerable<IMiddleware<T, TRes>> middlewares, IEnumerable<IFinally<T, TRes>> finallyBlocks)
            : this(middlewares, finallyBlocks?.FirstOrDefault())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChainOfResponsibility{T, TRes}"/> class.
        /// </summary>
        /// <param name="middlewares">
        /// Sequence of middlewares.
        /// </param>
        /// <param name="finallyBlock">
        /// Finally block.
        /// </param>
        internal ChainOfResponsibility(IEnumerable<IMiddleware<T, TRes>> middlewares, IFinally<T, TRes> finallyBlock)
        {
            _lazyHandler = new Lazy<Func<T, TRes>>(() => MergeChain(middlewares, finallyBlock));
        }

        /// <summary>
        /// Run chain.
        /// </summary>
        /// <param name="parameter">
        /// Parameter for processing.
        /// </param>
        /// <returns>
        /// Result of processing.
        /// </returns>
        public TRes Execute(T parameter) => _lazyHandler.Value(parameter);

        private static Func<T, TRes> MergeChain(IEnumerable<IMiddleware<T, TRes>> middlewares, IFinally<T, TRes> finallyBlock)
        {
            var handler = ToFunc(finallyBlock);
            var middlewareList = middlewares ?? Array.Empty<IMiddleware<T, TRes>>();
            foreach (var middleware in middlewareList.Reverse())
            {
                var handler1 = handler;
                handler = arg => middleware.Run(arg, handler1);
            }

            return handler;
        }

        private static Func<T, TRes> ToFunc(IFinally<T, TRes> @finally)
        {
            if (@finally == null)
            {
                return parameter => throw new InvalidOperationException($"Unhandled parameter '{parameter}'");
            }

            if (@finally is FuncFinally<T, TRes> value)
            {
                return value.Func;
            }

            return @finally.Run;
        }
    }
}
