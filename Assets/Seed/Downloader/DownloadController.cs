using System;
using System.Collections;
using UnityEngine;

namespace Seed.Downloader
{
    public class DownloadController : MonoBehaviour
    {
        public enum DownloadState
        {
            Idle,
            Initialize,
            UpdateCatalog,
            DownloadSize,
            DownloadDependencies,
            Downloading,
            Finished,
        }

        [SerializeField] private string labelToDownload;
        [SerializeField] private string downloadURL;

        private AddressableDownloader _downloader;
        private Action<DownloadEvents> _onEventObtained;

        public DownloadState CurrentDownloadState { get; private set; } = DownloadState.Idle;
        public DownloadState LastValidState { get; private set; } = DownloadState.Idle;

        public IEnumerator StartDownloadCoroutine(Action<DownloadEvents> onEventObtained)
        {
            _downloader = new AddressableDownloader();
            _onEventObtained = onEventObtained;

            LastValidState = CurrentDownloadState = DownloadState.Initialize;

            while (CurrentDownloadState != DownloadState.Finished)
            {
                OnExecute();
                yield return null;
            }
        }

        public void GoNext()
        {
            CurrentDownloadState = CurrentDownloadState switch
            {
                DownloadState.Initialize => DownloadState.UpdateCatalog,
                DownloadState.UpdateCatalog => DownloadState.DownloadSize,
                DownloadState.DownloadSize => DownloadState.DownloadDependencies,
                DownloadState.Downloading or DownloadState.DownloadDependencies => DownloadState.Finished,
                _ => CurrentDownloadState
            };

            LastValidState = CurrentDownloadState;
        }

        private void OnExecute()
        {
            switch (CurrentDownloadState)
            {
                case DownloadState.Idle:
                    return;
                case DownloadState.Initialize:
                {
                    DownloadEvents events = _downloader.InitializeSystem(labelToDownload, downloadURL);
                    _onEventObtained?.Invoke(events);
                    CurrentDownloadState = DownloadState.Idle;
                    break;
                }
                case DownloadState.UpdateCatalog:
                    _downloader.UpdateCatalog();
                    CurrentDownloadState = DownloadState.Idle;
                    break;
                case DownloadState.DownloadSize:
                    _downloader.DownloadSize();
                    CurrentDownloadState = DownloadState.Idle;
                    break;
                case DownloadState.DownloadDependencies:
                    _downloader.StartDownload();
                    CurrentDownloadState = DownloadState.Downloading;
                    break;
                case DownloadState.Downloading:
                    _downloader.Update();
                    break;
                case DownloadState.Finished:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
