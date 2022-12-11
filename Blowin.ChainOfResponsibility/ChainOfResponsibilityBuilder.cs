using System.Threading.Tasks;

namespace Blowin.ChainOfResponsibility
{
    /// <summary>
    /// Factory methods.
    /// </summary>
    public static class ChainOfResponsibilityBuilder
    {
        /// <summary>
        /// Create <see cref="ChainOfResponsibilityBuilder{T,TRes}"/>.
        /// </summary>
        /// <typeparam name="T"> parameter. </typeparam>
        /// <typeparam name="TRes"> result. </typeparam>
        /// <returns><see cref="ChainOfResponsibilityBuilder{T,TRes}"/>.</returns>
        public static ChainOfResponsibilityBuilder<T, TRes> Create<T, TRes>()
        {
            return new ChainOfResponsibilityBuilder<T, TRes>();
        }

        /// <summary>
        /// Create async <see cref="ChainOfResponsibilityBuilder{T,TRes}"/>.
        /// </summary>
        /// <typeparam name="T"> parameter.</typeparam>
        /// <typeparam name="TRes"> result.</typeparam>
        /// <returns><see cref="ChainOfResponsibilityBuilder{T,TRes}"/>.</returns>
        public static ChainOfResponsibilityBuilder<AsyncRequest<T>, Task<TRes>> CreateAsync<T, TRes>()
        {
            return new ChainOfResponsibilityBuilder<AsyncRequest<T>, Task<TRes>>();
        }
    }
}
