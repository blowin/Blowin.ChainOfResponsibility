using System;

namespace Blowin.ChainOfResponsibility.Middleware
{
    /// <summary>
    /// Middleware for ChainOfResponsibility.
    /// </summary>
    /// <typeparam name="TIn">
    /// parameter.
    /// </typeparam>
    /// <typeparam name="TOut">
    /// result.
    /// </typeparam>
    public interface IMiddleware<TIn, TOut>
    {
        /// <summary>
        /// Request handler.
        /// </summary>
        /// <param name="parameter">
        /// parameter.
        /// </param>
        /// <param name="next">
        /// The next handler in the chain.
        /// </param>
        /// <returns>
        /// result.
        /// </returns>
        TOut Run(TIn parameter, Func<TIn, TOut> next);
    }
}
