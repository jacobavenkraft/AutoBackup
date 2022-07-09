using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FrameworkLibrary.Collection
{
    /// <summary>
    /// Abstraction for something that acts like a queue and can Enqueue, Dequeue and Peek using
    /// asynchronous task pattern
    /// </summary>
    /// <typeparam name="T">item type contained in the queue</typeparam>
    public interface IAsyncQueue<T> : IEnumerable<T>, ICollection
    {
        Task EnqueueAsync(T syncRecord);
        Task<T?> DequeueAsync();
        Task<T?> PeekAsync();
    }
}
