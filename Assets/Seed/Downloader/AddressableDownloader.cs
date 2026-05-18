using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Seed.Downloader
{
    public class AddressableDownloader
    {
        public static string DownloadURL;

        private DownloadEvents _events;
        private string _labelToDownload;
        private long _totalSize;
        private AsyncOperationHandle _downloadHandle;

        public DownloadEvents InitializeSystem(string label, string downloadUrl)
        {
            _events = new DownloadEvents();
            _labelToDownload = label;
            DownloadURL = downloadUrl;

            ResourceManager.ExceptionHandler += OnException;
            Addressables.InitializeAsync().Completed += OnInitialized;
            return _events;
        }
        public void Update()
        {
            if (_downloadHandle.IsValid() && !_downloadHandle.IsDone && _downloadHandle.Status != AsyncOperationStatus.Failed)
            {
                var status = _downloadHandle.GetDownloadStatus();
                var currentDownloadedSize = status.DownloadedBytes;
                var remainingSize = _totalSize - currentDownloadedSize;

                _events.NotifyDownloadProgress(new DownloadProgressStatus(
                    currentDownloadedSize,
                    _totalSize,
                    remainingSize,
                    status.Percent));
            }
        }

        public void UpdateCatalog()
        {
            Addressables.CheckForCatalogUpdates().Completed += (result) =>
            {
                var catalogToUpdate = result.Result;
                if (catalogToUpdate.Count > 0)
                {
                    Addressables.UpdateCatalogs(catalogToUpdate).Completed += OnCatalogUpdate;
                }
                else
                {
                    _events.NotifyCatalogUpdated();
                }
            };
        }

        public void DownloadSize()
        {
            Addressables.GetDownloadSizeAsync(_labelToDownload).Completed += OnSizeDownloaded;
        }

        public void StartDownload()
        {
            _downloadHandle = Addressables.DownloadDependenciesAsync(_labelToDownload);
            _downloadHandle.Completed += OnDependenciesDownloaded;
        }

        private void OnInitialized(AsyncOperationHandle<IResourceLocator> result)
        {
            _events.NotifyInitialized();
        }

        private void OnCatalogUpdate(AsyncOperationHandle<List<IResourceLocator>> obj)
        {
            _events.NotifyCatalogUpdated();
        }

        private void OnSizeDownloaded(AsyncOperationHandle<long> result)
        {
            _totalSize = result.Result;
            _events.NotifySizeDownloaded(_totalSize);
        }

        private void OnDependenciesDownloaded(AsyncOperationHandle result)
        {
            _events.NotifyDownloadFinished(result.Status == AsyncOperationStatus.Succeeded);
        }
        
        private void OnException(AsyncOperationHandle handle, Exception exp)
        {
            Debug.LogError("CustomExceptionCaught: " + exp.Message);
        }
    }
}
