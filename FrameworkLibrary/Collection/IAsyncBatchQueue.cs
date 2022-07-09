using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FrameworkLibrary.Collection
{
    /// <summary>
    /// Abstraction for something that acts like a queue that can Enqueue and Dequeue batches of items
    /// and can perform those operations using the asynchronous task pattern
    /// </summary>
    /// <typeparam name="T">item type contained in the queue</typeparam>
    public interface IAsyncBatchQueue<T> : IEnumerable<T>, ICollection
    {
        Task EnqueueBatchAsync(IEnumerable<T> batch);
        Task<IEnumerable<T>> DequeueBatchAsync(int batchSize);
    }
}
