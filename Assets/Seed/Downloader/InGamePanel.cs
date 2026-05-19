using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Seed.Downloader
{
    public class InGamePanel : MonoBehaviour
    {
        [SerializeField] private Image resultIcon;
        [SerializeField] private TMP_Text resultLabel;
        [SerializeField] private Button goDownloadSceneButton;
        [SerializeField] private string targetSpriteKey;

        private void Awake()
        {
            goDownloadSceneButton.onClick.AddListener(OnClickGoDownloadScene);
        }

        private void Start()
        {
            Addressables.LoadAssetAsync<Sprite>(targetSpriteKey).Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    resultLabel.text = "Succeed";
                    resultIcon.sprite = handle.Result;
                }
                else
                {
                    resultLabel.text = "Fail";
                    resultIcon.sprite = null;
                }
            };
        }

        private void OnClickGoDownloadScene()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }
}