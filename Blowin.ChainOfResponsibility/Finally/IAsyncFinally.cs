using System.Threading.Tasks;

namespace Blowin.ChainOfResponsibility.Finally
{
    /// <summary>
    /// The final async handler in the chain.
    /// </summary>
    /// <typeparam name="TIn">
    /// parameter.
    /// </typeparam>
    /// <typeparam name="TOut">
    /// result.
    /// </typeparam>
    public interface IAsyncFinally<TIn, TOut> : IFinally<AsyncRequest<TIn>, Task<TOut>>
    {
    }
}
