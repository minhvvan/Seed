using System;

namespace Seed.Downloader
{
    public class DownloadEvents
    {
        public event Action OnSystemInitialized;
        public void NotifyInitialized() => OnSystemInitialized?.Invoke();

        public event Action OnCatalogUpdated;
        public void NotifyCatalogUpdated() => OnCatalogUpdated?.Invoke();

        public event Action<long> OnSizeDownloaded;
        public void NotifySizeDownloaded(long size) => OnSizeDownloaded?.Invoke(size);

        public event Action<DownloadProgressStatus> OnDownloadProgress;
        public void NotifyDownloadProgress(DownloadProgressStatus status) => OnDownloadProgress?.Invoke(status);

        public event Action<bool> OnDownloadFinished;
        public void NotifyDownloadFinished(bool isSuccess) => OnDownloadFinished?.Invoke(isSuccess);
    }
}
