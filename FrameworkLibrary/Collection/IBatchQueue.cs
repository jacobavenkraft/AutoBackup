using System.Collections;
using System.Collections.Generic;

namespace FrameworkLibrary.Collection
{
    /// <summary>
    /// Abstraction for something that acts like a queue that can be enqueued and dequeued in batches
    /// </summary>
    /// <typeparam name="T">item type contained in the queue</typeparam>
    public interface IBatchQueue<T> : IEnumerable<T>, ICollection
    {
        void EnqueueBatch(IEnumerable<T> batch);
        IEnumerable<T> DequeueBatch(int batchSize);
    }
}
