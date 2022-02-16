using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class SceneBootstrap : MonoBehaviour
{

    [SerializeField]
    private InputField _inputField;

    public void OnButtonClick()
    {
        if (!string.IsNullOrEmpty(_inputField.text))
        {
            Addressables.LoadSceneAsync(SceneLoading.ToName(_inputField.text));
        }
    }
}
