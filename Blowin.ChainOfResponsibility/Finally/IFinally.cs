namespace Blowin.ChainOfResponsibility.Finally
{
    /// <summary>
    /// The final handler in the chain.
    /// </summary>
    /// <typeparam name="TIn">
    /// parameter.
    /// </typeparam>
    /// <typeparam name="TOut">
    /// result.
    /// </typeparam>
    public interface IFinally<in TIn, out TOut>
    {
        /// <summary>
        /// Request handler.
        /// </summary>
        /// <param name="parameter">
        /// parameter.
        /// </param>
        /// <returns>
        /// result.
        /// </returns>
        TOut Run(TIn parameter);
    }
}
