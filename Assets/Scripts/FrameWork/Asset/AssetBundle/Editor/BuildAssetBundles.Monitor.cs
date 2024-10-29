/*-------------------------------------------------------------------------------------------
// Copyright (C) 2019 成都，天龙互娱
//
// 模块名：BuildAssetBundles.Local
// 创建日期：2021-3-29
// 创建者：qibo.li
// 模块描述：AssetBundle 数据监控
//-------------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace FrameWork.Asset
{
    [System.Serializable]
    public class ABMonitor
    {
        public ABMonitorHead head = new ABMonitorHead();
        public List<ABMonitorBody> body = new List<ABMonitorBody>();
    }

    [System.Serializable]
    public class ABMonitorHead
    {
        public string app;
        public string bundle;
        public string version;
        public string date;
        public string unity;
        public string git;
        public string platform;
        public bool coverCareDate;
        
        public ABMonitorHead()
        {
            app = PlayerSettings.productName.Replace(" ","");
            bundle = EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS ? 
                PlayerSettings.iOS.buildNumber : PlayerSettings.Android.bundleVersionCode.ToString();
            version = EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS
                ? AssetConfigController.Instance.IOSRootVersion : AssetConfigController.Instance.RootVersion;
            date = DateTime.Now.ToString("yyyy-MM-dd");
            unity = Application.unityVersion;
            git = BuildAssetBundles.GetCommitID().Substring(0, 8);
            platform = FilePathTools.targetName;
            coverCareDate = false;
        }
    }

    [System.Serializable]
    public class ABMonitorBody
    {
        public string path;
        public string hash;
        public long size;

        public ABMonitorBody(string _path, string _hash, long _size)
        {
            path = _path;
            hash = _hash;
            size = _size;
        }
    }
    
    public partial class BuildAssetBundles
    {
        /// <summary>
        /// 打包监控数据
        /// </summary>
        static void PackMonitorData()
        {
            bool coverCareDate = false;
            string[] cmdArguments = Environment.GetCommandLineArgs();
            for (int count = 0; count < cmdArguments.Length; count++)
            {
                if (cmdArguments[count] == "-coverCareDate")
                {
                    coverCareDate = cmdArguments[count + 1].Equals("true");
                }
            }
            
            ABMonitor monitor = new ABMonitor();
            monitor.head.coverCareDate = coverCareDate;
            
            List<string> paths = new List<string>();
            foreach (var group in AssetConfigController.Instance.Groups)
            {
                foreach (var p in group.Paths)
                {
                    if (!p.FromOutside)
                    {
                        paths.Add(p.Path);                     
                    }
                }
            }
            foreach (var p in AssetConfigController.Instance.ActivityResPaths)
            {
                paths.Add($"Activity/{p.Replace('/', '_')}");
            }

            foreach (var p in paths)
            {
                string path = $"{p.ToLower()}.ab";
                string localPath = $"{FilePathTools.assetbundlePatchPath}/{path}";
                if (File.Exists(localPath) && File.Exists($"{localPath}.patch"))
                {
                    JObject jobj = JObject.Parse(File.ReadAllText($"{localPath}.patch"));
                    string hash = jobj["hash"].ToString();
                    long size = new FileInfo(localPath).Length / 1024;
                            
                    monitor.body.Add(new ABMonitorBody(path, hash, size));
                }
                else
                {
                    DebugUtil.LogError($"can not find {localPath} or *.patch.");
                }       
            }
            
            if(Directory.Exists(FilePathTools.assetbundleMonitorPath))
                Directory.Delete(FilePathTools.assetbundleMonitorPath, true);
            if (!Directory.Exists(FilePathTools.assetbundleMonitorPath))
                Directory.CreateDirectory(FilePathTools.assetbundleMonitorPath);
            
            string str = JsonConvert.SerializeObject(monitor);
            CreateFile($"{FilePathTools.assetbundleMonitorPath}/", $"MonitorVersion.json", str);

            string patchDirectory = $"{FilePathTools.assetbundleMonitorPath}/Patch";
            foreach (var p in paths)
            {
                string path = $"{p.ToLower()}.ab";
                string localPatchPath = $"{FilePathTools.assetbundlePatchPath}/{path}.patch";
                if (File.Exists(localPatchPath))
                {
                    string destPath = $"{patchDirectory}/{path}.patch";
                    string destDirectory = destPath.Substring(0, destPath.LastIndexOf("/"));
                    if (!Directory.Exists(destDirectory))
                        Directory.CreateDirectory(destDirectory);
                    File.Copy(localPatchPath, destPath);
                }
            }
            
            DebugUtil.Log("PackMonitorData Finish.");
        }
    }
}