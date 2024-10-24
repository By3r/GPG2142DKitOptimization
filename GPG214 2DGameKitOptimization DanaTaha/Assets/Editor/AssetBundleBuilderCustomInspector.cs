using UnityEditor; 
using UnityEngine;
using System.IO;

public class AssetBundleBuilderCustomInspector
{
    /// <summary>
    /// You can access this by going to Assets, and scroll down till you read Build AssetBundles.
    /// </summary>
    [MenuItem("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        string _assetBundleFolder = "Assets/StreamingAssets/RangedAttacksBundle";

        if (!Directory.Exists(_assetBundleFolder))
        {
          Directory.CreateDirectory(_assetBundleFolder);
        }

        BuildPipeline.BuildAssetBundles(_assetBundleFolder, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
    }
}
