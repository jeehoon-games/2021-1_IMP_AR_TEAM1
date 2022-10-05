using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;

public class AndroidToast : MonoBehaviour
{
    
#if UNITY_ANDROID

    #region Instance variables & Init Method
    
    public static AndroidToast instance;

    private AndroidJavaObject _currActivity;
    private AndroidJavaClass _unityPlayer;
    private AndroidJavaObject _context;
    private AndroidJavaObject _toast;

    private void Init()
    {
        
    }
    
    #endregion


    #region Unity event functions

    private void Awake()
    {
        if (instance == null) { instance = this; }
        else { Destroy(gameObject); }

        _unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        _currActivity = _unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        _context = _currActivity.Call<AndroidJavaObject>("getApplicationContext");
        DontDestroyOnLoad(gameObject);
    }

    #endregion

    #region Toast Message methods

    public void ShowToast(string msg)
    {
        _currActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
        {
            AndroidJavaClass Toast = new AndroidJavaClass("android.widget.Toast");
            AndroidJavaObject jString = new AndroidJavaObject("java.lang.String", msg);

            _toast = Toast.CallStatic<AndroidJavaObject>("makeText", _context, jString,
                Toast.GetStatic<int>("LENGTH_SHORT"));
            _toast.Call("show");
        }));
    }

    public void CloseToast()
    {
        _currActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
        {
            if (_toast != null)
            {
                _toast.Call("cancel");
            }
        }));
    }

    #endregion
    
#endif
}
