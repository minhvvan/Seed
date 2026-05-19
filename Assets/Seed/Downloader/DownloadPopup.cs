using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Seed.Downloader
{
    public class DownloadPopup : MonoBehaviour
    {
        public enum State
        {
            None = 0,

            CalculatingSize,
            NothingToDownload,      // 다운로드 받을 게 없음
            AskingDownload,         // 다운로드 시작 여부 묻기
            Downloading,            // 다운로드 진행중
            DownloadFinished        // 다운로드 완료
        }

        [Serializable]
        public class Root
        {
            public State state;
            public Transform root;
        }

        [SerializeField] private List<Root> roots;
        [SerializeField] private TMP_Text txtTitle;
        [SerializeField] private TMP_Text txtDesc;
        [SerializeField] private TMP_Text downloadingBarStatus;
        [SerializeField] private Slider downloadProgressBar;
        [SerializeField] private Button startDownloadButton;
        [SerializeField] private Button cancelButton;
        [SerializeField] private Button enterGameButton;
        [SerializeField] private DownloadController downloader;

        private DownloadProgressStatus _progressInfo;
        private SizeUnits _sizeUnit;
        private long _curDownloadedSizeInUnit;
        private long _totalSizeInUnit;

        public State CurrentState { get; private set; } = State.None;

        private void Awake()
        {
            startDownloadButton.onClick.AddListener(OnClickStartDownload);
            cancelButton.onClick.AddListener(OnClickCancelBtn);
            enterGameButton.onClick.AddListener(OnClickEnterGame);
        }

        private IEnumerator Start()
        {
            SetState(State.CalculatingSize, true);

            yield return downloader.StartDownloadCoroutine(events =>
            {
                events.OnSystemInitialized += OnInitialized;
                events.OnCatalogUpdated += OnCatalogUpdated;
                events.OnSizeDownloaded += OnSizeDownloaded;
                events.OnDownloadProgress += OnDownloadProgress;
                events.OnDownloadFinished += OnDownloadFinished;
            });
        }

        private void OnClickStartDownload()
        {
            Debug.Log("다운로드를 시작합니다");
            SetState(State.Downloading, true);
            downloader.GoNext();
        }

        private void OnClickCancelBtn()
        {
#if UNITY_EDITOR
            if (Application.isEditor)
                UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void OnClickEnterGame()
        {
            Debug.Log("Start Game!");
            UnityEngine.SceneManagement.SceneManager.LoadScene(1);
        }

        private void SetState(State newState, bool updateUI)
        {
            Root prevRoot = roots.Find(t => t.state == CurrentState);
            Root newRoot = roots.Find(t => t.state == newState);

            CurrentState = newState;

            if (prevRoot is not null)
                prevRoot.root.gameObject.SetActive(false);

            if (newRoot is not null)
                newRoot.root.gameObject.SetActive(true);

            if (updateUI)
                UpdateUI();
        }

        private void UpdateUI()
        {
            switch (CurrentState)
            {
                case State.CalculatingSize:
                    txtTitle.text = "알림";
                    txtDesc.text = "다운로드 사이즈를 계산하고 있습니다. 잠시만 기다려주세요.";
                    break;
                case State.NothingToDownload:
                    txtTitle.text = "완료";
                    txtDesc.text = "다운로드 받을 데이터가 없습니다.";
                    break;
                case State.AskingDownload:
                    txtTitle.text = "확인";
                    txtDesc.text = $"다운로드를 시작하시겠습니까? 데이터가 많이 들 수 있습니다. <color=green>({_totalSizeInUnit}{_sizeUnit})</color>";
                    break;
                case State.Downloading:
                    txtTitle.text = "다운로드중";
                    txtDesc.text = $"다운로드중입니다. 잠시만 기다려주세요. {(_progressInfo.totalProgress * 100):0.00}% 완료";
                    downloadProgressBar.value = _progressInfo.totalProgress;
                    downloadingBarStatus.text = $"{_curDownloadedSizeInUnit}/{_totalSizeInUnit}{_sizeUnit}";
                    break;
                case State.DownloadFinished:
                    txtTitle.text = "완료";
                    txtDesc.text = "다운로드가 완료되었습니다. 게임을 시작하시겠습니까?";
                    break;
            }
        }

        private void OnInitialized() => downloader.GoNext();

        private void OnCatalogUpdated() => downloader.GoNext();

        private void OnSizeDownloaded(long size)
        {
            Debug.Log($"다운로드 사이즈 다운로드 완료! : {size} 바이트");

            if (size == 0)
            {
                SetState(State.NothingToDownload, true);
                return;
            }

            _sizeUnit = DownloadNetworkUtil.GetProperByteUnit(size);
            _totalSizeInUnit = DownloadNetworkUtil.ConvertByteByUnit(size, _sizeUnit);
            SetState(State.AskingDownload, true);
        }

        private void OnDownloadProgress(DownloadProgressStatus newInfo)
        {
            bool changed = _progressInfo.downloadedBytes != newInfo.downloadedBytes;
            _progressInfo = newInfo;

            if (!changed) return;

            UpdateUI();
            _curDownloadedSizeInUnit = DownloadNetworkUtil.ConvertByteByUnit(newInfo.downloadedBytes, _sizeUnit);
        }

        private void OnDownloadFinished(bool isSuccess)
        {
            Debug.Log($"다운로드 완료! 결과 : {isSuccess}");
            SetState(State.DownloadFinished, true);
            downloader.GoNext();
        }
    }
}
