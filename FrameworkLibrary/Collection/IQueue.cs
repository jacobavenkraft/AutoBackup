using System.Collections;
using System.Collections.Generic;

namespace FrameworkLibrary.Collection
{
    /// <summary>
    /// Abstraction for something that acts like a queue
    /// </summary>
    /// <typeparam name="T">item type contained in the queue</typeparam>
    public interface IQueue<T> : IEnumerable<T>, ICollection
    {
        void Enqueue(T syncRecord);
        T Dequeue();
        T Peek();
    }
}
