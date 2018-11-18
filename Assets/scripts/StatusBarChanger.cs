using System;
using UnityEngine;


public class StatusBarChanger : MonoBehaviour
{
    private static int newStatusBarValue;

    public void Awake(){
        Show();
    }

    public static void Show()
    {
#if UNITY_ANDROID
        setStatusBarValue(2048); // WindowManager.LayoutParams.FLAG_FORCE_NOT_FULLSCREEN
#endif
    }

    public static void Hide()
    {
#if UNITY_ANDROID
        setStatusBarValue(1024); // WindowManager.LayoutParams.FLAG_FULLSCREEN; change this to 0 if unsatisfied
#endif
    } 

    private static void setStatusBarValue(int value)
    {
        newStatusBarValue = value;
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                if (activity != null){
                    activity.Call("runOnUiThread", new AndroidJavaRunnable(setStatusBarValueInThread));
                }
            }
        }
    }

    private static void setStatusBarValueInThread()
    {
#if UNITY_ANDROID
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                using (var window = activity.Call<AndroidJavaObject>("getWindow"))
                {
                    if (window != null) {
                        window.Call("setFlags", newStatusBarValue, newStatusBarValue);
                    }
                }
            }
        }
#endif
    }
}

