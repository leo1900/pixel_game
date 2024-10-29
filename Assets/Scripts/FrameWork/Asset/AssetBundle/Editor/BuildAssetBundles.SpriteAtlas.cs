using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

namespace FrameWork.Asset
{
    public static partial class BuildAssetBundles
    {
        [MenuItem("AssetBundle/SpriteAtlas/生成AtlasConfig")]
        public static void CreateAtlasConfig()
        {

            AtlasConfigController asset = ScriptableObject.CreateInstance<AtlasConfigController>();
            asset.ParseAtlasPath("/Export/SpriteAtlas");
            asset.ParseAtlasPath("/Export/Cooking/SpriteAtlas");

            AssetDatabase.CreateAsset(asset, "Assets/Resources/Settings/AtlasConfigController.asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }

    }
}
