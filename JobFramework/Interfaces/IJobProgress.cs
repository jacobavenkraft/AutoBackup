namespace JobFramework.Interfaces
{
    public delegate void JobProgressChangedDelegate(IJobProgress job, int oldProgress, int newProgress);

    public interface IJobProgress
    {
        event JobProgressChangedDelegate ProgressChanged;

        CancellationToken CancellationToken { get; }

        int ErrorNumberOfRetries { get; }

        int ErrorRetryDelaySeconds { get; }

        int Progress { get; }

        void InitializeProgress();

        void UpdateProgress(int newProgress);
    }
}
