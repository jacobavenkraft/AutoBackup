namespace JobFramework.Interfaces
{
    public delegate void JobProgressChangedDelegate(IJobProgress job, double oldProgress, double newProgress);

    public interface IJobProgress
    {
        event JobProgressChangedDelegate? ProgressChanged;

        CancellationToken CancellationToken { get; }

        int ErrorNumberOfRetries { get; }

        int ErrorRetryDelaySeconds { get; }

        int ProgressPercent { get; }

        double Progress { get; }

        void InitializeProgress();

        void UpdateProgress(double newProgress);
    }
}
