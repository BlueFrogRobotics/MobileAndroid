﻿using UnityEngine;
using System;

class LocalNotification
{
    /// <summary>
    /// Inexact uses `set` method
    /// Exact uses `setExact` method
    /// ExactAndAllowWhileIdle uses `setAndAllowWhileIdle` method
    /// Documentation: https://developer.android.com/intl/ru/reference/android/app/AlarmManager.html
    /// </summary>
    public enum NotificationExecuteMode
    {
        Inexact = 0,
        Exact = 1,
        ExactAndAllowWhileIdle = 2
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    private static string fullClassName = "net.agasper.unitynotification.UnityNotificationManager";
    private static string mainActivityClassName = "com.bfr.main.MainActivity";
#endif

    public static void SendNotification(int iId, TimeSpan iDelay, string iTitle, string iMessage)
    {
        SendNotification(iId, (int)iDelay.TotalSeconds, iTitle, iMessage, Color.white);
    }
    
    public static void SendNotification(int id, long delay, string title, string message, Color32 bgColor,
        bool sound = true, bool vibrate = false, bool lights = true, string bigIcon = "notify_buddy",
        NotificationExecuteMode executeMode = NotificationExecuteMode.Exact)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass pluginClass = new AndroidJavaClass(fullClassName);
        if (pluginClass != null)
        {
            pluginClass.CallStatic("SetNotification", id, delay * 1000L, title, message, message, sound ? 1 : 0, vibrate ? 1 : 0, lights ? 1 : 0, bigIcon, "notify_buddy", bgColor.r * 65536 + bgColor.g * 256 + bgColor.b, (int)executeMode, mainActivityClassName);
        }
#endif
    }

    public static void SendRepeatingNotification(int id, long delay, long timeout, string title, string message,
        Color32 bgColor, bool sound = true, bool vibrate = true, bool lights = true, string bigIcon = "")
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass pluginClass = new AndroidJavaClass(fullClassName);
        if (pluginClass != null)
        {
            pluginClass.CallStatic("SetRepeatingNotification", id, delay * 1000L, title, message, message, timeout * 1000, sound ? 1 : 0, vibrate ? 1 : 0, lights ? 1 : 0, bigIcon, "notify_buddy", bgColor.r * 65536 + bgColor.g * 256 + bgColor.b, mainActivityClassName);
        }
#endif
    }

    public static void CancelNotification(int id)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass pluginClass = new AndroidJavaClass(fullClassName);
        if (pluginClass != null) {
            pluginClass.CallStatic("CancelNotification", id);
        }
#endif
    }
}
