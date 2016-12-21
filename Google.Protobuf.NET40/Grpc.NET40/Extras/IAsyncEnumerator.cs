using System.Threading;
using System.Threading.Tasks;

namespace System.Collections.Generic
{
    //
    // Summary:
    //     Asynchronous version of the IEnumerator<T> interface, allowing elements to be
    //     retrieved asynchronously.
    //
    // Type parameters:
    //   T:
    //     Element type.
    public interface IAsyncEnumerator<out T> : IDisposable
    {
        //
        // Summary:
        //     Gets the current element in the iteration.
        T Current { get; }

        //
        // Summary:
        //     Advances the enumerator to the next element in the sequence, returning the result
        //     asynchronously.
        //
        // Parameters:
        //   cancellationToken:
        //     Cancellation token that can be used to cancel the operation.
        //
        // Returns:
        //     Task containing the result of the operation: true if the enumerator was successfully
        //     advanced to the next element; false if the enumerator has passed the end of the
        //     sequence.
        Task<bool> MoveNext(CancellationToken cancellationToken);
    }
}