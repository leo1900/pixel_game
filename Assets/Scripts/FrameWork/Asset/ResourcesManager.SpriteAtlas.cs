using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.U2D;

namespace FrameWork.Asset
{
    public partial class ResourcesManager
    {
        // 是否使用低清图集
        private bool useSdAtlas = false;
        //atlas预加载及缓存
        Dictionary<string, SpriteAtlas> loadedAtlasDict = new Dictionary<string, SpriteAtlas>();
        //图集加载目录前缀，根目录为Export，用于加载图集时补全路径
        private List<string> atlasPrefixList = new List<string>() { "SpriteAtlas/" };

        // 添加图集 prefix
        public void AddAtlasPrefix(string prefix)
        {
            atlasPrefixList.Add(prefix);
        }

        // 设置是否使用低清图集
        public void UseSDAtlas(bool useSd)
        {
            useSdAtlas = useSd;
        }

        public bool IsSdVariant
        {
            get { return useSdAtlas; }
        }

        #region spriteatlas预加载和缓存

        //[Obsolete("LoadSpriteAtlas is Obsolete,Use LoadSpriteAtlasVariant")]
        public SpriteAtlas LoadSpriteAtlas(string name)
        {
#if !DISABLE_SD_ATLAS
            if (useSdAtlas)
            {
                name += ".SD";
            }
#endif

            SpriteAtlas atlas = null;

            foreach (string prefix in atlasPrefixList)
            {
                var path = Path.Combine(prefix, name);
                if (loadedAtlasDict.TryGetValue(path, out atlas))
                {
                    return atlas;
                }
                else
                {
                    atlas = LoadResource<SpriteAtlas>(path);
                    if (atlas != null)
                    {
                        loadedAtlasDict.Add(path, atlas);
                        break;
                    }
                }
            }
            if (atlas == null)
            {
                DebugUtil.LogError("qushuang =====> 没有从现有的 atlas list 里找到图集:" + name);
            }
            return atlas;
        }

        public void LoadSpriteAtlasAsync(string name, Action<SpriteAtlas> OnFinished = null)
        {
#if !DISABLE_SD_ATLAS
            if (useSdAtlas)
            {
                name += ".SD";
            }
#endif

            SpriteAtlas atlas = null;

            int failCount = 0;
            foreach (string prefix in atlasPrefixList)
            {
                var path = Path.Combine(prefix, name);
                if (loadedAtlasDict.TryGetValue(path, out atlas))
                {
                    OnFinished?.Invoke(atlas);
                    break;
                }
                else
                {
                    LoadResourceAsync<SpriteAtlas>(path, (newAtlas) =>
                    {
                        if (newAtlas != null)
                        {
                            loadedAtlasDict[path] = newAtlas;
                            OnFinished?.Invoke(newAtlas);
                        }
                        else
                        {
                            failCount++;
                            if (failCount == atlasPrefixList.Count)
                            {
                                OnFinished?.Invoke(null);
                            }
                        }
                    });
                }
            }
        }

        public void UnloadSpriteAtlas(string name)
        {
#if !DISABLE_SD_ATLAS
            if (useSdAtlas)
            {
                name += ".SD";
            }
#endif

            foreach (string prefix in atlasPrefixList)
            {
                var path = Path.Combine(prefix, name);
                loadedAtlasDict.Remove(path);
            }
        }

        public void UnloadSpriteAtlasByName(string atlas_name)
        {
#if !DISABLE_SD_ATLAS
            if (useSdAtlas)
            {
                atlas_name += ".SD";
            }
#endif
            var path = GetExistingSpriteAtlasPath(atlas_name);
            if (string.IsNullOrEmpty(path))
            {
                DebugUtil.LogWarning($"yangrui =====> 没有从现有的 AtlasConfigController 里找到图集: {atlas_name}");
                return;
            }
            loadedAtlasDict.Remove(path);
        }


        public void UnloadSpriteAtlasImmediate(string name)
        {
#if !DISABLE_SD_ATLAS
            if (useSdAtlas)
            {
                name += ".SD";
            }
#endif

            foreach (string prefix in atlasPrefixList)
            {
                var path = Path.Combine(prefix, name);
                if (loadedAtlasDict.TryGetValue(path, out SpriteAtlas atlas))
                {
                    loadedAtlasDict.Remove(path);
                    ReleaseAtlas(path.ToLower(), true);
                }
            }
        }

        /// <summary>
        /// 获得图集中的某个精灵，用于Image换图
        /// </summary>
        /// <returns>The sprite.</returns>
        /// <param name="atlasPath">图集相对Assets/Export/SpriteAtlas的路径</param>
        /// <param name="spriteName">sprite name</param>
        //[Obsolete("GetSprite is Obsolete,Use GetSpriteVariant")]
        public Sprite GetSprite(string atlasPath, string spriteName)
        {
            SpriteAtlas sa = LoadSpriteAtlas(atlasPath);
            if (sa == null)
            {
                DebugUtil.LogError("sprite load error:" + atlasPath + ":" + spriteName);
                return null;
            }

            return sa.GetSprite(spriteName);
        }

        #endregion

        #region 新版spriteatlas加载和缓存
        /// <summary>
        /// 
        /// </summary>
        /// <param name="atlasName"> SpriteAtalasManager 请求时传入的图集名 </param>
        /// <param name="forcePath"> 强制使用这个路径加载图集，一般用在多层文件夹结构(ck3在用) </param>
        /// <returns></returns>
        public SpriteAtlas LoadSpriteAtlasVariant(string atlasName, bool forceBundle = false)
        {
            SpriteAtlas atlas = null;
            string _atlasPath = GetExistingSpriteAtlasPath(atlasName);
            if (string.IsNullOrEmpty(_atlasPath))
            {
                DebugUtil.LogError($"yangrui =====> 没有从现有的 AtlasConfigController 里找到图集: {atlasName}");
                return atlas;
            }
            if (loadedAtlasDict.TryGetValue(_atlasPath, out atlas))
            {
                return atlas;
            }
            else
            {
                atlas = LoadResource<SpriteAtlas>(_atlasPath, forceBundle);
                if (atlas != null)
                {
                    loadedAtlasDict.Add(_atlasPath, atlas);
                }
                else
                {
                    DebugUtil.LogError("qushuang =====> 没有从现有的 atlas list 里找到图集:" + _atlasPath);
                }

                return atlas;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="atlasName">SpriteAtalasManager 请求时传入的图集名</param>
        /// <param name="spriteName"></param>
        /// <returns></returns>
        public Sprite GetSpriteVariant(string atlasName, string spriteName, bool forceBundle = false, bool ignoreErrorLog = false)
        {
            try
            {
                SpriteAtlas sa = LoadSpriteAtlasVariant(atlasName, forceBundle);
                if (sa == null)
                {
                    if (!ignoreErrorLog)
                    {
                        DebugUtil.LogError("sprite load error:" + atlasName + ":" + spriteName);
                    }
                    return null;
                }

                var sprite = sa.GetSprite(spriteName);
                if (sprite == null)
                {
                    if (!ignoreErrorLog)
                    {
                        DebugUtil.LogError($"sprite load error, atlas : {atlasName}, sprite name : {spriteName}");
                    }
                }

                return sprite;
            }
            catch (Exception e)
            {
                DebugUtil.LogError(e);
            }

            return null;
        }


        /// <summary>
        /// 获得可以使用的图集路径
        /// 高清图集如果不存在，就自动加载低清的
        /// 保底是低清必须存在
        /// </summary>
        /// <returns></returns>
        private string GetExistingSpriteAtlasPath(string atlasName)
        {
            AtlasPathNode atlasPathNode = AtlasConfigController.Instance.GetAtlasPath(atlasName);
            if (atlasPathNode == null)
            {
                DebugUtil.LogError("SpriteAtlas Path Error:" + atlasName);
                return null;
            }

            if (!AssetConfigController.Instance.UseAssetBundle)// 不使用ab的情况下，恒定使用hd图集
                return atlasPathNode.HdPath;


            if (!IsSdVariant)// 要求使用高清
            {
                string abPath = string.Format("{0}.ab", atlasPathNode.HdPath.Substring(0, atlasPathNode.HdPath.Length - atlasName.Length - 1).ToLower());
                if (FilePathTools.IsFileExists(abPath))// 高清图集存在
                    return atlasPathNode.HdPath;

            }

            return atlasPathNode.SdPath;// 返回低清图集
        }

        /// <summary>
        /// 获取正常图集的ab包路径
        /// </summary>
        /// <param name="atlasName"></param>
        /// <returns></returns>
        public string GetNormalSpriteAtlasPath(string atlasName)
        {
            AtlasPathNode atlasPathNode = AtlasConfigController.Instance.GetAtlasPath(atlasName);
            string abPath = string.Empty;
            if (IsSdVariant)
            {
                abPath = string.Format("{0}.ab", atlasPathNode.SdPath.Substring(0, atlasPathNode.SdPath.Length - atlasName.Length - 1).ToLower());

            }
            else
            {
                abPath = string.Format("{0}.ab", atlasPathNode.HdPath.Substring(0, atlasPathNode.HdPath.Length - atlasName.Length - 1).ToLower());
            }
            return abPath;
        }


        public void UnloadSpriteAtlasImmediateVariant(string atlasName)
        {
            var path = GetExistingSpriteAtlasPath(atlasName);
            if (string.IsNullOrEmpty(path))
            {
                DebugUtil.LogWarning($"yangrui =====> 没有从现有的 AtlasConfigController 里找到图集: {atlasName}");
                return;
            }
            if (loadedAtlasDict.TryGetValue(path, out var atlas))
            {
                loadedAtlasDict.Remove(path);
                ReleaseAtlas(path.ToLower(), true);
            }
        }
        
        public void UnloadSpriteAtlasImmediateExist(string atlasName)
        {
            var path = GetExistingSpriteAtlasPath(atlasName);
            if (string.IsNullOrEmpty(path))
            {
                DebugUtil.LogWarning($"yangrui =====> 没有从现有的 AtlasConfigController 里找到图集: {atlasName}");
                return;
            }
            if (loadedAtlasDict.TryGetValue(path, out var atlas))
            {
                loadedAtlasDict.Remove(path);
                // path = $"{path.TrimEnd(".ab".ToCharArray())}/{atlasName}";
                path = path.TrimEnd(".ab".ToCharArray());
                ReleaseAtlas(path.ToLower(), true);
            }
        }

        public List<string> GetAllCachedAtlasName()
        {
            var result = new List<string>();
            foreach (var key in loadedAtlasDict.Keys)
            {
                var arr = key.Split(Path.AltDirectorySeparatorChar);
                result.Add(arr[arr.Length-1]);
            }
            return result;
        }

        #endregion
    }
}