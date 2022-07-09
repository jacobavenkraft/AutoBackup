using FrameworkLibrary.Collection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FrameworkLibrary.Scheduler
{
    /// <summary>
    /// This is a task scheduler designed to limit the parallel processing of tasks using this scheduler,
    /// it is taken almost verbatim from the Microsoft topic relating to the TaskScheduler Class
    /// https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.taskscheduler?view=net-6.0
    /// </summary>
    public class LimitedConcurrencyTaskScheduler<T> : TaskScheduler where T : class, IEnumerable<Task>, ICollection, new()
    {
        // Indicates whether the current thread is processing work items.
        [ThreadStatic]
        private static bool _currentThreadIsProcessingItems;

        // The list of tasks to be executed
        private readonly T _tasks = new T(); // protected by lock(_tasks)

        // The maximum concurrency level allowed by this scheduler.
        private readonly int _maximumConcurrentTasks;

        // Indicates whether the scheduler is currently processing work items.
        private int _delegatesQueuedOrRunning = 0;

        public LimitedConcurrencyTaskScheduler(int maximumConcurrentTasks)
        {
            if (maximumConcurrentTasks < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maximumConcurrentTasks));
            }

            _maximumConcurrentTasks = maximumConcurrentTasks;
        }

        public bool ProcessingEnabled { get; set; } = true;

        public int WaitingTaskCount
        {
            get
            {
                lock (_tasks)
                {
                    return _tasks.Count;
                }
            }
        }

        // Gets the maximum concurrency level supported by this scheduler.
        public sealed override int MaximumConcurrencyLevel => _maximumConcurrentTasks;

        protected T TaskCollection => _tasks;

        // Queues a task to the scheduler.
        protected sealed override void QueueTask(Task task)
        {
            // Add the task to the list of tasks to be processed.  If there aren't enough
            // delegates currently queued or running to process tasks, schedule another.
            lock (_tasks)
            {
                DoEnqueueTask(task);
                if (_delegatesQueuedOrRunning < _maximumConcurrentTasks)
                {
                    ++_delegatesQueuedOrRunning;
                    NotifyThreadPoolOfPendingWork();
                }
            }
        }

        // Attempts to execute the specified task on the current thread.
        protected sealed override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            // If this thread isn't already processing a task, we don't support inlining
            if (!_currentThreadIsProcessingItems)
            {
                return false;
            }

            // If the task was previously queued, remove it from the queue
            if (taskWasPreviouslyQueued)
            {
                // Try to run the task.
                if (TryDequeue(task))
                {
                    return base.TryExecuteTask(task);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return base.TryExecuteTask(task);
            }
        }

        // Attempt to remove a previously scheduled task from the scheduler.
        protected sealed override bool TryDequeue(Task task)
        {
            lock (_tasks)
            {
                switch (_tasks)
                {
                    case Queue<Task> queue:
                        if (queue.Peek() == task)
                        {
                            queue.Dequeue();
                            return true;
                        }
                        else
                        {
                            return false;
                        }

                    case Stack<Task> stack:
                        if (stack.Peek() == task)
                        {
                            stack.Pop();
                            return true;
                        }
                        else
                        {
                            return false;
                        }

                    case IQueue<Task> iQueue:
                        if (iQueue.Peek() == task)
                        {
                            iQueue.Dequeue();
                            return true;
                        }
                        else
                        {
                            return false;
                        }

                    case ICollection<Task> collection:
                        return collection.Remove(task);
                    default:
                        break;
                }
            }

            return false;
        }

        // Gets an enumerable of the tasks currently scheduled on this scheduler.
        protected sealed override IEnumerable<Task> GetScheduledTasks()
        {
            bool lockTaken = false;
            try
            {
                Monitor.TryEnter(_tasks, ref lockTaken);

                if (lockTaken)
                {
                    return _tasks;
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
                    Monitor.Exit(_tasks);
                }
            }
        }

        // Inform the ThreadPool that there's work to be executed for this scheduler.
        private void NotifyThreadPoolOfPendingWork()
        {
            ThreadPool.UnsafeQueueUserWorkItem(_ =>
            {
                // Note that the current thread is now processing work items.
                // This is necessary to enable inlining of tasks into this thread.
                _currentThreadIsProcessingItems = true;
                try
                {
                    // Process all available items in the queue.
                    while (true)
                    {
                        if (!ProcessingEnabled)
                        {
                            Thread.Sleep(TimeSpan.FromSeconds(1));
                            continue;
                        }

                        Task item;
                        lock (_tasks)
                        {
                            // When there are no more items to be processed,
                            // note that we're done processing, and get out.
                            if (_tasks.Count() == 0)
                            {
                                --_delegatesQueuedOrRunning;
                                break;
                            }

                            // Get the next item from the queue
                            item = DoDequeueTask();
                        }

                        // Execute the task we pulled out of the queue
                        base.TryExecuteTask(item);
                    }
                }
                // We're done processing items on the current thread
                finally { _currentThreadIsProcessingItems = false; }
            }, null);
        }

        private void DoEnqueueTask(Task task)
        {
            if (task == null)
            {
                return;
            }

            switch (_tasks)
            {
                case Queue<Task> queue:
                    queue.Enqueue(task);
                    break;
                case Stack<Task> stack:
                    stack.Push(task);
                    break;
                case IQueue<Task> iQueue:
                    iQueue.Enqueue(task);
                    break;
                case ICollection<Task> collection:
                    collection.Add(task);
                    break;
                default:
                    break;
            }
        }

        private Task DoDequeueTask()
        {
            switch (_tasks)
            {
                
                case Queue<Task> queue:
                    return queue.Dequeue();
                case Stack<Task> stack:
                    return stack.Pop();
                case IQueue<Task> iQueue:
                    return iQueue.Dequeue();
                case ICollection<Task> collection:
                    var task = collection.FirstOrDefault() 
                        ?? throw new InvalidOperationException("Cannot Dequeue an empty collection.");
                    collection.Remove(task);
                    return task;
                default:
                    break;
            }

            throw new InvalidOperationException("Cannot Dequeue an empty collection.");
        }
    }
}
