    typedef void(*reauthorizeFacebookDataAccessIOS_CallBack)();

extern "C"
{
    bool isFacebookInstalledIOS();

    void openSetting();

    void openAppStoreRate(const char* rateUrl);

    bool isNotificationPermissionOpeniOS();
    
    bool isAppInstalledIOS(const char* scheme);
    
    void popupPrivacyIOS(const char* title, const char* message, const char* linkTitle, const char* linkUrl, const char* agreeTitle, const char* cancelTitle, const char* gameObjectName);
    
    void showRateUsViewControllerIOS(const char* url, const char* title, const char* message, const char* rateButtonText, const char* laterButtonText, const char* noButtonText, const char* gameObjectName);
    
    void saveDataToKeyChainIOS(const char* key, const char* data);
    
    const char* getDataFromKeyChainIOS(const char* key);
    
    const char* getVersionCodeIOS();
    
    int getScreenOrientationIOS();
    
    bool FBMessagerShareIOS(const char* linkUrl, const char* imageUrl, const char* pageId, const char* title, const char* subTitle, const char* buttonText);

    bool isFaceBookAccessTokenActiveIOS();

    bool isFacebookDataAccessExpiredIOS();

    void reauthorizeFacebookDataAccessIOS(reauthorizeFacebookDataAccessIOS_CallBack callback);

    bool isUserNotificationEnabled();
    bool needTrackingAuthorizationIOS();
    
    int ATTStatusIOS();

    void requestTrackingAuthorizationIOS(const char* gameObjectName);

    void copy(const char *text);

    const char* paste();
    
    bool iOSSAvailableFourteen();
    
    bool iOSSAvailableFourteenFive();

    void requestAndLoadInterstitialAd();

    bool isInterstitialAdReady();

    void tryShowInterstitialAd();

    void requestAndLoadRVAd();

    bool isRVAdReady();

    void tryShowRVAd();

    void init(const char* a1, const char* a2);
}
