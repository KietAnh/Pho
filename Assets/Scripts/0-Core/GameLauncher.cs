using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class GameLauncher : MonoBehaviour
//{
//    private void Awake()
//    {
//        gameObject.AddComponent<CoroutineManager>();

//        WindowManager.Singleton.Awake();

//        UpdateResource();

//        Screen.sleepTimeout = SleepTimeout.NeverSleep;
//        Application.targetFrameRate = 60;//Screen.currentResolution.refreshRate;
//    }
//    private void UpdateResource()
//    {
//        StartupManager.Singleton.Start((param) => LaunchGame(param));
//    }
//    private void LaunchGame(AppInfo appInfo)     // entry point of the application
//    {
//        StartupManager.DestroySelf();
//        Debug.Log("KIET LOG >> Launch Game !!!");
//        // ILruntime process: temp skip
//        //
//        gameObject.AddComponent<GameManager>().Init(appInfo);
//#if !UNITY_EDITOR
//        //TrackingManager.PushEvent(EVENT_TRACK.CmpLoading);
//#endif
//    }
//}
