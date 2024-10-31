
package com.dragonplus;

import android.Manifest;
import android.app.Activity;
import android.content.ActivityNotFoundException;
import android.content.ComponentName;
import android.content.Context;
import android.content.ServiceConnection;
import android.content.pm.PackageInfo;
import android.content.pm.PackageManager;
import android.content.Intent;
import android.content.res.AssetFileDescriptor;
import android.content.res.Resources;
import android.content.res.Configuration;
import android.media.AudioManager;
import android.media.SoundPool;
import android.net.Uri;
import android.os.Bundle;
import android.os.IBinder;
import android.os.RemoteException;
import androidx.annotation.NonNull;
import android.text.method.ScrollingMovementMethod;
import androidx.core.app.NotificationManagerCompat;
import android.provider.Settings;

import android.net.wifi.WifiManager;
import android.os.Build;
import android.os.Handler;

import androidx.core.content.ContextCompat;

import android.provider.Settings.Secure;
import android.telephony.TelephonyManager;

import java.io.IOException;
import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.Map;
import java.util.Random;

import androidx.appcompat.app.AlertDialog;
import android.content.DialogInterface;
import android.util.JsonReader;
import android.util.Log;
import android.view.View;


// import com.facebook.share.model.ShareMessengerGenericTemplateContent;
// import com.facebook.share.model.ShareMessengerGenericTemplateElement;
// import com.facebook.share.model.ShareMessengerURLActionButton;
import com.facebook.share.widget.MessageDialog;
import com.facebook.AccessToken;
import com.facebook.login.LoginManager;

import com.google.android.gms.ads.AdError;
import com.google.android.gms.ads.AdRequest;
import com.google.android.gms.ads.FullScreenContentCallback;
import com.google.android.gms.ads.LoadAdError;
import com.google.android.gms.ads.OnUserEarnedRewardListener;
import com.google.android.gms.ads.interstitial.InterstitialAd;
import com.google.android.gms.ads.interstitial.InterstitialAdLoadCallback;
import com.google.android.gms.ads.rewarded.RewardItem;
import com.google.android.gms.ads.rewarded.RewardedAd;
import com.google.android.gms.ads.rewarded.RewardedAdLoadCallback;
import com.unity3d.player.UnityPlayer;

import android.content.ClipData;
import android.content.ClipboardManager;

public class DragonNativeBridge {

    private static String TAG = "DragonNativeBridge";
    private static Context mContext;
    private static Activity mActivity;
    private static SoundPool pool;

    private static Map<String,Integer> soundMap;
    private static Map<Integer,String> soundIdMap;

    private static String soundDefaultPath;

    private static String mInterUnit;
    private static String mRvUnit;
    private static InterstitialAd mInterstitialAd;
    private static RewardedAd mRewardedAd;

    private static Random random;

    // private static MaterialDialog privacyDialog = null;

    public static void initialize(Context context,Activity activity, String interUnit, String rvUnit) {
        mContext = context;
        mActivity = activity;
        mInterUnit = interUnit;
        mRvUnit = rvUnit;

        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.LOLLIPOP) {
            pool = new SoundPool.Builder()
                    .setMaxStreams(30)
                    .build();
        } else {
            pool = new SoundPool(30, AudioManager.STREAM_MUSIC, 1);
        }

        soundMap = new HashMap<String,Integer>();
        soundIdMap = new HashMap<Integer, String>();

        pool.setOnLoadCompleteListener(new SoundPool.OnLoadCompleteListener() {
            @Override
            public void onLoadComplete(SoundPool soundPool, int sampleId, int status) {
                // if(soundIdMap.containsKey(sampleId)){
                Log.e(TAG,"sound "+ soundIdMap.get(sampleId) + " is loaded. sample id is "+ sampleId + " status is "+status);

                // soundMap.put(soundIdMap.get(sampleId),sampleId);
                // }
            }
        });

        random = new Random();

    }

    public static boolean isAppInstalled(String packageName){
        System.err.println("enter isAppInstalled " + packageName);
        PackageInfo packageInfo = null;
        try {
            synchronized(mContext){
                packageInfo = mContext.getPackageManager().getPackageInfo(packageName, 0);
            }
        } catch (PackageManager.NameNotFoundException e) {
            packageInfo = null;
            e.printStackTrace();
        }
        if (packageInfo == null) {
            return false;
        } else {
            return true;
        }
    }

    public static boolean isFacebookInstalled(){
        System.err.println("enter isFacebookInstalled");
        PackageInfo packageInfo = null;
        try {
            synchronized(mContext){
                packageInfo = mContext.getPackageManager().getPackageInfo("com.facebook.katana", 0);
            }
        } catch (PackageManager.NameNotFoundException e) {
            packageInfo = null;
            e.printStackTrace();
        }
        if (packageInfo == null) {
            return false;
        } else {
            return true;
        }
    }

    //"https://www.facebook.com/YourPageName";
    public static void openFacebookPage(String url,String pageId){
        if(url.isEmpty() && pageId.isEmpty()){
            return;
        }
        Intent facebookIntent = new Intent(Intent.ACTION_VIEW);
        facebookIntent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
        String facebookUrl = getFacebookPageURL(mContext,url,pageId);

        System.out.println("Url is "+ facebookUrl);

        facebookIntent.setData(Uri.parse(facebookUrl));

        try {
            mContext.startActivity(facebookIntent);
        }catch(Exception e){
            try {
                Intent webIntent = new Intent(Intent.ACTION_VIEW, Uri.parse(url));
                webIntent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
                mContext.startActivity(webIntent);
            } catch (Exception err) {
                err.printStackTrace();
            }
        }
    }


    public static void openTwitterPage(String twitterName,String twitterId) {
        if(twitterName.isEmpty() && twitterId.isEmpty()){
            return;
        }
        try {

            Uri uri = null;

            if(!twitterId.isEmpty()) {
                uri = Uri.parse("twitter://user?user_id=" + twitterId);
            }else{
                uri = Uri.parse("twitter://user?screen_name==" + twitterName);
            }

            Intent twitterIntent = new Intent(Intent.ACTION_VIEW,uri);
            twitterIntent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
            mContext.startActivity(twitterIntent);

        } catch (ActivityNotFoundException e) {
            try {
                Intent webIntent = new Intent(Intent.ACTION_VIEW, Uri.parse("https://twitter.com/" + twitterName));
                webIntent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
                mContext.startActivity(webIntent);
            } catch (Exception err) {
                err.printStackTrace();
            }
        }
    }

    public static void openInstagramPage(String instagramPageName,String instagramId) {
        if(instagramPageName.isEmpty() || instagramId.isEmpty()){
            return;
        }
        try {
            Uri uri = Uri.parse("http://instagram.com/_u/"+instagramPageName);
            Intent likeIng = new Intent(Intent.ACTION_VIEW, uri);
            likeIng.setPackage("com.instagram.android");
            likeIng.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
            mContext.startActivity(likeIng);

        } catch (ActivityNotFoundException e) {
            try {
                Intent webIntent = new Intent(Intent.ACTION_VIEW, Uri.parse("http://instagram.com/"+instagramPageName));
                webIntent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
                mContext.startActivity(webIntent);
            } catch (Exception err) {
                err.printStackTrace();
            }
        }
    }



    //method to get the right URL to use in the intent
    private static String getFacebookPageURL(Context context,String facebookUrl,String pageId) {
        PackageManager packageManager = context.getPackageManager();
        try {
            int versionCode = packageManager.getPackageInfo("com.facebook.katana", 0).versionCode;
            System.err.println("versioncode is " + versionCode);
            if(!pageId.isEmpty()){
                return "fb://page/" + pageId;
            } if (versionCode >= 3002850) { //newer versions of fb app
                return "fb://facewebmodal/f?href=" + facebookUrl;
            } else { //older versions of fb app
                return "fb://profile/" + pageId;
            }
        } catch (PackageManager.NameNotFoundException e) {
            return facebookUrl; //normal web url
        }
    }

    public static void popupPrivacy(final String title,final String message,final  String linkTitle, final String linkUrl,final String agreeTitle,final String cancelTitle,final String gameObjectName) {
        System.err.println("in popupPrivacy");
        if (title == null || message == null || linkTitle == null || linkUrl == null || agreeTitle == null || gameObjectName == null){
            return;
        }

        AlertDialog.Builder builder = new AlertDialog.Builder(mActivity);

        builder.setTitle(title);
        builder.setMessage(message);
        // Add the buttons
        builder.setPositiveButton(agreeTitle, new DialogInterface.OnClickListener() {
            public void onClick(DialogInterface dialog, int id) {
                // User clicked OK button
                UnityPlayer.UnitySendMessage(gameObjectName, "OnPrivacyAccepted","");
            }
        });
        builder.setNeutralButton(linkTitle, new DialogInterface.OnClickListener() {
            @Override
            public void onClick(DialogInterface dialog, int which) {
                //DETAIL
                Intent privacyIntent = new Intent(Intent.ACTION_VIEW);
                privacyIntent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
                System.out.println("Url is "+ linkUrl);

                privacyIntent.setData(Uri.parse(linkUrl));
                mContext.startActivity(privacyIntent);
            }
        });

        final AlertDialog dialog = builder.create();
        dialog.setCancelable(false);
        dialog.show();

        dialog.getButton(AlertDialog.BUTTON_NEUTRAL).setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View v)
            {
                Intent privacyIntent = new Intent(Intent.ACTION_VIEW);
                privacyIntent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
                System.out.println("Url is "+ linkUrl);

                privacyIntent.setData(Uri.parse(linkUrl));
                mContext.startActivity(privacyIntent);

                Boolean wantToCloseDialog = false;
                //Do stuff, possibly set wantToCloseDialog to true then...
                if(wantToCloseDialog)
                    dialog.dismiss();
                //else dialog stays open. Make sure you have an obvious way to close the dialog especially if you set cancellable to false.
            }
        });
    }


    public static String getVersionName() {
        try {
            PackageManager manager = mContext.getPackageManager();
            PackageInfo info = manager.getPackageInfo(mContext.getPackageName(), 0);
            return info.versionName;
        } catch (Exception e) {
            return "unknown";
        }
    }


    public static int getVersionCode() {
        try {
            PackageManager manager = mContext.getPackageManager();
            PackageInfo info = manager.getPackageInfo(mContext.getPackageName(), 0);
            return info.versionCode;
        } catch (Exception e) {
            return -1;
        }
    }

    public static String getMacAddress() {
        WifiManager wifi = (WifiManager) mContext.getSystemService(
                Context.WIFI_SERVICE);

        String wifiAddress = wifi.getConnectionInfo().getMacAddress();
        if (wifiAddress != null) {
            return wifiAddress;
        }
        try {
            boolean isWifiEnable = wifi.isWifiEnabled();
            if (!isWifiEnable) {
                wifi.setWifiEnabled(true);
            }
            for (int i = 0; i < 10; i++) {
                try {
                    Thread.sleep(1);
                    wifiAddress = wifi.getConnectionInfo().getMacAddress();
                } catch (InterruptedException e) {
                    // TODO Auto-generated catch block
                    e.printStackTrace();
                }
            }
            if (!isWifiEnable) {
                wifi.setWifiEnabled(false);
            }
            if (wifiAddress == null) {
                return "";
            }
            return wifiAddress;
        } catch (Exception ex) {
            return "";
        }
    }

    public static String getAndroidID() {
        return Secure.getString(mContext.getContentResolver(), Secure.ANDROID_ID);
    }

    public static String getDevIDShort() {
        String m_szDevIDShort = "35" + //we make this look like a valid IMEI
                Build.BOARD.length()%10+ Build.BRAND.length()%10 +
                Build.CPU_ABI.length()%10 + Build.DEVICE.length()%10 +
                Build.DISPLAY.length()%10 + Build.HOST.length()%10 +
                Build.ID.length()%10 + Build.MANUFACTURER.length()%10 +
                Build.MODEL.length()%10 + Build.PRODUCT.length()%10 +
                Build.TAGS.length()%10 + Build.TYPE.length()%10 +
                Build.USER.length()%10 ; //13 digits
        return m_szDevIDShort;
    }

    public static String getIMEI() {
        TelephonyManager TelephonyMgr = (TelephonyManager)mContext.getSystemService(Activity.TELEPHONY_SERVICE);
        if(TelephonyMgr != null) {
            if (ContextCompat.checkSelfPermission(mContext,
                    Manifest.permission.READ_PHONE_STATE)
                    != PackageManager.PERMISSION_GRANTED) {

                // No explanation needed, we can request the permission.
//暂不申请权限
//              ActivityCompat.requestPermissions((Activity) mContext,new String[]{
//                              Manifest.permission.READ_PHONE_STATE,},
//                      1010);

                return "";

            }
            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
                return TelephonyMgr.getImei();
            } else {
                return TelephonyMgr.getDeviceId(); // Requires READ_PHONE_STATE
            }
        }
        return "";
    }

    public static String getDeviceId() {

        String m_szLongID = getIMEI() + getDevIDShort() + getAndroidID() + getMacAddress();

        System.err.println("getIMEI : " + getIMEI());
        System.err.println("getDevIDShort : " + getDevIDShort());
        System.err.println("getAndroidID : " + getAndroidID());
        System.err.println("getMacAddress : " + getMacAddress());

        // compute md5
        MessageDigest m = null;
        try {
            m = MessageDigest.getInstance("MD5");
        } catch (NoSuchAlgorithmException e) {
            e.printStackTrace();
        }
        m.update(m_szLongID.getBytes(), 0, m_szLongID.length());
        // get md5 bytes
        byte p_md5Data[] = m.digest();
        // create a hex string
        String m_szUniqueID = new String();
        for (int i = 0; i < p_md5Data.length; i++) {
            int b = (0xFF & p_md5Data[i]);
            // if it is a single digit, make sure it have 0 in front (proper
            // padding)
            if (b <= 0xF)
                m_szUniqueID += "0";
            // add number to string
            m_szUniqueID += Integer.toHexString(b);
        } // hex string to uppercase
        m_szUniqueID = m_szUniqueID.toUpperCase();
        return m_szUniqueID;
    }


    public static String getDeviceType() {
        Resources resources = mContext.getResources();
        Configuration configuration = resources.getConfiguration();
        int screenLayout = configuration.screenLayout;
        int screenSize = screenLayout & Configuration.SCREENLAYOUT_SIZE_MASK;

        switch (screenSize) {
            case Configuration.SCREENLAYOUT_SIZE_SMALL:
            case Configuration.SCREENLAYOUT_SIZE_NORMAL:
                return "phone";
            case Configuration.SCREENLAYOUT_SIZE_LARGE:
            case 4:
                return "tablet";
            default:
                return "phone";
        }
    }


    public static int loadSound(String soundName){

        Log.e(TAG, "JJJJ soundName "+soundName);
        if(soundMap.containsKey(soundName)){
            return soundMap.get(soundName);
        }

        try {
            String filePath = soundDefaultPath+"/"+soundName+".mp3";
            Log.e(TAG,"file path is "+ filePath);
            AssetFileDescriptor afd = mActivity.getResources().getAssets().openFd(filePath);
            if(afd == null){
                Log.e(TAG, "JJJJafd is null");
                return -1;
            }
            int soundId = pool.load(afd ,1);
            if(!soundMap.containsKey(soundName)){
                soundMap.put(soundName,soundId);
            }

            soundIdMap.put(soundId,soundName);

            pool.play(soundId,0.0f,0.0f,1,0,1.0f);

            return soundId;
        }catch(IOException e){
            Log.e(TAG, "JJJJ读取失败");
            Log.e(TAG, e.getMessage());
            return -1;
        }

    }

    public static void initSoundPool(String soundPath){

        if(soundPath.isEmpty()){
            Log.e(TAG,"init path is null");
            return ;
        }

        if(soundPath.endsWith("/")){
            soundPath = soundPath.substring(0,soundPath.lastIndexOf("/"));
        }

        soundDefaultPath = soundPath;

        try {
            String[] files = mActivity.getResources().getAssets().list(soundDefaultPath);

            Log.e(TAG,"files names num is "+files.length);
            for(String name : files){
                Log.e(TAG,"JJJJ:"+name);
                System.err.println("JJJJ:"+name);
                if(name.endsWith(".mp3")) {

                    Log.e(TAG,"JJJJ init mp3 file name is "+name);

                    name = name.replace(".mp3","");

                    Log.e(TAG,"JJJJ init mp3 file name without mp3 is "+name);
                    loadSound(name);
                }

            }
        }catch(IOException e){
            Log.e(TAG,e.getMessage());
        }
    }

    public static void pauseAllSound(){
        pool.autoPause();
    }

    public static void resumeAllSound(){
        pool.autoResume();
    }

    public static void releaseSoundPool(){
        pool.release();
    }

    public static void stopSound(int streamId){
        pool.stop(streamId);
    }

    private static int playSound(String soundName,boolean loop){
        if(!soundMap.containsKey(soundName)){
            loadSound(soundName);
        }

        if(soundMap.containsKey(soundName)){
            int soundId = soundMap.get(soundName);

            int loopInt = loop ? -1 : 0;

            return pool.play(soundId,1,1,1,loopInt,1);
        }

        return -1;
    }

    public static int testSound(){
        try {
            String filePath = "Export/Audios/Sound/sfx_add_diamond.mp3";
            Log.e(TAG,"file path is "+ filePath);
            AssetFileDescriptor afd = mActivity.getResources().getAssets().openFd(filePath);
            if(afd == null){
                Log.e(TAG, "JJJJafd is null");
                return -1;
            }
            int soundId = pool.load(afd ,1);

            pool.play(soundId,1.0f,1.0f,1,0,1.0f);

        }catch(IOException e){
            Log.e(TAG, "JJJJ读取失败");
            Log.e(TAG, e.getMessage());
            return -1;
        }
        return -1;
    }

    public static int play(final String soundName, final  boolean loop){
        mActivity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                playSound(soundName,loop);
            }
        });
        return -1;
    }

    public static boolean FBMessagerShare(String linkUrl, String imageUrl, String pageId, String title, String subTitle, String buttonText){
        // ShareMessengerURLActionButton actionButton =
        //         new ShareMessengerURLActionButton.Builder()
        //                 .setTitle(buttonText)
        //                 .setUrl(Uri.parse(linkUrl))
        //                 .build();
        // ShareMessengerGenericTemplateElement genericTemplateElement =
        //         new ShareMessengerGenericTemplateElement.Builder()
        //                 .setTitle(title)
        //                 .setSubtitle(subTitle)
        //                 .setImageUrl(Uri.parse(imageUrl))
        //                 .setButton(actionButton)
        //                 .build();
        // ShareMessengerGenericTemplateContent genericTemplateContent =
        //         new ShareMessengerGenericTemplateContent.Builder()
        //                 .setPageId(pageId) // Your page ID, required
        //                 .setGenericTemplateElement(genericTemplateElement)
        //                 .build();

        // MessageDialog dialog = new MessageDialog(mActivity);

        // if (dialog.canShow(genericTemplateContent)) {
        //     dialog.show(mActivity, genericTemplateContent);
        //     return true;
        // }

        return false;

    }

   /**
     * 检查Facebook AccessToken 是否有效
     */
    public static boolean isFaceBookAccessTokenActive(){
        return AccessToken.isCurrentAccessTokenActive();
    }

    /**
     * 检查Facebook DataAccess 是否授权是否过期
     */
    public static boolean isFacebookDataAccessExpired(){
        AccessToken cur = AccessToken.getCurrentAccessToken();
        if(cur != null)
        {
            return cur.isDataAccessExpired();
        }
        else
        {
            return true;
        }
    }
    
    /**
     * 刷新 Facebook DataAccess 授权
     *
     */
    public static void reauthorizeFacebookDataAccess(){

        LoginManager.getInstance().reauthorizeDataAccess(mActivity);
    }

    public  static void RequestAppUpdate()
    {
        // 创建需要启动的Activity对应的Intent
        Intent intent = new Intent(mActivity,com.dragonplus.InAppUpdateActivity.class);
        // 启动intent对应的Activity
        mActivity.startActivity(intent);
    }

    public static boolean isUserNotificationEnabled()
    {
        return NotificationManagerCompat.from(mContext).areNotificationsEnabled();
    }

    public static void copy(String text) {
        ClipboardManager clipboard = (ClipboardManager) UnityPlayer.currentActivity.getSystemService(Context.CLIPBOARD_SERVICE);
        ClipData clipData = ClipData.newPlainText("text", text);
        clipboard.setPrimaryClip(clipData);
    }

    public static String paste() {
        ClipboardManager clipboard = (ClipboardManager) UnityPlayer.currentActivity.getSystemService(Context.CLIPBOARD_SERVICE);
        ClipData clipData = clipboard.getPrimaryClip();
        return clipData.getItemAt(0).getText().toString();
    }

    public static void requestAndLoadInterstitialAd(){
        String unit = (mInterUnit == null || mInterUnit.isEmpty()) ? "ca-app-pub-9923642111658707/1808088614" : mInterUnit;
        mActivity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                Handler handler = new Handler();
                handler.postDelayed(new Runnable() {
                    @Override
                    public void run() {
                        AdRequest adRequest = new AdRequest.Builder().build();

                        InterstitialAd.load(mActivity, unit, adRequest, new InterstitialAdLoadCallback() {
                            @Override
                            public void onAdLoaded(@NonNull InterstitialAd interstitialAd) {
                                // The mInterstitialAd reference will be null until
                                // an ad is loaded.
                                mInterstitialAd = interstitialAd;
                                Log.i(TAG, "onAdLoaded Inter");
                            }

                            @Override
                            public void onAdFailedToLoad(@NonNull LoadAdError loadAdError) {
                                // Handle the error
                                Log.i(TAG, loadAdError.getMessage());
                                mInterstitialAd = null;
                                requestAndLoadInterstitialAd();
                            }
                        });
                    }
                },30000);
            }
        });
    }

    public static void requestAndLoadRVAd(){
        String unit = (mRvUnit == null || mRvUnit.isEmpty()) ? "ca-app-pub-9923642111658707/5922647349" : mRvUnit;
        mActivity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                Handler handler = new Handler();
                handler.postDelayed(new Runnable() {
                    @Override
                    public void run() {
                        AdRequest adRequest = new AdRequest.Builder().build();

                        RewardedAd.load(mActivity, unit, adRequest, new RewardedAdLoadCallback() {
                            @Override
                            public void onAdLoaded(@NonNull RewardedAd rewardedAd) {
                                // The mInterstitialAd reference will be null until
                                // an ad is loaded.
                                mRewardedAd = rewardedAd;
                                Log.i(TAG, "onAdLoaded RV");
                            }

                            @Override
                            public void onAdFailedToLoad(@NonNull LoadAdError loadAdError) {
                                // Handle the error
                                Log.i(TAG, loadAdError.getMessage());
                                mRewardedAd = null;
                                requestAndLoadRVAd();
                            }
                        });
                    }
                },30000);
            }
        });
    }

    public static boolean isInterstitialAdReady()
    {
        if(mInterstitialAd == null){
            return false;
        }
        int test = random.nextInt(100);

        if(test <= 7){
            return true;
        }

        return false;
    }

    public static boolean isRVAdReady()
    {
        if(mRewardedAd == null){
            return false;
        }
   
        return false;
    }

    public static void tryShowInterstitialAd(){
        if(mInterstitialAd == null){
            return;
        }
        mInterstitialAd.setFullScreenContentCallback(new FullScreenContentCallback(){
            @Override
            public void onAdDismissedFullScreenContent() {
                // Called when fullscreen content is dismissed.
                Log.d("TAG", "The ad was dismissed.");
                mInterstitialAd = null;
                requestAndLoadInterstitialAd();
            }

            @Override
            public void onAdFailedToShowFullScreenContent(AdError adError) {
                // Called when fullscreen content failed to show.
                Log.d("TAG", "The ad failed to show.");
                mInterstitialAd = null;
                requestAndLoadInterstitialAd();
            }

            @Override
            public void onAdShowedFullScreenContent() {
                // Called when fullscreen content is shown.
                // Make sure to set your reference to null so you don't
                // show it a second time.
                mInterstitialAd = null;
                Log.d("TAG", "The ad was shown.");
                requestAndLoadInterstitialAd();
            }
        });

        mActivity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                mInterstitialAd.show(mActivity);
            }
        });

    }

    public static void tryShowRVAd(){
        if(mRewardedAd == null){
            return;
        }
        mRewardedAd.setFullScreenContentCallback(new FullScreenContentCallback(){
            @Override
            public void onAdDismissedFullScreenContent() {
                // Called when fullscreen content is dismissed.
                Log.d("TAG", "The ad was dismissed.");
                mRewardedAd = null;
                requestAndLoadRVAd();
            }

            @Override
            public void onAdFailedToShowFullScreenContent(AdError adError) {
                // Called when fullscreen content failed to show.
                Log.d("TAG", "The ad failed to show.");
                mRewardedAd = null;
                requestAndLoadRVAd();
            }

            @Override
            public void onAdShowedFullScreenContent() {
                // Called when fullscreen content is shown.
                // Make sure to set your reference to null so you don't
                // show it a second time.
                mRewardedAd = null;
                Log.d("TAG", "The ad was shown.");
                requestAndLoadRVAd();
            }
        });

        mActivity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                mRewardedAd.show(mActivity, new OnUserEarnedRewardListener() {
                    @Override
                    public void onUserEarnedReward(@NonNull RewardItem rewardItem) {
                        Log.d("TAG", "The rv ad was shown.");
                    }
                });
            }
        });

    }
    
     public static void openNotificationSetting() {
            try {
                // 根据通知栏开启权限判断结果，判断是否需要提醒用户跳转系统通知管理页面
                Intent intent = new Intent();
                intent.setAction(Settings.ACTION_APP_NOTIFICATION_SETTINGS);
                //这种方案适用于 API 26, 即8.0（含8.0）以上可以用
                intent.putExtra(Settings.EXTRA_APP_PACKAGE, mActivity.getPackageName());
                intent.putExtra(Settings.EXTRA_CHANNEL_ID, mActivity.getApplicationInfo().uid);
                //这种方案适用于 API21——25，即 5.0——7.1 之间的版本可以使用
                intent.putExtra("app_package", mActivity.getPackageName());
                intent.putExtra("app_uid", mActivity.getApplicationInfo().uid);
                mActivity.startActivity(intent);
            } catch (Exception e) {
                e.printStackTrace();
                // 出现异常则跳转到应用设置界面
                Intent intent = new Intent();
                intent.setAction(Settings.ACTION_APPLICATION_DETAILS_SETTINGS);
                Uri uri = Uri.fromParts("package", mActivity.getPackageName(), null);
                intent.setData(uri);
                mActivity.startActivity(intent);
            }
        }
}