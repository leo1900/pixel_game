using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.Diagnostics;
using System;
using BestHTTP;
using System.Net;

namespace FrameWork.Asset
{
    public static partial class BuildAssetBundles
    {
        public static void CommonResLog(string format)
        {
            DebugUtil.Log(format);
            //Console.WriteLine(format);
        }

        public static void CommonResLog(string format, object arg1)
        {
            string str = string.Format(format, arg1);
            CommonResLog(str);
        }

        public static void CommonResLog(string format, object arg1, object arg2)
        {
            string str = string.Format(format, arg1, arg2);
            CommonResLog(str);
        }

        [MenuItem("AssetBundle/清除本地所有ab资源")]
        public static void ClearAB()
        {
#if UNITY_EDITOR
            string abOutPath = Application.dataPath + "/AssetBundleOut";
            if (Directory.Exists(abOutPath))
            {
                foreach (string dir in Directory.GetDirectories(abOutPath))
                {
                    Directory.Delete(dir, true);
                }

                foreach (string file in Directory.GetFiles(abOutPath))
                {
                    File.Delete(file);
                }
            }

            string saPath = Application.streamingAssetsPath;
            if (Directory.Exists(saPath))
            {
                foreach (string dir in Directory.GetDirectories(saPath))
                {
                    Directory.Delete(dir, true);
                }

                foreach (string file in Directory.GetFiles(saPath))
                {
                    File.Delete(file);
                }
            }

            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Info", "成功清除AssetBundleOut与StreamingAssets，请重新打包", "ok");
#else
            EditorUtility.DisplayDialog("Info", "只支持editor", "ok");
#endif
        }
        
        [MenuItem("AssetBundle/清除本地ab备份资源")]
        public static void ClearABBack()
        {
#if UNITY_EDITOR
            string abBackPath = Directory.GetParent(Application.dataPath).FullName + "/AssetBundlePatch";
            if (Directory.Exists(abBackPath))
            {
                foreach (string dir in Directory.GetDirectories(abBackPath))
                {
                    Directory.Delete(dir, true);
                }

                foreach (string file in Directory.GetFiles(abBackPath))
                {
                    File.Delete(file);
                }
            }

            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Info", "成功清除AssetBundlePatch", "ok");
#else
            EditorUtility.DisplayDialog("Info", "只支持editor", "ok");
#endif
        }

        [MenuItem("AssetBundle/清除本地download下资源")]
        public static void ClearDowload()
        {
#if UNITY_EDITOR
            string downloadPath = FilePathTools.downLoadPath;
            if (Directory.Exists(downloadPath))
            {
                foreach (string dir in Directory.GetDirectories(downloadPath))
                {
                    Directory.Delete(dir, true);
                }

                foreach (string file in Directory.GetFiles(downloadPath))
                {
                    File.Delete(file);
                }

                AssetDatabase.Refresh();
                EditorUtility.DisplayDialog("Info", "成功清除download下资源", "ok");
            }
            else
            {
                EditorUtility.DisplayDialog("Info", "文件夹不存在", "ok");
            }
#else
            EditorUtility.DisplayDialog("Info", "只支持editor", "ok");
#endif
        }

        [MenuItem("AssetBundle/打开本地download文件夹")]
        public static void OpenDowload()
        {
#if UNITY_EDITOR
            string downloadPath = FilePathTools.downLoadPath;
            if (Directory.Exists(downloadPath))
            {
                Process.Start(downloadPath);
            }
            else
            {
                EditorUtility.DisplayDialog("Info", "文件夹不存在", "ok");
            }
#else
            EditorUtility.DisplayDialog("Info", "只支持editor", "ok");
#endif
        }
    }
}