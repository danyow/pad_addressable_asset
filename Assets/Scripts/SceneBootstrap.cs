using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class SceneBootstrap : MonoBehaviour
{

    public void OnButtonClick(string sceneName)
    {
        Addressables.LoadSceneAsync(SceneLoading.ToName(sceneName));
    }
}
