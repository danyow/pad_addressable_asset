using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace AddressablesPlayAssetDelivery
{
    public class ButtonInteraction : MonoBehaviour
    {
        //        [SerializeField] private AssetReference reference;
        [SerializeField]
        private Transform parent;
        [SerializeField]
        private string _type;
        private bool        _isLoading;
        private GameObject  _obj;
        private AudioClip   _clip;
        private Image       _image;
        private AudioSource _audioSource;

        private void Awake()
        {
            _image            = GetComponent<Image>();
            _image.fillAmount = 0;
            _audioSource      = GetComponent<AudioSource>();
        }

        public void OnButtonClicked()
        {
            switch (_isLoading)
            {
                case true:
                    Debug.LogError("Loading operation currently in progress.");
                    break;
                case false when _clip == null:
                    // Load the object
                    StartCoroutine(Instantiate());
                    break;
                case false:
                    // Unload the object
                    Addressables.ReleaseInstance(_obj);
                    Addressables.Release(_clip);
                    _obj  = null;
                    _clip = null;
                    _audioSource.Stop();
                    break;
            }
        }

        private IEnumerator Instantiate()
        {
            _isLoading = true;
            var prefabPath = $"Assets/Prefabs/{_type}/Prefab.prefab";
            var asyncTask  = Khepri.AssetDelivery.AddressablesAssetDelivery.GetDownloadSizeAsync(prefabPath);
            while (!asyncTask.IsDone)
            {
                yield return null;
            }
            var isCached = asyncTask.Status == AsyncOperationStatus.Succeeded && asyncTask.Result == 0;
            if (!isCached)
            {
                var downloadAsyncTask = Addressables.DownloadDependenciesAsync(prefabPath);
                while (!downloadAsyncTask.IsDone)
                {
                    _image.fillAmount = downloadAsyncTask.PercentComplete;
                    yield return null;
                }
            }
            var prefabHandle = Addressables.InstantiateAsync(prefabPath, parent);
            while (!prefabHandle.IsDone)
            {
                _image.fillAmount = prefabHandle.PercentComplete;
                yield return null;
            }
            _image.fillAmount = 0;
            var musicPath = $"Assets/Prefabs/{_type}/Music.mp3";
            asyncTask = Khepri.AssetDelivery.AddressablesAssetDelivery.GetDownloadSizeAsync(musicPath);
            while (!asyncTask.IsDone)
            {
                yield return null;
            }
            isCached = asyncTask.Status == AsyncOperationStatus.Succeeded && asyncTask.Result == 0;
            if (!isCached)
            {
                var downloadAsyncTask = Addressables.DownloadDependenciesAsync(prefabPath);
                while (!downloadAsyncTask.IsDone)
                {
                    _image.fillAmount = downloadAsyncTask.PercentComplete;
                    yield return null;
                }
            }
            // Khepri.AssetDelivery.AddressablesAssetDelivery.IsPack(musicPath)
            var musicHandle = Addressables.LoadAssetAsync<AudioClip>(musicPath);
            while (!musicHandle.IsDone)
            {
                _image.fillAmount = musicHandle.PercentComplete;
                yield return null;
            }
            _image.fillAmount = 1;
            // _obj              = prefabHandle.Result;
            _clip             = musicHandle.Result;
            _audioSource.clip = musicHandle.Result;
            _audioSource.Play();
            _isLoading = false;
        }
    }
}
