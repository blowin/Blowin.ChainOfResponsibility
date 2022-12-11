using System;
using System.Collections.Generic;
using System.Threading;

namespace Blowin.ChainOfResponsibility
{
    /// <summary>
    /// Request for async middleware.
    /// </summary>
    /// <typeparam name="T">Parameter of middleware.</typeparam>
    public sealed class AsyncRequest<T> : IEquatable<AsyncRequest<T>>
    {
        /// <summary>
        /// Gets parameter of middleware.
        /// </summary>
        public T Parameter { get; }

        /// <summary>
        /// Gets cancellation token.
        /// </summary>
        public CancellationToken CancellationToken { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncRequest{T}"/> class.
        /// </summary>
        /// <param name="parameter">Parameter.</param>
        public AsyncRequest(T parameter)
            : this(parameter, CancellationToken.None)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncRequest{T}"/> class.
        /// </summary>
        /// <param name="parameter">Parameter.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public AsyncRequest(T parameter, CancellationToken cancellationToken)
        {
            Parameter = parameter;
            CancellationToken = cancellationToken;
        }

        /// <inheritdoc/>
        public bool Equals(AsyncRequest<T> other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return EqualityComparer<T>.Default.Equals(Parameter, other.Parameter) && CancellationToken.Equals(other.CancellationToken);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || (obj is AsyncRequest<T> other && Equals(other));
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                return (EqualityComparer<T>.Default.GetHashCode(Parameter) * 397) ^ CancellationToken.GetHashCode();
            }
        }
    }
}
