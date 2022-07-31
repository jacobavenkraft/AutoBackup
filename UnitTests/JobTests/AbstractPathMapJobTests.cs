using AutoBackup.Job;
using System.Collections.Generic;
using Xunit;

namespace UnitTests.JobTests
{
    public class AbstractPathMapJobTests
    {
        public class MockConcretePathMapJob: AbstractPathMapJob
        {

        }

        [Fact]
        public void AbstractPathMapJob_UpdateProgress_NoChange_DoesNotFireProgressChangedEvent()
        {
            //setup
            var pathMapJob = new MockConcretePathMapJob();
            pathMapJob.UpdateProgress(0.45);
            int progressChangeEventFiredCount = 0;

            pathMapJob.ProgressChanged += (sender, oldValue, newValue) => progressChangeEventFiredCount++;

            //execute            

            pathMapJob.UpdateProgress(0.45);
            pathMapJob.UpdateProgress(0.45);

            //assert
            Assert.Equal(0, progressChangeEventFiredCount);
        }

        [Fact]
        public void AbstractPathMapJob_UpdateProgress_NewValue_DoesFireProgressChangedEvent()
        {
            //setup
            var pathMapJob = new MockConcretePathMapJob();
            var expectedOldValue = 0.45;
            var expectedNewValue = 0.55;

            pathMapJob.UpdateProgress(expectedOldValue);

            int progressChangeEventFiredCount = 0;
            double progressOldValue = 0.0;
            double progressNewValue = 0.0;

            pathMapJob.ProgressChanged += (sender, oldValue, newValue) =>
            {
                progressOldValue = oldValue;
                progressNewValue = newValue;
                progressChangeEventFiredCount++;
            };

            //execute            

            pathMapJob.UpdateProgress(expectedNewValue);

            //assert
            Assert.Equal(1, progressChangeEventFiredCount);
            Assert.Equal(expectedOldValue, progressOldValue);
            Assert.Equal(expectedNewValue, progressNewValue);
            Assert.Equal(expectedNewValue, pathMapJob.Progress);
        }

        [Fact]
        public void AbstractPathMapJob_UpdateProgress_NewValue_CannotExceed_1()
        {
            //setup
            var pathMapJob = new MockConcretePathMapJob();
            var expectedOldValue = 0.45;
            var expectedNewValue = 1.0;

            pathMapJob.UpdateProgress(expectedOldValue);

            int progressChangeEventFiredCount = 0;
            double progressOldValue = 0.0;
            double progressNewValue = 0.0;

            pathMapJob.ProgressChanged += (sender, oldValue, newValue) =>
            {
                progressOldValue = oldValue;
                progressNewValue = newValue;
                progressChangeEventFiredCount++;
            };

            //execute            

            pathMapJob.UpdateProgress(1.5);
            pathMapJob.UpdateProgress(2.0);
            pathMapJob.UpdateProgress(3.5);

            //assert
            Assert.Equal(1, progressChangeEventFiredCount);
            Assert.Equal(expectedOldValue, progressOldValue);
            Assert.Equal(expectedNewValue, progressNewValue);
            Assert.Equal(expectedNewValue, pathMapJob.Progress);
        }

        [Fact]
        public void AbstractPathMapJob_UpdateProgress_NewValue_CannotFallBelow_0()
        {
            //setup
            var pathMapJob = new MockConcretePathMapJob();
            var expectedOldValue = 0.45;
            var expectedNewValue = 0.0;

            pathMapJob.UpdateProgress(expectedOldValue);

            int progressChangeEventFiredCount = 0;
            double progressOldValue = 1.0;
            double progressNewValue = 1.0;

            pathMapJob.ProgressChanged += (sender, oldValue, newValue) =>
            {
                progressOldValue = oldValue;
                progressNewValue = newValue;
                progressChangeEventFiredCount++;
            };

            //execute            

            pathMapJob.UpdateProgress(-1.0);
            pathMapJob.UpdateProgress(-2.0);
            pathMapJob.UpdateProgress(-3.5);

            //assert
            Assert.Equal(1, progressChangeEventFiredCount);
            Assert.Equal(expectedOldValue, progressOldValue);
            Assert.Equal(expectedNewValue, progressNewValue);
            Assert.Equal(expectedNewValue, pathMapJob.Progress);
        }

        [Theory]
        [InlineData(0.45123456789, 4, 0.4512)]
        [InlineData(0.45123456789, 3, 0.451)]
        [InlineData(0.45123456789, 2, 0.45)]
        [InlineData(0.45123456789, 5, 0.45123)]
        [InlineData(0.45123456789, 6, 0.451235)]
        [InlineData(0.45123456789, 7, 0.4512346)]
        [InlineData(0.45123456789, 8, 0.45123457)]
        [InlineData(0.45123456789, 9, 0.451234568)]
        [InlineData(0.45123456789, 10, 0.4512345679)]
        [InlineData(0.45123456789, 11, 0.45123456789)]
        public void AbstractPathMapJob_UpdateProgress_ObeysPrecision(double newProgressValue, int precision, double expectedProgressResult)
        {
            //setup
            var pathMapJob = new MockConcretePathMapJob();
            AbstractPathMapJob.Precision = precision;

            //execute            

            pathMapJob.UpdateProgress(newProgressValue);

            //assert            
            Assert.Equal(expectedProgressResult, pathMapJob.Progress);
        }

        [Fact]
        public void AbstractPathMapJob_PercentProgress_FollowsProgress()
        {
            //setup
            List<int> percentList = new List<int>();
            var pathMapJob = new MockConcretePathMapJob();
            double incrementValue = 0.01;

            //execute
            while(pathMapJob.Progress < 1.0)
            {
                pathMapJob.UpdateProgress(pathMapJob.Progress + incrementValue);
                percentList.Add(pathMapJob.ProgressPercent);
            }

            //assert
            Assert.Equal(100, percentList.Count);

            for (int i = 1; i <= 100; i++)
            {
                Assert.Equal(i, percentList[i - 1]);
            }
        }
    }
}
