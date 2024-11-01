package com.catswimming;

import android.app.Activity;
import android.content.Intent;
import android.content.IntentSender;
import android.os.Bundle;
import androidx.annotation.Nullable;

import com.google.android.play.core.appupdate.AppUpdateInfo;
import com.google.android.play.core.appupdate.AppUpdateManager;
import com.google.android.play.core.appupdate.AppUpdateManagerFactory;
import com.google.android.play.core.install.InstallState;
import com.google.android.play.core.install.InstallStateUpdatedListener;
import com.google.android.play.core.install.model.AppUpdateType;
import com.google.android.play.core.install.model.InstallStatus;
import com.google.android.play.core.install.model.UpdateAvailability;
import com.google.android.gms.tasks.OnFailureListener;
import com.google.android.gms.tasks.OnSuccessListener;
import com.google.android.gms.tasks.Task;

public class InAppUpdateActivity extends Activity {

    private AppUpdateManager appUpdateManager;
    private Activity mActivity;
    private int MY_REQUEST_CODE = -1;
    private InstallStateUpdatedListener mInstallStateUpdatedListener;

    @Override
    protected void onCreate(@Nullable Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        mActivity = this;
        // Creates instance of the manager.
        appUpdateManager = AppUpdateManagerFactory.create(this);
        mInstallStateUpdatedListener = new InstallStateUpdatedListener() {
            @Override
            public void onStateUpdate(InstallState installState) {
                if (installState.installStatus() == InstallStatus.DOWNLOADED) {
                    // After the update is downloaded, show a notification
                    // and request user confirmation to restart the app.
                    popupSnackbarForCompleteUpdate();
                }
            }
        };
        appUpdateManager.registerListener(mInstallStateUpdatedListener);

        RequestUpdateInfoState();
    }


    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);
        if (requestCode == MY_REQUEST_CODE) {
            if (resultCode != RESULT_OK) {
                System.out.println("Update flow failed! Result code: " + resultCode);
                // If the update is cancelled or fails,
                // you can request to start the update again.
            }
        }

        Close();
    }


    @Override
    protected void onResume() {
        super.onResume();
        RequestUpdateInfoState();
    }

    private void RequestUpdateInfoState()
    {
        System.out.println("RequestUpdateInfoState 1");
        // Returns an intent object that you use to check for an update.
        Task<AppUpdateInfo> appUpdateInfoTask = appUpdateManager.getAppUpdateInfo();

        // Checks that the platform will allow the specified type of update.
        appUpdateInfoTask.addOnSuccessListener(new OnSuccessListener<AppUpdateInfo>() {
            @Override
            public void onSuccess(AppUpdateInfo appUpdateInfo) {
                // Task completed successfully
                // ...
                if (appUpdateInfo.installStatus() == InstallStatus.DOWNLOADED) {
                    System.out.println("RequestUpdateInfoState 2");
                    popupSnackbarForCompleteUpdate();
                    Close();
                }
                else if (appUpdateInfo.updateAvailability() == UpdateAvailability.DEVELOPER_TRIGGERED_UPDATE_IN_PROGRESS) {
                    MY_REQUEST_CODE = 1;
                    // If an in-app update is already running, resume the update.
                    try {
                        System.out.println("RequestUpdateInfoState 3");
                        appUpdateManager.startUpdateFlowForResult(
                                appUpdateInfo,
                                AppUpdateType.IMMEDIATE,
                                mActivity,
                                MY_REQUEST_CODE);
                    } catch (IntentSender.SendIntentException e) {
                        e.printStackTrace();
                        System.out.println("RequestUpdateInfoState 4");
                        Close();
                    }
                }
               else if (appUpdateInfo.updateAvailability() == UpdateAvailability.UPDATE_AVAILABLE)
                {
                    // For a flexible update, use AppUpdateType.FLEXIBLE
                    if (appUpdateInfo.isUpdateTypeAllowed(AppUpdateType.IMMEDIATE))
                    {
                        MY_REQUEST_CODE = 1;
                        // Request the update.
                        try {
                            System.out.println("RequestUpdateInfoState 5");
                            appUpdateManager.startUpdateFlowForResult(
                                    // Pass the intent that is returned by 'getAppUpdateInfo()'.
                                    appUpdateInfo,
                                    // Or 'AppUpdateType.FLEXIBLE' for flexible updates.
                                    AppUpdateType.IMMEDIATE,
                                    // The current activity making the update request.
                                    mActivity,
                                    // Include a request code to later monitor this update request.
                                    MY_REQUEST_CODE);
                        } catch (IntentSender.SendIntentException e) {
                            System.out.println("RequestUpdateInfoState 6");
                            e.printStackTrace();
                            Close();
                        }

                    }
                    else if (appUpdateInfo.isUpdateTypeAllowed(AppUpdateType.FLEXIBLE))
                    {
                        MY_REQUEST_CODE = 2;
                        // Request the update.
                        try {
                            System.out.println("RequestUpdateInfoState 7");
                            appUpdateManager.startUpdateFlowForResult(
                                    // Pass the intent that is returned by 'getAppUpdateInfo()'.
                                    appUpdateInfo,
                                    // Or 'AppUpdateType.FLEXIBLE' for flexible updates.
                                    AppUpdateType.FLEXIBLE,
                                    // The current activity making the update request.
                                    mActivity,
                                    // Include a request code to later monitor this update request.
                                    MY_REQUEST_CODE);
                        } catch (IntentSender.SendIntentException e) {
                            System.out.println("RequestUpdateInfoState 8");
                            e.printStackTrace();
                            Close();
                        }
                    }
                    else
                    {
                        System.out.println("RequestUpdateInfoState 9");
                        Close();
                    }
                }
                else
                {
                    System.out.println("RequestUpdateInfoState 10");
                    Close();
                }
            }
        }).addOnFailureListener(new OnFailureListener() {
            @Override
            public void onFailure(Exception e) {
                System.out.println("appUpdateInfoTask failed!");
                System.out.println("RequestUpdateInfoState 11");
                Close();
            }
        });

    }


    /* Displays the snackbar notification and call to action. */
    private void popupSnackbarForCompleteUpdate() {
        appUpdateManager.completeUpdate();
    }

    private void Close() {
        appUpdateManager.unregisterListener(mInstallStateUpdatedListener);
        finish();
    }

}
