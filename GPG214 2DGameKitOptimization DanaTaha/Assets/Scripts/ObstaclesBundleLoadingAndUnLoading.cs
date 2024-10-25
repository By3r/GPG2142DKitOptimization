using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Gamekit2D.DanaChanges
{
    public class ObstaclesBundleLoadingAndUnLoading : MonoBehaviour
    {
        #region Variables
        public string assetBundleName = "obstaclewall";
        public string obstacleName = "DestructableColumn";
        public string folderName = "Obstacle";
        public Transform player;
        public Transform mainAssetSpawnPoint;
        public Transform[] assetSpawnPoints;
        public float loadingDistance = 10f;
        public float unloadingDistance = 13f;
        public float delay = 1f;

        private AssetBundle _assetBundle;
        private List<GameObject> _assetPool = new List<GameObject>();
        private GameObject _assetPrefab;
        private bool _areAssetsLoaded = false;
        private bool _isLoadingOrUnloading = false;
        #endregion

        private void Start()
        {
            foreach (AssetBundle loadedBundle in AssetBundle.GetAllLoadedAssetBundles())
            {
                if (loadedBundle.name == assetBundleName)
                {
                    Debug.Log("Unloading previously loaded AssetBundle: " + assetBundleName);
                    loadedBundle.Unload(false);
                    break;
                }
            }
        }

        private void Update()
        {
            float _distanceToPlayer = Vector3.Distance(mainAssetSpawnPoint.position, player.position);

            if (!_isLoadingOrUnloading)
            {
                if (!_areAssetsLoaded && _distanceToPlayer <= loadingDistance)
                {
                    StartCoroutine(LoadAllAssets());
                }
                else if (_areAssetsLoaded && _distanceToPlayer >= unloadingDistance)
                {
                    StartCoroutine(UnloadAllAssets());
                }
            }
        }

        #region Private Functions.
        /// <summary>
        /// Loads all wall prefabs from the assetbundle and creates them at the set spawn points.
        /// </summary>
        private IEnumerator LoadAllAssets()
        {
            _isLoadingOrUnloading = true;

            if (_assetBundle == null)
            {
                foreach (AssetBundle loadedBundle in AssetBundle.GetAllLoadedAssetBundles())
                {
                    if (loadedBundle.name == assetBundleName)
                    {
                        _assetBundle = loadedBundle;
                        break;
                    }
                }

                if (_assetBundle == null)
                {
                    string _bundlePath = Path.Combine(Application.streamingAssetsPath, folderName, assetBundleName);

                    if (File.Exists(_bundlePath))
                    {
                        AssetBundleCreateRequest bundleRequest = AssetBundle.LoadFromFileAsync(_bundlePath);
                        yield return bundleRequest;

                        _assetBundle = bundleRequest.assetBundle;

                        if (_assetBundle == null)
                        {
                            Debug.Log("Failed to load AssetBundle!");
                            _isLoadingOrUnloading = false;
                            yield break;
                        }
                    }
                    else
                    {
                        Debug.Log("AssetBundle not found at: " + _bundlePath);
                        _isLoadingOrUnloading = false;
                        yield break;
                    }
                }
            }

            if (_assetPrefab == null)
            {
                AssetBundleRequest _prefabRequest = _assetBundle.LoadAssetAsync<GameObject>(obstacleName);
                yield return _prefabRequest;

                _assetPrefab = _prefabRequest.asset as GameObject;
                if (_assetPrefab == null)
                {
                    Debug.Log("Failed to load _wall prefab: " + obstacleName);
                    _isLoadingOrUnloading = false;
                    yield break;
                }
            }

            for (int i = 0; i < assetSpawnPoints.Length; i++)
            {
                GameObject _wall = GetWallFromPool(assetSpawnPoints[i].position);
                _wall.SetActive(true);
            }

            _areAssetsLoaded = true;
            Debug.Log("All walls loaded and activated.");

            yield return new WaitForSeconds(delay);
            _isLoadingOrUnloading = false;
        }

        /// <summary>
        /// Unloads all wall prefabs by deactivating them and returning them to the pool.
        /// </summary>
        private IEnumerator UnloadAllAssets()
        {
            _isLoadingOrUnloading = true;

            for (int i = 0; i < _assetPool.Count; i++)
            {
                _assetPool[i].SetActive(false);
            }

            _areAssetsLoaded = false;

            if (_assetBundle != null)
            {
                _assetBundle.Unload(false);
                Debug.Log("AssetBundle unloaded after all walls were unloaded.");
                _assetBundle = null;
            }

            Debug.Log("All walls unloaded and returned to the pool.");

            yield return new WaitForSeconds(delay);
            _isLoadingOrUnloading = false;
        }

        /// <summary>
        /// Gets a column wall from the wall list if available, or creates a new one if none are available.
        /// </summary>
        private GameObject GetWallFromPool(Vector3 position)
        {
            foreach (GameObject wall in _assetPool)
            {
                if (!wall.activeInHierarchy)
                {
                    wall.transform.position = position;
                    return wall;
                }
            }

            GameObject _newWall = Instantiate(_assetPrefab, position, Quaternion.identity);
            _assetPool.Add(_newWall);
            return _newWall;
        }
        #endregion
    }
}
