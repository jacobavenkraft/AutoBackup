using FrameworkLibrary.Collection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.FrameworkLibraryTests.Collections
{
    public class ConcurrentCollectionWrapperTests
    {
        public class MockRefObject
        {

        }

        [Fact]
        public void ConcurrentCollectionWrapper_EnqueueNullShouldRaiseArgumentNullException()
        {
            //setup
            var queue = new ConcurrentCollectionWrapper<Queue<MockRefObject>, MockRefObject>();
            //execute
            bool exceptionWasRaised = false;
            try
            {
#pragma warning disable CS8625 // disable warning, we are testing if setting null raises an exception
                queue.Enqueue(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }
            catch (ArgumentNullException)
            {
                exceptionWasRaised = true;
            }

            //assert
            Assert.True(exceptionWasRaised);
        }

        [Fact]
        public void ConcurrentCollectionWrapper_DequeueEmptyCollectionShouldRaiseInvalidOperationException()
        {
            //setup
            var queue = new ConcurrentCollectionWrapper<Queue<MockRefObject>, MockRefObject>();
            //execute
            bool exceptionWasRaised = false;
            try
            {
                var record = queue.Dequeue();
            }
            catch (InvalidOperationException)
            {
                exceptionWasRaised = true;
            }

            //assert
            Assert.True(exceptionWasRaised);
        }

        [Fact]
        public void ConcurrentCollectionWrapper_EnqueueBatchWithNullItemsShouldRaiseArgumentNullException()
        {
            //setup
            var queue = new ConcurrentCollectionWrapper<Queue<MockRefObject>, MockRefObject>();

            var batch = new MockRefObject?[] { null };

            //execute
            bool exceptionWasRaised = false;
            try
            {
#pragma warning disable CS8620 // Suppress warning because we want to test this
                queue.EnqueueBatch(batch);
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
            }
            catch (ArgumentNullException)
            {
                exceptionWasRaised = true;
            }

            //assert
            Assert.True(exceptionWasRaised);
        }

        [Fact]
        public void ConcurrentCollectionHelper_EnqueueEmptyBatchShouldNotRaiseException()
        {
            //setup
            var queue = new ConcurrentCollectionWrapper<Queue<MockRefObject>, MockRefObject>();

            var batch = new MockRefObject[] { };

            //execute
            bool exceptionWasRaised = false;
            try
            {
                queue.EnqueueBatch(batch);
            }
            catch (ArgumentNullException)
            {
                exceptionWasRaised = true;
            }

            //assert
            Assert.False(exceptionWasRaised);
        }

        [Fact]
        public void ConcurrentCollectionHelper_DequeueBatchWhenEmptyShouldNotRaiseException()
        {
            //setup
            var queue = new ConcurrentCollectionWrapper<Queue<MockRefObject>, MockRefObject>();

            //execute
            bool exceptionWasRaised = false;
            try
            {
                var batch = queue.DequeueBatch(1);
            }
            catch
            {
                exceptionWasRaised = true;
            }

            //assert
            Assert.False(exceptionWasRaised);
        }
    }
}
