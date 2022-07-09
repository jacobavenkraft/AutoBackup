using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FrameworkLibrary.Collection
{
    /// <summary>
    /// Generic wrapper around a collection which provides concurrent access through Queue-based interfaces.
    /// NOTE: although external access to the wrapper is through queue-based Enqueue, Dequeue operations, the
    /// collection that gets wrapped may be anything, a Queue, a Stack, a List, etc.
    /// NOTE: this supports the Monitor.Wait/Monitor.Pulse pattern for waking up when a collection changes,
    /// interested outside components can lock on SyncRoot and this wrapper will Monitor.Pulse the lock when
    /// items are added to the internal collection
    /// </summary>
    /// <typeparam name="TCollectionType"></typeparam>
    /// <typeparam name="TItemType"></typeparam>
    public class ConcurrentCollectionWrapper<TCollectionType, TItemType> : IQueue<TItemType>, IAsyncQueue<TItemType>, IBatchQueue<TItemType>, IAsyncBatchQueue<TItemType> where TCollectionType : class, IEnumerable<TItemType>, ICollection, new()
    {
        private readonly object _lock = new object();
        private readonly TCollectionType _collection = new TCollectionType();

        public int Count
        {
            get
            {
                lock (_lock)
                {
                    return _collection.Count;
                }
            }
        }

        protected TCollectionType Collection => _collection;

        #region IBatchQueue

        public IEnumerable<TItemType> DequeueBatch(int batchSize)
        {
            var batch = new List<TItemType>();

            lock (_lock)
            {
                while (Count > 0 && batch.Count < batchSize)
                {
                    batch.Add(InternalDequeue());
                }
            }

            return batch;
        }

        public void EnqueueBatch(IEnumerable<TItemType> batch)
        {
            if (!batch.Any())
            {
                return;
            }

            lock (_lock)
            {
                foreach (var item in batch)
                {
                    if (item == null)
                    {
                        throw new ArgumentNullException(nameof(item));
                    }

                    InternalEnqueue(item);
                }

                Monitor.Pulse(_lock);
            }
        }

        #endregion IBatchQueue

        #region IAsyncBatchQueue

        public async Task EnqueueBatchAsync(IEnumerable<TItemType> batch) => await Task.Run(() => EnqueueBatch(batch));

        public async Task<IEnumerable<TItemType>> DequeueBatchAsync(int batchSize) => await Task.Run(() => DequeueBatch(batchSize));

        #endregion IAsyncBatchQueue

        #region IQueue

        public void Enqueue(TItemType item) => EnqueueBatch(new TItemType[] { item });

        public TItemType Dequeue()
        {
            if (Count == 0)
            {
                throw new InvalidOperationException("Cannot call Dequeue on an empty queue.");
            }

            return DequeueBatch(1).FirstOrDefault()
                ?? throw new InvalidOperationException("Cannot call Dequeue on an empty queue.");
        }

        public TItemType Peek()
        {
            lock (_lock)
            {
                return InternalPeek();
            }
        }

        #endregion IQueue

        #region IAsyncQueue

        public async Task EnqueueAsync(TItemType item) => await Task.Run(() => Enqueue(item));

        public async Task<TItemType?> DequeueAsync() => await Task.Run(() => Dequeue());

        public async Task<TItemType?> PeekAsync() => await Task.Run(() => Peek());

        #endregion IAsyncQueue

        #region ICollection

        bool ICollection.IsSynchronized => true;

        object ICollection.SyncRoot => _lock;

        void ICollection.CopyTo(Array array, int index)
        {
            lock(_lock)
            {
                _collection.CopyTo(array, index);
            }
        }

        #endregion ICollection

        public IEnumerator<TItemType> GetEnumerator()
        {
            bool lockTaken = false;
            try
            {
                Monitor.TryEnter(_lock, ref lockTaken);

                if (lockTaken)
                {
                    return _collection.GetEnumerator();
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
            finally
            {
                if (lockTaken)
                {
                    Monitor.Exit(_lock);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => (this as IEnumerable<TItemType>).GetEnumerator();

        protected virtual TItemType InternalDequeue()
        {
            switch (_collection)
            {
                case Queue<TItemType> queue:
                    return queue.Dequeue();
                case Stack<TItemType> stack:
                    return stack.Pop();
                case IQueue<TItemType> iQueue:
                    return iQueue.Dequeue();
                case ICollection<TItemType> collection:
                    {
                        var item = collection.FirstOrDefault();
                        
                        if (item != null)
                        {
                            collection.Remove(item);
                            return item;
                        }

                        throw new InvalidOperationException("Cannot call Dequeue on an empty collection.");
                    }

                default:
                    break;
            }

            throw new InvalidOperationException("Cannot call Dequeue on an empty collection.");
        }

        protected virtual void InternalEnqueue(TItemType item)
        {
            switch (_collection)
            {
                case Queue<TItemType> queue:
                    queue.Enqueue(item);
                    break;
                case Stack<TItemType> stack:
                    stack.Push(item);
                    break;
                case IQueue<TItemType> iQueue:
                    iQueue.Enqueue(item);
                    break;
                case ICollection<TItemType> collection:
                    {
                        collection.Add(item);
                    }

                    break;
            }
        }

        protected virtual TItemType InternalPeek()
        {
            switch (_collection)
            {
                case Queue<TItemType> queue:
                    return queue.Peek();
                case Stack<TItemType> stack:
                    return stack.Peek();
                case IQueue<TItemType> iQueue:
                    return iQueue.Peek();
                case ICollection<TItemType> collection:
                    return collection.FirstOrDefault()
                        ?? throw new InvalidOperationException("Cannot call Peek on an empty queue.");
                default:
                    break;
            }

            throw new InvalidOperationException("Cannot call Peek on an empty queue.");
        }
    }
}
