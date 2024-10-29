
using System.IO;
using UnityEditor;
using UnityEngine;

namespace FrameWork.Asset
{
    public class AssetBundleSet : ScriptableObject
    {
        private static AssetBundleSet _instance = null;
        public static AssetBundleSet Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = AssetDatabase.LoadAssetAtPath<AssetBundleSet>("Assets/Editor/AssetBundleSet.asset");
                }
                return _instance;
            }
        }

        [Space(10)]
        [Header(" --------------------- 是否排序依赖 ----------------------")]
        public bool SortDeps;
        
        [MenuItem("AssetBundle/Patch/创建打包配置")]
        public static void CreateAssetBundleSet()
        {
            AssetBundleSet asset = CreateInstance<AssetBundleSet>();

            if (!Directory.Exists("Assets/Editor"))
                Directory.CreateDirectory("Assets/Editor");
            
            AssetDatabase.CreateAsset(asset, "Assets/Editor/AssetBundleSet.asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }
    }
}