using System;
using System.Collections.Specialized;
using FrameWork.Asset;

namespace FrameWork.SDKEvents
{

    [Obsolete("DragonU3DSDK.SDKEvents.ConnectResServerSuccessEvent is deprecated")]
    public class ConnectResServerSuccessEvent : Subject<ConnectResServerSuccessEvent>
    {
    }

    [Obsolete("DragonU3DSDK.SDKEvents.ConnectResServerFailEvent is deprecated")]
    public class ConnectResServerFailEvent : Subject<ConnectResServerFailEvent>
    {
        public string Error;
        public ConnectResServerFailEvent Data(string error)
        {
            Error = error;
            return this;
        }
    }

    [Obsolete("DragonU3DSDK.SDKEvents.LoginEvent is deprecated")]
    public class LoginEvent: Subject<LoginEvent>
    {
    }

    public class ProfileReplacedEvent: Subject<ProfileReplacedEvent>
    {
        public bool clear;
        public ProfileReplacedEvent Data(bool clear)
        {
            this.clear = clear;
            return this;
        }
    }

    [Obsolete("DragonU3DSDK.SDKEvents.ProfileCreatedEvent is deprecated")]
    public class ProfileCreatedEvent: Subject<ProfileCreatedEvent>
    {
    }

    [Obsolete("DragonU3DSDK.SDKEvents.ProfileFetchedEvent is deprecated")]
    public class ProfileFetchedEvent: Subject<ProfileFetchedEvent>
    {
    }

    [Obsolete("DragonU3DSDK.SDKEvents.ProfileResolvedEvent is deprecated")]
    public class ProfileResolvedEvent: Subject<ProfileResolvedEvent>
    {
        public bool server;
        public ProfileResolvedEvent Data(bool server)
        {
            this.server = server;
            return this;
        }
    }

    public class AccountLoginOtherDeviceEvent : Subject<AccountLoginOtherDeviceEvent>
    {
        public AccountLoginOtherDeviceEvent Data()
        {
            return this;
        }
    }

    [Obsolete("DragonU3DSDK.SDKEvents.AdsLoadFailEvent is deprecated")]
    public class AdsLoadFailEvent : Subject<AdsLoadFailEvent>
    {
        public AdsLoadFailEvent Data()
        {
            return this;
        }
    }

    [Obsolete("DragonU3DSDK.SDKEvents.AdsCloseEvent is deprecated")]
    public class AdsCloseEvent : Subject<AdsCloseEvent>
    {
        public AdsCloseEvent Data()
        {
            return this;
        }
    }

    [Obsolete("DragonU3DSDK.SDKEvents.CheckUpdateEvent is deprecated")]
    public class CheckUpdateEvent: Subject<CheckUpdateEvent>
    {
        public enum CheckUpdateEventType
        {
            CHECK_UPDATE_EVENT_TYPE_START = 0,
            CHECK_UPDATE_EVENT_TYPE_FINISH = 1,
            CHECK_UPDATE_EVENT_TYPE_FAILURE = 2,
        };

        public CheckUpdateEventType checkUpdateEventType;
        public AppUpdateResult updateResult;
        public string errno;
        
        public CheckUpdateEvent Data(CheckUpdateEventType checkUpdateEventType, AppUpdateResult result, string errno = null)
        {
            this.checkUpdateEventType = checkUpdateEventType;
            this.updateResult = result;
            this.errno = errno;
            return this;
        }
        
        
    }

    [Obsolete("DragonU3DSDK.SDKEvents.AdjustIdUpdatedEvent is deprecated")]
    public class AdjustIdUpdatedEvent: Subject<AdjustIdUpdatedEvent>
    {
    }

    // [Obsolete("DragonU3DSDK.SDKEvents.DownloadFileEvent is deprecated")]
    public class DownloadFileEvent: Subject<DownloadFileEvent>
    {
        public string name;
        public string stage;
        public string data;
        public int bytesDownloaded;

        public DownloadFileEvent Data(string name, string stage, int bytesDownloaded, string data = "")
        {
            this.name = name;
            this.stage = stage;
            this.bytesDownloaded = bytesDownloaded;
            this.data = data;
            return this;
        }
    }
    
    public class ABTestRequestFinishEvent : Subject<ABTestRequestFinishEvent>
    {
        public bool success;
        public ABTestRequestFinishEvent Data(bool success)
        {
            this.success = success;
            return this;
        }
    }

    public class DeepLinkEvent: Subject<DeepLinkEvent>
    {
        public string route;
        public string title;
        public string content;
        public NameValueCollection rawData;

        public DeepLinkEvent Data(NameValueCollection nvc)
        {
            if(nvc == null)
            {
                return this;
            }

            foreach(string key in nvc.AllKeys)
            {
                if (key.Equals("route"))
                {
                    this.route = nvc.Get(key);
                    continue;
                }

                if (key.Equals("title"))
                {
                    this.title = nvc.Get(key);
                    continue;
                }

                if (key.Equals("content"))
                {
                    this.content = nvc.Get(key);
                    continue;
                }
            }

            this.rawData = nvc;

            return this;
        }
    }

    public class NativeVersionChanged: Subject<NativeVersionChanged>
    {
        public string OldVersion;
        public string NewVersion;

        public NativeVersionChanged Data(string oldVersion, string newVersion)
        {
            OldVersion = oldVersion;
            NewVersion = newVersion;
            return this;
        }
    }

    public class SocketIOConnected: Subject<SocketIOConnected>
    {

    }

    public class SocketIODisconnected : Subject<SocketIODisconnected>
    {

    }
    public class SocketIOError : Subject<SocketIOError>
    {

    }

    public class FirebaseInstanceIdReceived : Subject<FirebaseInstanceIdReceived>
    {
        public string FirebaseInstanceId;
        public FirebaseInstanceIdReceived Data(string firebaseInstanceId)
        {
            FirebaseInstanceId = firebaseInstanceId;
            return this;
        }
    }
    public class IAPInitialized : Subject<IAPInitialized>
    {
    }

 

    public class DiskFullEvent : Subject<DiskFullEvent>
    {
    }

    public class AssetCheckClearEvent : Subject<AssetCheckClearEvent>
    {
    }

    public class ConfirmWindowEvent : Subject<ConfirmWindowEvent>
    {
        public class UIData
        {
            public string DescString = "";            // 提示的文字
            public Action OKCallback = null;          // 确认按钮的回掉,传不传都会关闭确认框
            public string OKButtonText = null;        // 确认按钮的文字,如果不传为OK
            public bool HasCancelButton = false;      // 是否有取消按钮
            public Action CancelCallback = null;      // 取消按钮的回掉,传不传都会关闭确认框
            public string CancelButtonText = null;    // 取消按钮的文字,如果不传为Calcel
            public bool HasCloseButton = true;        // 是否有关闭按钮
            public bool HasOkButton = true;
        }

        public UIData UiData;
        
        public ConfirmWindowEvent Data(UIData _data)
        {
            UiData = _data;
            return this;
        }
    }
    
    public class AdsOnCurrencySpend : Subject<AdsOnCurrencySpend>
    {
        public string name;
        public int cut;
        
        public AdsOnCurrencySpend Data(string _name, int _cut)
        {
            DebugUtil.Log(string.Format("AdsOnCurrencySpend : {0} {1}", _name, _cut));
            
            name = _name;
            cut = _cut;
            if (cut < 0)
            {
                cut = 0;
            }
            return this;
        }
    }
    
    public class ConfigHubUpdatedEvent : Subject<ConfigHubUpdatedEvent>
    {
    }

}
