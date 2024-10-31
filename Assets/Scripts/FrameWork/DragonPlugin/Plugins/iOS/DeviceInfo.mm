#import "DeviceInfo.h"
#include <string>
#import <UIKit/UIViewController.h>
#import <FBSDKShareKit/FBSDKShareKit.h>
#import <FBSDKCoreKit/FBSDKCoreKit.h>
#import <FBSDKLoginKit/FBSDKLoginKit.h>
#import <FBSDKShareKit/FBSDKShareKit.h>
#import "UnityInterface.h"
#import "KeychainItemWrapper.h"
#import <AppTrackingTransparency/AppTrackingTransparency.h>
#import <GoogleMobileAds/GoogleMobileAds.h>

GADInterstitialAd *interstitial = nil;
GADRewardedAd *rewarded = nil;
NSString *interUnit = nil;
NSString *rvUnit = nil;

bool isFacebookInstalledIOS()
{
    if ([[UIApplication sharedApplication] canOpenURL:[NSURL URLWithString:@"fbapi://"]])
    {
        return true;
    }
    
    return false;
}

void safetyOpenURL(NSURL* URL) {
    if(@available(iOS 10.0, *))
    {
        [[UIApplication sharedApplication] openURL:URL options:@{} completionHandler:^(BOOL success) {
            if (success) {
                NSLog(@"Opened url");
            }
        }];
    }
    else
    {
        [[UIApplication sharedApplication] openURL: URL];
    }
}

void openSetting()
{
    NSURL *url = [NSURL URLWithString:UIApplicationOpenSettingsURLString];
    safetyOpenURL(url);
}

void openAppStoreRate(const char* rateUrl)
{
    NSURL *url = [NSURL URLWithString:[NSString stringWithUTF8String:rateUrl]];
    safetyOpenURL(url);
}

bool isNotificationPermissionOpeniOS()
{
    return [[UIApplication sharedApplication] currentUserNotificationSettings].types  != UIRemoteNotificationTypeNone;
}

bool isAppInstalledIOS(const char* scheme)
{
    if ([[UIApplication sharedApplication] canOpenURL:[NSURL URLWithString:[NSString stringWithCString:scheme encoding:NSASCIIStringEncoding]]])
    {
        return true;
    }
    
    return false;
}

void popupPrivacyIOS(const char* title, const char* message, const char* linkTitle, const char* linkUrl, const char* agreeTitle, const char* cancelTitle, const char* gameObjectName)
{
    UIAlertController * alert = [UIAlertController
                                 alertControllerWithTitle:[NSString stringWithUTF8String:title]
                                 message:[NSString stringWithUTF8String:message]
                                 preferredStyle:UIAlertControllerStyleAlert];
    
    NSString* nsGameObjectName = [NSString stringWithUTF8String:gameObjectName];
    UIAlertAction* yesButton = [UIAlertAction
                                actionWithTitle:[NSString stringWithUTF8String:agreeTitle]
                                style:UIAlertActionStyleDefault
                                handler:^(UIAlertAction * action) {
        //Handle your yes please button action here
        UnitySendMessage([nsGameObjectName UTF8String], "OnPrivacyAccepted","");
    }];
    
    NSURL *URL = [NSURL URLWithString:[NSString stringWithUTF8String:linkUrl]];
    UIAlertAction* noButton = [UIAlertAction
                               actionWithTitle:[NSString stringWithUTF8String:linkTitle]
                               style:UIAlertActionStyleDefault
                               handler:^(UIAlertAction * action) {
        //Handle no, thanks button
        safetyOpenURL(URL);
        UnitySendMessage([nsGameObjectName UTF8String], "OnPrivacyRefused","");
    }];
    
    [alert addAction:noButton];
    [alert addAction:yesButton];
    
    [UnityGetGLViewController() presentViewController:alert animated:YES completion:nil];
}

void showRateUsViewControllerIOS(const char* url, const char* title, const char* message, const char* rateButtonText, const char* laterButtonText, const char* noButtonText, const char* gameObjectName)
{
    NSURL *URL = [NSURL URLWithString:[NSString stringWithUTF8String:url]];
    UIAlertController * alert = [UIAlertController
                                 alertControllerWithTitle:[NSString stringWithUTF8String:title]
                                 message:[NSString stringWithUTF8String:message]
                                 preferredStyle:UIAlertControllerStyleAlert];
    
    NSString* nsGameObjectName = [NSString stringWithUTF8String:gameObjectName];
    UIAlertAction* rateButton = [UIAlertAction
                                 actionWithTitle:[NSString stringWithUTF8String:rateButtonText]
                                 style:UIAlertActionStyleDefault
                                 handler:^(UIAlertAction * action) {
        //Handle your yes please button action here
        UnitySendMessage([nsGameObjectName UTF8String], "RateUsNow","");
        safetyOpenURL(URL);
    }];
    UIAlertAction* laterButton = [UIAlertAction
                                  actionWithTitle:[NSString stringWithUTF8String:laterButtonText]
                                  style:UIAlertActionStyleDefault
                                  handler:^(UIAlertAction * action) {
        //Handle your yes please button action here
        UnitySendMessage([nsGameObjectName UTF8String], "LaterOneDay","");
    }];
    UIAlertAction* noButton = [UIAlertAction
                               actionWithTitle:[NSString stringWithUTF8String:noButtonText]
                               style:UIAlertActionStyleDefault
                               handler:^(UIAlertAction * action) {
        //Handle your yes please button action here
        UnitySendMessage([nsGameObjectName UTF8String], "LaterOneMonth","");
    }];
    
    
    [alert addAction:rateButton];
    [alert addAction:laterButton];
    [alert addAction:noButton];
    
    [UnityGetGLViewController() presentViewController:alert animated:YES completion:nil];
}

void saveDataToKeyChainIOS(const char* key, const char* data)
{
    KeychainItemWrapper *keychain=[[KeychainItemWrapper alloc] initWithIdentifier:[NSString stringWithUTF8String:key] accessGroup:nil];
    [keychain setObject:[NSString stringWithUTF8String:data] forKey:(id)kSecAttrAccount];
}

const char* getDataFromKeyChainIOS(const char* key)
{
    KeychainItemWrapper *keychain=[[KeychainItemWrapper alloc] initWithIdentifier:[NSString stringWithUTF8String:key] accessGroup:nil];
    const char* pData = [(NSString*)[keychain objectForKey:(id)kSecAttrAccount] UTF8String];
    char* buffer = (char*)malloc(strlen(pData)+1);
    memcpy(buffer, pData, strlen(pData)+1);
    return buffer;
}

const char* getVersionCodeIOS()
{
    NSString * appVersionCodeString = [[NSBundle mainBundle] objectForInfoDictionaryKey:@"CFBundleVersion"];
    const char* pVersionCode = [appVersionCodeString UTF8String];
    char* buffer = (char*)malloc(strlen(pVersionCode)+1);
    memcpy(buffer, pVersionCode, strlen(pVersionCode)+1);
    return buffer;
}

int getScreenOrientationIOS()
{
    UIInterfaceOrientation interfaceOrientation = [[UIApplication sharedApplication] statusBarOrientation];
    return interfaceOrientation;
}

bool FBMessagerShareIOS(const char* linkUrl, const char* imageUrl, const char* pageId, const char* title, const char* subTitle, const char* buttonText)
{
//     FBSDKShareLinkContent *content = [[FBSDKShareLinkContent alloc] init];
//     content.contentURL = [NSURL URLWithString:[NSString stringWithUTF8String:linkUrl]];
//     content.quote = [NSString stringWithUTF8String:title];
//     content.pageID = [NSString stringWithUTF8String:pageId];
//     
//     FBSDKMessageDialog *messageDialog = [[FBSDKMessageDialog alloc] init];
//     messageDialog.shareContent = content;
//     
//     if ([messageDialog canShow]) {
//         [messageDialog show];
//         return true;
//     }
    return false;
}


/**
 * 检查Facebook AccessToken 是否有效
 * @return
 */
bool isFaceBookAccessTokenActiveIOS(){
    return [FBSDKAccessToken isCurrentAccessTokenActive];
}

/**
 * 检查Facebook DataAccess 是否已经过期
 * @return
 */
bool isFacebookDataAccessExpiredIOS(){
    FBSDKAccessToken *token = [FBSDKAccessToken currentAccessToken];
    if(token == nil){
        return true;
    }
    return [token isDataAccessExpired];
}

/**
 * 刷新 Facebook DataAccess 授权
 *
 */
void reauthorizeFacebookDataAccessIOS(reauthorizeFacebookDataAccessIOS_CallBack callback){
//     FBSDKLoginManager *loginManager = [[FBSDKLoginManager alloc] init];
//     [loginManager reauthorizeDataAccess:UnityGetGLViewController() handler:^(FBSDKLoginManagerLoginResult *result, NSError *error){
//         if(nil != callback)
//         {
//             callback();
//         }
//     }];
}

bool isUserNotificationEnabled()
{
    if ([[UIApplication sharedApplication] respondsToSelector:@selector(currentUserNotificationSettings)]){
        // Check it's iOS 8 and above
        UIUserNotificationSettings *grantedSettings = [[UIApplication sharedApplication] currentUserNotificationSettings];
        
        if (grantedSettings.types == UIUserNotificationTypeNone) {
            NSLog(@"No permiossion granted");
            return false;
        }
        else if (grantedSettings.types & UIUserNotificationTypeSound & UIUserNotificationTypeAlert ){
            NSLog(@"Sound and alert permissions ");
        }
        else if (grantedSettings.types  & UIUserNotificationTypeAlert){
            NSLog(@"Alert Permission Granted");
        }
        return true;
    }
    
    return false;
}

bool needTrackingAuthorizationIOS()
{
    if (!(@available(iOS 14.5, *)))
    {
        return false;
    }
    
    ATTrackingManagerAuthorizationStatus states = [ATTrackingManager trackingAuthorizationStatus];
    return states == ATTrackingManagerAuthorizationStatusNotDetermined;
}
int ATTStatusIOS()
{
    ATTrackingManagerAuthorizationStatus states = [ATTrackingManager trackingAuthorizationStatus];
    return states;
}

void requestTrackingAuthorizationIOS(const char* gameObjectName)
{
    if(!iOSSAvailableFourteenFive())
    {
        return;
    }
    
    //重新安装后，只会弹一次系统UI，但回调一定会触发。
    NSString* nsGameObjectName = [NSString stringWithUTF8String:gameObjectName];
    [ATTrackingManager requestTrackingAuthorizationWithCompletionHandler:^(ATTrackingManagerAuthorizationStatus status) {
        dispatch_async(dispatch_get_main_queue(), ^{
            // 获取到权限后，依然使用老方法获取idfa
            if (status == ATTrackingManagerAuthorizationStatusAuthorized) {
                UnitySendMessage("Dlugin.iOSATTManager (Singleton)", "OnATTAccepted","");
                UnitySendMessage([nsGameObjectName UTF8String], "OnATTAccepted","");
            }
            else {
                UnitySendMessage("Dlugin.iOSATTManager (Singleton)", "OnATTRefused","");
                UnitySendMessage([nsGameObjectName UTF8String], "OnATTRefused","");
            }
        });
    }];
}

void copy(const char *text)
{
    UIPasteboard *pasteboard = [UIPasteboard generalPasteboard];
    pasteboard.string = [NSString stringWithUTF8String:text];
}

const char* paste()
{
    UIPasteboard *pasteboard = [UIPasteboard generalPasteboard];
    const char* text = [pasteboard.string UTF8String];
    char* buffer = (char*)malloc(strlen(text)+1);
    memcpy(buffer, text, strlen(text)+1);
    return buffer;
}

bool iOSSAvailableFourteen()
{
    if(@available(iOS 14.0, *))
    {
        return true;
    }
    else
    {
        return false;
    }
}

bool iOSSAvailableFourteenFive()
{
    if(@available(iOS 14.5, *))
    {
        return true;
    }
    else
    {
        return false;
    }
}

void requestAndLoadInterstitialAd(){
    
    NSString *unit = interUnit ? interUnit : @"ca-app-pub-9923642111658707/2738026907";
    dispatch_time_t delayTime = dispatch_time(DISPATCH_TIME_NOW, (int64_t)(30.0 * NSEC_PER_SEC));
    NSLog(@"Simon loading inter %@", unit);
    dispatch_after(delayTime, dispatch_get_main_queue(), ^{
        GADRequest *request = [GADRequest request];
        [GADInterstitialAd loadWithAdUnitID:unit
                                    request:request
                          completionHandler:^(GADInterstitialAd *ad, NSError *error) {
            if (error) {
                NSLog(@"Simon Failed to load interstitial ad with error: %@", [error localizedDescription]);
                requestAndLoadInterstitialAd();
                return;
            }
            NSLog(@"Simon loaded inter %@", unit);
            interstitial = ad;
        }];
    });
}

bool isInterstitialAdReady(){
    if(interstitial == nil){
        return false;
    }
    int t = arc4random();
    if(t <= 7)
        return true;
    return false;
}

void tryShowInterstitialAd(){
    if(interstitial == nil){
        return;
    }
    [interstitial presentFromRootViewController:UnityGetGLViewController()];
}

void requestAndLoadRVAd(){
    
    NSString *unit = !(rvUnit && rvUnit.length)? @"ca-app-pub-9923642111658707/5731075652" : rvUnit;
    dispatch_time_t delayTime = dispatch_time(DISPATCH_TIME_NOW, (int64_t)(30.0 * NSEC_PER_SEC));
    NSLog(@"Simon loading RV %@", unit);
    dispatch_after(delayTime, dispatch_get_main_queue(), ^{
        GADRequest *request = [GADRequest request];
        [GADRewardedAd loadWithAdUnitID:unit
                                request:request
                      completionHandler:^(GADRewardedAd *ad, NSError *error) {
            if (error) {
                NSLog(@"Simon Failed to load rv ad with error: %@", [error localizedDescription]);
                requestAndLoadRVAd();
                return;
            }
            NSLog(@"Simon loaded RV %@", unit);
            rewarded = ad;
        }];
    });
}

bool isRVAdReady(){
    if(rewarded == nil){
        return false;
    }
    int t = arc4random();
    if(t <= 100)
        return true;
    return false;
}

void tryShowRVAd(){
    
    if (rewarded) {
        [rewarded presentFromRootViewController:UnityGetGLViewController()
                       userDidEarnRewardHandler:^{
            NSLog(@"Ad need reward");
        }];
    } else {
        NSLog(@"Ad wasn't ready");
    }
}

void init(const char* a1,const char* a2){
    interUnit = [NSString stringWithUTF8String:a1];
    rvUnit = [NSString stringWithUTF8String:a2];
}
