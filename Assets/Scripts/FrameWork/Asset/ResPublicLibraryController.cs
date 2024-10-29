using System.Collections.Generic;
using UnityEngine;
using System;

namespace FrameWork.Asset
{
    [Serializable]
    public class ResPublicLibraryCommit
    {
        public string CustomName;
        public string Res_Server_URL_Release;
        public string Res_Server_URL_Beta;
        public string Commit;

        public string GetVersionFileUrl(bool forceBeta)
        {
            string resUrl = (forceBeta || ConfigurationController.Instance.version != VersionStatus.RELEASE) ? Res_Server_URL_Beta : Res_Server_URL_Release;
            return $"{resUrl}/{FilePathTools.targetName}/{Commit}/Version.10000.txt";
        }

        public string GetResServerURL(bool forceBeta)
        {
            string resUrl = (forceBeta || ConfigurationController.Instance.version != VersionStatus.RELEASE) ? Res_Server_URL_Beta : Res_Server_URL_Release;
            return $"{resUrl}/{FilePathTools.targetName}/{Commit}/v10000_0_1";
        }
    }
    
    public class ResPublicLibraryController : ScriptableObject
    {
        public static string AssetConfigPath = "Settings/ResPublicLibraryController";
        private static ResPublicLibraryController _instance = null;

        public static ResPublicLibraryController Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<ResPublicLibraryController>(AssetConfigPath);
                }
                return _instance;
            }
        }
        
        [Space(10)]
        [Header("安卓 服务器地址")]
        public List<ResPublicLibraryCommit> Commit_Android;
        
        [Space(10)]
        [Header("iOS 服务器地址")]
        public List<ResPublicLibraryCommit> Commit_iOS;
        
        [Space(10)]
        [Header("Mac 服务器地址")]
        public List<ResPublicLibraryCommit> Commit_Mac;
        
        [Space(10)]
        [Header("Win 服务器地址")]
        public List<ResPublicLibraryCommit> Commit_Win;
    }
}