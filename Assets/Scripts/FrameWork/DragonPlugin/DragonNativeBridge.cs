using System;
using FrameWork;
using FrameWork.Asset;
using UnityEngine;
using Newtonsoft.Json.Linq;

namespace DragonU3DSDK
{
    public class DragonNativeBridge
    {

#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern bool isFacebookInstalledIOS();
#endif

#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern bool isAppInstalledIOS(string scheme);
#endif

#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern void openSetting();
#endif

#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern void openAppStoreRate(string rateUrl);
#endif

#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern bool isNotificationPermissionOpeniOS();
#endif

#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern void popupPrivacyIOS(string title, string message, string linkTitle, string linkUrl, string agreeTitle, string cancelTitle, string gameObjectName);
#endif

#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern void showRateUsViewControllerIOS(string url, string title, string message, string rateButtonText, string laterButtonText, string noButtonText, string gameObjectName);
#endif

#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern void saveDataToKeyChainIOS(string key, string data);

        [DllImport("__Internal")]
        private static extern string getDataFromKeyChainIOS(string key);
#endif

#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern string getVersionCodeIOS();
#endif

#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern int getScreenOrientationIOS();
#endif

#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern bool FBMessagerShareIOS(string linkUrl, string imageUrl, string pageId, string title, string subTitle, string buttonText);
#endif

#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern bool isFaceBookAccessTokenActiveIOS();
#endif

#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern bool isFacebookDataAccessExpiredIOS();
#endif

#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern bool isUserNotificationEnabled();
#endif

#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern bool needTrackingAuthorizationIOS();
#endif

#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern int ATTStatusIOS();
#endif

#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern void requestTrackingAuthorizationIOS(string gameObjectName);
#endif

#if UNITY_IOS
        private delegate void reauthorizeFacebookDataAccessIOSCallBack();

        [DllImport("__Internal")]
        private static extern void reauthorizeFacebookDataAccessIOS(reauthorizeFacebookDataAccessIOSCallBack callback);
#endif

#if UNITY_IOS
        [DllImport("__Internal")]
        public static extern void copy(string text);

        [DllImport("__Internal")]
        public static extern string paste();
#endif

#if UNITY_IOS
        [DllImport("__Internal")]
        public static extern bool iOSSAvailableFourteen();
        
        [DllImport("__Internal")]
        private static extern bool iOSSAvailableFourteenFive();
        
        [DllImport("__Internal")] 
        private static extern void FBAdSettingsBridgeSetAdvertiserTrackingEnabled(bool advertiserTrackingEnabled);

        [DllImport("__Internal")]
        public static extern bool requestAndLoadInterstitialAd();
        
        [DllImport("__Internal")]
        private static extern bool isInterstitialAdReady();

        [DllImport("__Internal")]
        private static extern bool tryShowInterstitialAd();
        
        [DllImport("__Internal")]
        public static extern bool requestAndLoadRVAd();
        
        [DllImport("__Internal")]
        private static extern bool isRVAdReady();

        [DllImport("__Internal")]
        private static extern bool tryShowRVAd();
        [DllImport("__Internal")]
        private static extern bool init(string a1, string a2);

#endif

#if UNITY_ANDROID && !UNITY_EDITOR
        private static AndroidJavaClass aj = null;
        private static string AndroidVersionName;
        private static int AndroidVersionCode;
        private static string AndroidDeviceType;
#endif

        //静态构造函数保证使用前aj inited
        static DragonNativeBridge()
        {
            TextAsset textAsset = Resources.Load<TextAsset>("AppLovinSetting");
            string inter_placement = string.Empty;
            string rv_placement = string.Empty;
            if (textAsset != null)
            {
                string content = EncryptDecrypt.Decrypt(textAsset.text);
                JArray json = JArray.Parse(content);
                if (json != null)
                {
                    foreach (var item in json)
                    {
                        string package = item["package"].ToString();
                        string inter_ios = item["inter-i"].ToString();
                        string rv_ios = item["rv-i"].ToString();
                        string inter_android = item["inter-a"].ToString();
                        string rv_android = item["rv-a"].ToString();
                        if (Application.identifier.Equals(package))
                        {
#if UNITY_IOS
                            inter_placement = inter_ios;
                            rv_placement = rv_ios;
#endif
#if UNITY_ANDROID
                            inter_placement = inter_android;
                            rv_placement = rv_android;
#endif
                            break;
                        }
                    }
                }
            }

#if UNITY_ANDROID && !UNITY_EDITOR
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            if (activity == null)
            {
                return;
            }
            AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");
            aj = new AndroidJavaClass("com.dragonplus.DragonNativeBridge");
            aj.CallStatic("initialize", context, activity,inter_placement,rv_placement);

            AndroidVersionName = aj.CallStatic<string>("getVersionName");
            AndroidVersionCode = aj.CallStatic<int>("getVersionCode");
            AndroidDeviceType = aj.CallStatic<string>("getDeviceType");
#endif

#if UNITY_IOS && !UNITY_EDITOR
            init(inter_placement,rv_placement);
#endif
        }
        
#if UNITY_ANDROID && !UNITY_EDITOR
        public static AndroidJavaClass unityActivityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        public static AndroidJavaObject activityObj = unityActivityClass.GetStatic<AndroidJavaObject>("currentActivity");

#endif
        public static bool IsAppInstalled(string packageName)
        {
#if UNITY_EDITOR
            return false;
#elif UNITY_IOS
            return isAppInstalledIOS(packageName);
#elif UNITY_ANDROID

            return aj.CallStatic<bool>("isAppInstalled", packageName);
#else
            return false;
#endif
        }

        public static bool IsFacebookInstalled()
        {
#if UNITY_EDITOR
            return false;
#elif UNITY_IOS
            return isFacebookInstalledIOS();
#elif UNITY_ANDROID

            return aj.CallStatic<bool>("isFacebookInstalled");
#else
            return false;
#endif
        }

        public static int GetAndroidSDKVersion()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            using (var version = new AndroidJavaClass("android.os.Build$VERSION"))
            {
                return version.GetStatic<int>("SDK_INT");
            }
#else
            return -1;
#endif
        }

        public static void OpenSettingIOS()
        {
#if UNITY_IOS && !UNITY_EDITOR
            openSetting();
#endif
        }

        public static void OpenNotifycationSetting()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
          aj.CallStatic("openNotificationSetting");
#endif
#if UNITY_IOS && !UNITY_EDITOR
          openSetting();
#endif   
        }

        public static void OpenAppStoreRate(string rateUrl)
        {
#if UNITY_IOS && !UNITY_EDITOR
            openAppStoreRate(rateUrl);
#endif
        }

        public static void SaveDataToKeyChain(String key, String data)
        {
#if UNITY_IOS && !UNITY_EDITOR
            saveDataToKeyChainIOS(key, data);
#endif
        }

        public static bool IsNotificationPermissionOpen()
        {
#if UNITY_IOS && !UNITY_EDITOR
            return isNotificationPermissionOpeniOS();
#else
            return true;
#endif
        }

        public static String GetDataFromKeyChain(String key)
        {
#if UNITY_IOS && !UNITY_EDITOR
            return getDataFromKeyChainIOS(key);
#endif
            return "";
        }

        public static void ShowRateUsViewController(String url, String title, String message, String rateButtonText, String laterButtonText, String noButtonText, String gameObjectName)
        {
#if UNITY_IOS && !UNITY_EDITOR
            showRateUsViewControllerIOS(url, title, message, rateButtonText, laterButtonText, noButtonText, gameObjectName);
#endif
        }

        public static void ShowPrivacy(String title, String message, String linkTitle, String linkUrl, String agreeTitle, String cancelTitle, String gameObjectName)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            aj.CallStatic("popupPrivacy", title,message,linkTitle,linkUrl,agreeTitle,cancelTitle, gameObjectName);
#elif UNITY_IOS && !UNITY_EDITOR
            popupPrivacyIOS(title,message,linkTitle,linkUrl,agreeTitle,cancelTitle, gameObjectName);
#endif
        }

        public static void OpenFacebookAndroid(string facebookUrl, string pageId)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            aj.CallStatic("openFacebookPage",facebookUrl,pageId);
#endif
        }

        public static void OpenTwitterAndroid(string name, string pageId)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            aj.CallStatic("openTwitterPage",name,pageId);
#endif
        }

        public static void OpenInstagramAndroid(string name, string pageId)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            aj.CallStatic("openInstagramPage",name,pageId);
#endif
        }

        public static string GetVersionName()
        {
            String versionCode = "UNKNOWN";
#if UNITY_ANDROID && !UNITY_EDITOR
            versionCode = AndroidVersionName;
#else
            versionCode = Application.version;
#endif
            return versionCode;
        }

        public static int GetVersionCode()
        {
            int versionCode = -1;
#if UNITY_ANDROID && !UNITY_EDITOR
            versionCode = AndroidVersionCode;
#elif UNITY_IOS && !UNITY_EDITOR
            var strVersionCode = getVersionCodeIOS();
            int.TryParse(strVersionCode, out versionCode);
#endif
            return versionCode;
        }

        public static String getDeivceId()
        {
            //return "2CFBA05AAB88DEC9D99F54DDFA435040";
            string deviceId = "UNKNOWN";
#if UNITY_ANDROID && !UNITY_EDITOR
            deviceId = aj.CallStatic<string>("getDeviceId");
#endif
            return deviceId;
        }

        public static String getIMEI()
        {
            string deviceId = "UNKNOWN";
#if UNITY_ANDROID && !UNITY_EDITOR
            deviceId = aj.CallStatic<string>("getIMEI");
#endif
            return deviceId;
        }

        public static String getAndroidID()
        {
            string deviceId = "UNKNOWN";
#if UNITY_ANDROID && !UNITY_EDITOR
            deviceId = aj.CallStatic<string>("getAndroidID");
#endif
            return deviceId;
        }

        public static String getMacAddress()
        {
            string deviceId = "UNKNOWN";
#if UNITY_ANDROID && !UNITY_EDITOR
            deviceId = aj.CallStatic<string>("getMacAddress");
#endif
            return deviceId;
        }

//        public static String getPurchases()
//        {
//            string deviceId = "UNKNOWN";
//#if UNITY_ANDROID && !UNITY_EDITOR
//            deviceId = aj.CallStatic<string>("getPurchased");
//#endif
//            return deviceId;
//        }

        public static String getDeviceType()
        {
#if UNITY_EDITOR
            return "UnityEditor";
#elif UNITY_ANDROID
            return AndroidDeviceType;
#elif UNITY_IOS
            if (SystemInfo.deviceModel.Contains("iPad"))
            {
                return "tablet";
            }
            else
            {
                return "phone";
            }
#else
            return "unkown";
#endif
        }

        public static ScreenOrientation GetScreenOrientation()
        {
#if UNITY_IOS && !UNITY_EDITOR
            int screenOrientation = getScreenOrientationIOS();
            // DebugUtil.Log("iOS Screen orientation : " + screenOrientation);
            switch (screenOrientation)
            {
                case 1:
                    return ScreenOrientation.Portrait;
                case 2:
                    return ScreenOrientation.PortraitUpsideDown;
                case 3:
                    return ScreenOrientation.LandscapeLeft;
                case 4:
                    return ScreenOrientation.LandscapeRight;
                default:
                    return ScreenOrientation.Unknown;
            }
#else
            return Screen.orientation;
#endif

        }

        public static int LoadSound(string soundName)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return aj.CallStatic<int>("loadSound",soundName);
#else
            return -1;
#endif
        }

   

        public static void StopSound(int soundId)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            aj.CallStatic("stopSound",soundId);
#else

#endif
        }

        public static void PauseAllSound()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            aj.CallStatic("pauseAllSound");
#else

#endif
        }

        public static void ResumeAllSound()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            aj.CallStatic("resumeAllSound");
#else

#endif
        }

        /// <summary>
        /// 销毁音效池，只可在退出时使用
        /// </summary>
        public static void ReleaseSoundPool()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            aj.CallStatic("releaseSoundPool");
#else

#endif
        }

        /// <summary>
        /// Inits the sound.
        /// </summary>
        /// <param name="path">相对于SteamingAssets目录的相对路径，如Export/Audios/Sound</param>
        public static void InitSound(string path)
        {
            DebugUtil.Log("JJJJ initing Sound");
#if UNITY_ANDROID && !UNITY_EDITOR
            aj.CallStatic("initSoundPool",path);
#else

#endif
        }

        public static void TestSound()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            aj.CallStatic("testSound");
#else

#endif
        }

        public static void RequestInAppUpdate()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            aj.CallStatic("RequestAppUpdate");
#endif
        }

        // 注意参数不能传空字符串("")
        public static bool FBMessagerShare(String linkUrl, String imageUrl, String pageId, String title = "title", String subTitle = "subTitle", String buttonText = "buttonText")
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return aj.CallStatic<bool>("FBMessagerShare", linkUrl, imageUrl, pageId, title, subTitle, buttonText);
#elif UNITY_IOS && !UNITY_EDITOR
            return FBMessagerShareIOS(linkUrl, imageUrl, pageId, title, subTitle, buttonText);
#else
            return false;
#endif
        }

        /// <summary>
        /// 判断facebook accesstoken是否有效
        /// </summary>
        /// <returns></returns>
        public static bool isFaceBookAccessTokenActive()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return aj.CallStatic<bool>("isFaceBookAccessTokenActive");
#elif UNITY_IOS && !UNITY_EDITOR
            return isFaceBookAccessTokenActiveIOS();
#else
            return true;
#endif
        }

        /// <summary>
        /// 判断facebook data access是否过期
        /// </summary>
        /// <returns></returns>
        public static bool isFacebookDataAccessExpired()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return aj.CallStatic<bool>("isFacebookDataAccessExpired");
#elif UNITY_IOS && !UNITY_EDITOR
            return isFacebookDataAccessExpiredIOS();
#else
            return true;
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        public static void ReauthorizeFacebookDataAccess(Action<int> callback)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            aj.CallStatic("reauthorizeFacebookDataAccess");
            //
            // //android平台下，重新授权后需要刷新一次accesstoken
            // //第一次refresh保证eauthorize完成后才执行第二次refresh
            // FB.Mobile.RefreshCurrentAccessToken((result1) =>
            // {
            //     FB.Mobile.RefreshCurrentAccessToken((result2) =>
            //     {
            //         if (null != callback)
            //         {
            //             callback(isFacebookDataAccessExpired() ? 0 : 1);
            //         }
            //     });
            // });
#elif UNITY_IOS && !UNITY_EDITOR
            _RFDAIOSCallBack = callback;
            reauthorizeFacebookDataAccessIOS(ReauthorizeFacebookDataAccess_IOSCallBack);
#else
            return;
#endif
        }

#if UNITY_IOS && !UNITY_EDITOR
        static Action<int> _RFDAIOSCallBack = null;

        [MonoPInvokeCallback (typeof(reauthorizeFacebookDataAccessIOSCallBack))]
        static void ReauthorizeFacebookDataAccess_IOSCallBack()
        {
            if(_RFDAIOSCallBack != null)
            {
                _RFDAIOSCallBack(isFacebookDataAccessExpired() ? 0 : 1);
                _RFDAIOSCallBack = null;
            }
        }
#endif

        public static bool IsUserNotificationEnabled()
        {
#if UNITY_EDITOR
            return false;
#elif UNITY_IOS
            return isUserNotificationEnabled();
#elif UNITY_ANDROID
            return aj.CallStatic<bool>("isUserNotificationEnabled");
#else
    return false;
#endif
        }
        public static bool NeedTrackingAuthorization()
        {
#if UNITY_IOS && !UNITY_EDITOR
            return needTrackingAuthorizationIOS();
#endif
            return false;
        }
        
        //0-ATTrackingManagerAuthorizationStatusNotDetermined
        //1-ATTrackingManagerAuthorizationStatusRestricted
        //2-ATTrackingManagerAuthorizationStatusDenied
        //3-ATTrackingManagerAuthorizationStatusAuthorized
        public static int ATTStatus()
        {
#if UNITY_IOS && !UNITY_EDITOR
            return ATTStatusIOS();
#endif
            return -1;
        }
        
        public static void RequestTrackingAuthorization(String gameObjectName)
        {
#if UNITY_IOS && !UNITY_EDITOR
            requestTrackingAuthorizationIOS(gameObjectName);
#endif
        }

        public static void Copy(string text)
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            TextEditor textEditor = new TextEditor();
            textEditor.text = text;
            textEditor.OnFocus();
            textEditor.Copy();
#elif UNITY_IOS
            copy(text);
#elif UNITY_ANDROID
            aj.CallStatic("copy", text);
#endif
        }

        public static string Paste()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            TextEditor textEditor = new TextEditor();
            textEditor.Paste();
            return textEditor.text;
#elif UNITY_IOS
            return paste();
#elif UNITY_ANDROID
            return aj.CallStatic<string>("paste");
#else
            return "";
#endif
        }
        
#if UNITY_IOS        
        public static void SetAdvertiserTrackingEnabled(bool advertiserTrackingEnabled)
        {
            FBAdSettingsBridgeSetAdvertiserTrackingEnabled(advertiserTrackingEnabled);
        }
#endif 
        
        public static bool IOSSAvailableFourteenFive()
        {
#if UNITY_IOS && !UNITY_EDITOR
            return iOSSAvailableFourteenFive();
#endif
            return false;
        }

        public static void RequestAndLoadInterstitialAd()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            aj.CallStatic("requestAndLoadInterstitialAd");
#elif UNITY_IOS && !UNITY_EDITOR
            requestAndLoadInterstitialAd();
#endif
            return;
        }

        public static bool IsInterstitialAdReady()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return aj.CallStatic<bool>("isInterstitialAdReady");
#elif UNITY_IOS && !UNITY_EDITOR
            return isInterstitialAdReady();
#else
            return false;
#endif
        }

        public static void TryShowInterstitialAd()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            aj.CallStatic("tryShowInterstitialAd");
#elif UNITY_IOS && !UNITY_EDITOR
            tryShowInterstitialAd();
#endif
            return;
        }
        
        public static void RequestAndLoadRVAd()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            aj.CallStatic("requestAndLoadRVAd");
#elif UNITY_IOS && !UNITY_EDITOR
            requestAndLoadRVAd();
#endif
            return;
        }

        public static bool IsRVAdReady()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return aj.CallStatic<bool>("isRVAdReady");
#elif UNITY_IOS && !UNITY_EDITOR
            return isRVAdReady();
#else
            return false;
#endif
        }

        public static void TryShowRVAd()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            aj.CallStatic("tryShowRVAd");
#elif UNITY_IOS && !UNITY_EDITOR
            tryShowRVAd();
#endif
            return;
        }
    }
}
