using System.Threading.Tasks;

namespace Blowin.ChainOfResponsibility.Middleware
{
    /// <summary>
    /// Async middleware.
    /// </summary>
    /// <typeparam name="TIn">
    /// parameter.
    /// </typeparam>
    /// <typeparam name="TOut">
    /// result.
    /// </typeparam>
    public interface IAsyncMiddleware<TIn, TOut> : IMiddleware<AsyncRequest<TIn>, Task<TOut>>
    {
    }
}
