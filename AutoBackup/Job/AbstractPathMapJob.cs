using AutoBackup.Interfaces;
using AutoBackup.Model;
using JobFramework.Interfaces;
using System;
using System.Threading;

namespace AutoBackup.Job
{
    public abstract class AbstractPathMapJob : IPathMapBasedJob
    {
        public static int Precision { get; set; } = 4;
        private double _progress = 0.0;

        public IPath Source { get; set; } = new SinglePathModel();
        public IPath Target { get; set; } = new SinglePathModel();

        public virtual CancellationToken CancellationToken { get; set; }

        public virtual int ErrorNumberOfRetries { get; set; }

        public virtual int ErrorRetryDelaySeconds { get; set; }

        public int ProgressPercent => Convert.ToInt32(Math.Round(Progress * 100.0, 0));

        public double Progress => _progress;

        public string Description => $"Copy: [{Source.Path}] to [{Target.Path}]";

        public event JobProgressChangedDelegate? ProgressChanged;

        public void InitializeProgress()
        {
            _progress = 0.0;
        }

        public void UpdateProgress(double newProgress)
        {
            newProgress = Math.Round(newProgress, Precision);           //make sure our value is rounded to nearest precision

            newProgress = Math.Max(0.0, Math.Min(newProgress, 1.0));    //make sure our value is between 0.0 and 1.0

            var oldProgress = Interlocked.Exchange(ref _progress, newProgress);

            if (oldProgress != newProgress)
            {
                SendProgressChanged(oldProgress, newProgress);
            }
        }

        protected virtual void SendProgressChanged(double oldProgress, double newProgress)
        {
            ProgressChanged?.Invoke(this, oldProgress, newProgress);
        }

    }
}
