using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

//public class StartupManager : SingletonTemplate<StartupManager>
//{
//    public const string KEY_APP_VERSION = "htak_AppVersion";   // refactor: lưu trong một file json đặt ở local (LocalConfig.json)
//    //public const string APP_FILE_URL = "https://s3.ap-southeast-1.amazonaws.com/htak.tripletile";       // refactor: lưu trong một file json đặt ở local (LocalConfig.json)
//    public const string APP_INFO_PATH = "/app_info.json";
//    public const string APP_URL = "https://play.google.com/store/apps/details?id=com.htak.matchtiles";
//    private Action<AppInfo> _callback;
//    private AppInfo _appInfo;
//    private Dictionary<string, float> _loadingSizeMap = new Dictionary<string, float>();
//    private Dictionary<string, bool> _loadedFileMap = new Dictionary<string, bool>();
//    private Dictionary<string, FileEntry> _abFileMap = new Dictionary<string, FileEntry>();
//    private float _loadedSize = 0;
//    private float _totalSize = 0;

//    private bool _isRealLoadDone = false;
//    private bool _isFakeLoadDone = false;
//    public void Start(Action<AppInfo> callback)
//    {
//        CoroutineManager.Singleton.startCoroutine(CheckUpdate());
//        _callback = callback;
//    }
//    //    private IEnumerator CheckUpdate()
//    //    {
//    //        Debug.Log("KIET LOG >> Check update...!!!");
//    //        int localVersion = PlayerPrefs.GetInt(KEY_APP_VERSION, 0);

//    //        yield return GetRemoteVersion();

//    //#if UNITY_EDITOR
//    //        if (!AssetLoadManager.SimulateAssetBundleInEditor)
//    //        {
//    //            yield return new WaitForSeconds(0.5f);
//    //            LoadingManager.Singleton.CloseLoading();
//    //            _callback(_appInfo);
//    //            yield break;
//    //        }
//    //#endif
//    //        if (_appInfo.appVersion > localVersion)
//    //        {
//    //            if (_appInfo.isUpdateStore)
//    //            {
//    //                // go To Store to update
//    //            }
//    //            else
//    //            {
//    //                // get assetbundle info list
//    //                GetAbFileMap();
//    //                // update resource in game (download all assetbundles to local disk)
//    //                foreach (var abUrl in _abFileMap.Keys)
//    //                {
//    //                    WWWLoader.Singleton.Download(abUrl, OnFileLoaded, OnLoadProgress);
//    //                }
//    //            }
//    //        }
//    //        else
//    //        {
//    //            _callback(_appInfo);
//    //        }
//    //    }
//    private IEnumerator CheckUpdate()
//    {
//        yield return null;
//        Debug.Log("KIET LOG >> Check update...!!!");

//        GetLocalVersion();

//        //RemoteConfigManager.Singleton.SetDefaults();
//        //Task fetchTask = RemoteConfigManager.Singleton.FetchDataAsync();
//        //while (!fetchTask.IsCompleted)
//        //{
//        //    yield return null;
//        //}

//        long appVersion = 0;// RemoteConfigManager.Singleton.GetLongValue("app_version");
//        long localVersion = long.Parse(Application.version.Replace(".", string.Empty));
//        Debug.Log("kiet log >> appversion: " + appVersion + " >> local version: " + localVersion);
//        if (localVersion < appVersion)
//        {
//            // go to store
//            ShowUpdateRequest();
//        }
//        else
//        {
//            LoadResources();
//        }
//    }
//    private void LoadResources()
//    {
//        CoroutineManager.Singleton.startCoroutine(ILoadResources());
//    }
//    IEnumerator ILoadResources()
//    {
//#if UNITY_EDITOR
//        if (!AssetLoadManager.SimulateAssetBundleInEditor)
//        {
//            yield return new WaitForSeconds(0.5f);
//            //LoadingManager.Singleton.CloseLoading();
//            _callback(_appInfo);
//            yield break;
//        }
//#endif
//        // get assetbundle info list
//        GetAbFileMap();
//        // update resource in game (download all assetbundles to local disk)
//        foreach (var abUrl in _abFileMap.Keys)
//        {
//            WWWLoader.Singleton.Download(abUrl, OnFileLoaded, OnLoadProgress);
//        }

//        yield return null;
//        yield return LoadingManager.Singleton.FakeLoad(3f);

//        _isFakeLoadDone = true;
//        if (_isRealLoadDone)
//        {
//            //LoadingManager.Singleton.CloseLoading();
//            _callback(_appInfo);
//        }
//    }
    

//    private void ShowUpdateRequest()
//    {
//        //string content = "A new version has arrived. Update now?";
//        //WindowManager.Singleton.OpenWindow<ConfirmWindow>(null, new WinInfo(content, new List<UnityAction>() { GoToStore, LoadResources}));
//    }
//    private void GoToStore()
//    {
//        Application.OpenURL(APP_URL);
//#if UNITY_EDITOR
//        UnityEditor.EditorApplication.isPlaying = false;
//#else
//        Application.Quit();
//#endif
//    }

//    //private IEnumerator GetRemoteVersion()       // refactor: chuyển sang WWWLoader
//    //{
//    //    using (UnityWebRequest www = UnityWebRequest.Get(APP_FILE_URL + APP_INFO_PATH))
//    //    {
//    //        // Send the request and wait for a response
//    //        float startTime = Time.time;
//    //        yield return www.SendWebRequest();
//    //        Debug.Log("KIET LOG >> Time send request: " + (Time.time - startTime));

//    //        // Check for errors
//    //        if (www.result != UnityWebRequest.Result.Success)
//    //        {
//    //            Debug.Log("KIET LOG >> UnityWebRequest Error: >> " + www.error);
//    //        }
//    //        else
//    //        {
//    //            string jsonContent = www.downloadHandler.text;
//    //            Debug.Log("KIET LOG >> Download content: >> " + jsonContent);
//    //            _appInfo = JsonUtility.FromJson<AppInfo>(jsonContent);
//    //        }
//    //    }
//    //}
//    private void GetLocalVersion()
//    {
//        string path = APP_INFO_PATH.Remove(0, 1);
//        path = path.Remove(path.IndexOf('.'));
//        string jsonContent = Resources.Load<TextAsset>(path).text;
//        Debug.Log("KIET LOG >> Download content: >> " + jsonContent);
//        _appInfo = JsonUtility.FromJson<AppInfo>(jsonContent);
//    }
//    //private void GetAbFileMap()  
//    //{
//    //    string abFilePath = string.Format(APP_FILE_URL + _appInfo.assetBundleUrl, PathUtil.PlatformName);
//    //    foreach (var fileEntry in _appInfo.abFiles)
//    //    {
//    //        string filePath = abFilePath + "/" + fileEntry.resName;
//    //        _abFileMap.Add(filePath, fileEntry);
//    //        _totalSize += fileEntry.size;
//    //    }
//    //}
//    private void GetAbFileMap()
//    {
//        string abFilePath = string.Format(Application.streamingAssetsPath + _appInfo.assetBundleUrl, PathUtil.PlatformName);
//        foreach (var fileEntry in _appInfo.abFiles)
//        {
//            string filePath = abFilePath + "/" + fileEntry.resName;
//            _abFileMap.Add(filePath, fileEntry);
//            _totalSize += fileEntry.size;
//        }
//    }
//    private void OnFileLoaded(string path, bool isSuccess, byte[] data)
//    {
//        if (_loadingSizeMap.ContainsKey(path))
//        {
//            _loadingSizeMap.Remove(path);
//        }
//        string name = _abFileMap[path].resName;
//        if (isSuccess && data != null)
//        {
//            // lưu ab vào cục bộ
//            string mPath = PathUtil.GetAssetBundleDiskPath(name);
//            Debug.Log("kiet try >> " + mPath);
//            try
//            {
//                var dir = Path.GetDirectoryName(mPath);
//                if (!Directory.Exists(dir))
//                    Directory.CreateDirectory(dir);
//                if (File.Exists(mPath))
//                    File.Delete(mPath);
//                File.WriteAllBytes(mPath, data);
//            }
//            catch (Exception e)
//            {
//                Debug.LogError("KIET LOG >> Lỗi khi ghi tệp ForceFileList >> " + e);
//                return;
//            }
//            _loadedSize += _abFileMap[path].size;
//            _loadedFileMap[path] = true;
//            TryComplete();
//        }
//        else
//        {
//            Debug.LogError("KIET LOG >> File load fail !!! >> " + path);
//        }
//    }
//    private void OnLoadProgress(string path, float progress, ulong loadedBytes)
//    {
//        float loadingLoadedSize = 0;
//        if (_abFileMap.ContainsKey(path))
//        {
//            _loadingSizeMap[path] = _abFileMap[path].size * progress;
//            foreach (var size in _loadingSizeMap.Values)
//                loadingLoadedSize += size;
//        }

//        LoadingManager.Singleton.OnProgress((_loadedSize + loadingLoadedSize) / _totalSize);
//    }

//    private void TryComplete()
//    {
//        bool isloadComplete = true;
//        if (_loadedFileMap.Keys.Count == _abFileMap.Keys.Count)
//        {
//            for (int i = 0; i < _loadedFileMap.Keys.Count; i++) 
//            {
//                string path = _loadedFileMap.Keys.ToArray()[i];
//                if (_loadedFileMap[path] == false)
//                {
//                    isloadComplete = false;
//                    break;
//                }
//            }
//        }
//        else
//        {
//            isloadComplete= false;
//        }
//        if (isloadComplete)
//        {
//            Debug.Log("KIET LOG >> real loading complete >> " + Time.realtimeSinceStartup);

//            //LoadingManager.Singleton.CloseLoading();
//            //_callback(_appInfo);
//            _isRealLoadDone = true;
//            if (_isFakeLoadDone)
//            {
//                //LoadingManager.Singleton.CloseLoading();
//                _callback(_appInfo);
//            }
//        }
//    }
//}

//[Serializable]
//public class AppInfo
//{
//    public int appVersion;
//    public bool isUpdateStore;
//    public string assetBundleUrl;
//    public List<FileEntry> abFiles;
//    public List<string> managers;
//}

//[Serializable]
//public class FileEntry
//{
//    //public string folder;
//    public string resName;
//    //public int version;
//    //public int priority;

//    public long size;
//    //public string md5;
//}
