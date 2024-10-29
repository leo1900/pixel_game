using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

namespace FrameWork.Asset
{
    public class AssetConfigPrototype : ScriptableObject
    {
        public static string AssetConfigPath = "Settings/AssetConfigPrototype";
        private static AssetConfigPrototype _instance = null;

        public static AssetConfigPrototype Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<AssetConfigPrototype>(AssetConfigPath);
                }
                return _instance;
            }
        }

        [Space(10)]
        [Header("[填入相对Assets/Export的相对路径，大小写敏感，只支持文件夹]")]
        [Header(" ---------------------- AB包分组 -----------------------")]
        public BundleGroup[] Groups;

        public void Modify(AssetConfigController assetConfigController, bool includeOutside = false)
        {
            HashSet<string> activityResSet = new HashSet<string>(assetConfigController.ActivityResPaths);

            string root = "Assets/Export/";
            for (int i = 0; i < Groups.Length; i++)
            {
                BundleGroup oldGroup = assetConfigController.Groups[i];
                BundleGroup group = Groups[i];
                var newGroup = new BundleGroup();
                newGroup.GroupName = group.GroupName;
                newGroup.Version = group.Version;
                newGroup.GroupIndex = group.GroupIndex;
                newGroup.Paths = new List<BundleState>();
                newGroup.UpdateWholeAB = group.UpdateWholeAB;

                var dict = new Dictionary<string, BundleState>();
                var datas = new List<BundleState>(group.Paths);
                datas.Sort((a, b) => Convert.ToInt32(a.InInitialPacket) - Convert.ToInt32(b.InInitialPacket));

                if (!includeOutside)
                {
                    foreach (var data in oldGroup.Paths)
                    {
                        if (data.FromOutside)
                        {
                            dict[data.Path] = data;
                        }
                    }
                }

                foreach (var data in datas)
                {
                    if (data.FromOutside && !includeOutside)
                    {
                        continue;
                    }

                    string dir;
                    string[] subDirs;
                    if (data.Path.IndexOf("\\/") > 0)
                    {
                        dir = FilePathTools.GetDirectoryName(data.Path.Replace("\\/", ""));
                        subDirs = FilePathTools.GetDirectoriesEx(root + dir, "^" + root + data.Path + "$", SearchOption.AllDirectories);
                    }
                    else
                    {
                        dir = FilePathTools.GetDirectoryName(data.Path);
                        string name = FilePathTools.GetFileName(data.Path);
                        subDirs = FilePathTools.GetDirectories(root + dir, "^" + name + "$");
                    }

                    foreach (string subDir in subDirs)
                    {
                        string path = subDir.Replace(root, "");
                        if (activityResSet.Contains(path) || FilePathTools.GetFiles(subDir, "^(?!\\.)").Length == 0)
                        {
                            continue;
                        }

                        BundleState bundleState = new BundleState
                        {
                            Path = path,
                            InInitialPacket = data.InInitialPacket,
                            FromOutside = data.FromOutside,
                            NameOutside = data.NameOutside,
                            GroupOutside = data.GroupOutside,
                            PathOutside = data.PathOutside.Replace("{0}", subDir.Replace(root + (string.IsNullOrEmpty(dir) ? "" : dir + "/"), "").ToLower())
                        };
                        dict[bundleState.Path] = bundleState;
                    }
                }
                newGroup.Paths.AddRange(dict.Values);
                newGroup.Paths.Sort((a, b) => string.Compare(a.Path, b.Path));

                if (i < assetConfigController.Groups.Length)
                {
                    assetConfigController.Groups[i] = newGroup;
                }
            }
        }
    }
}