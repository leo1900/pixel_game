
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
import android.text.method.ScrollingMovementMethod;
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


    public  static void RequestAppUpdate()
    {
        // 创建需要启动的Activity对应的Intent
        Intent intent = new Intent(mActivity,com.dragonplus.InAppUpdateActivity.class);
        // 启动intent对应的Activity
        mActivity.startActivity(intent);
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

   
}