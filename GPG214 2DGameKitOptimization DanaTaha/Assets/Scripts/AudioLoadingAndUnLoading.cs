using System.Collections;
using UnityEngine;
using System.IO;
using UnityEngine.Audio;

public class AudioLoadingAndUnLoading : MonoBehaviour
{
    #region Variables.
    public string assetBundleName = "ellenrangedattacks";
    public string folderName = "RangedAttacksBundle";
    public string[] audioClipNames = { "EllenRangedAttack01", "EllenRangedAttack02", "EllenRangedAttack03", "EllenRangedAttack04" };

    private AssetBundle _audioAssetBundle;
    private AudioSource _audioSource;
    #endregion

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            StartCoroutine(LoadAssetBundleFromFile());
        }
    }

    #region Private Functions.
    private IEnumerator LoadAssetBundleFromFile()
    {
        string _bundlePath = Path.Combine(Application.streamingAssetsPath, folderName, assetBundleName);
        AssetBundleCreateRequest _bundleRequest = AssetBundle.LoadFromFileAsync(_bundlePath);
        yield return _bundleRequest;

        _audioAssetBundle = _bundleRequest.assetBundle;

        if (_audioAssetBundle == null)
        {
            Debug.LogError("failed to load AssetBundle from: " + _bundlePath);
            yield break;
        }

        Debug.Log("AssetBundle is loaded from file: " + _bundlePath);
        foreach (string _clipName in audioClipNames)
        {
            AssetBundleRequest _audioRequest = _audioAssetBundle.LoadAssetAsync<AudioClip>(_clipName);
            yield return _audioRequest;

            AudioClip _audioClip = _audioRequest.asset as AudioClip;

            if (_audioClip != null)
            {
                Debug.Log("Loaded clip: " + _clipName);

                _audioSource.clip = _audioClip;
                _audioSource.Play();

                yield return new WaitForSeconds(_audioClip.length);
            }
            else
            {
                Debug.Log("Failed to load clip: " + _clipName);
            }
        }

        _audioAssetBundle.Unload(true);
        Debug.Log("AssetBundle unloaded.");
    }

    #endregion
}

