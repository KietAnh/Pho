using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WWWLoader : SingletonTemplate<WWWLoader>  // update option: use cache
{
    private int _nowCorNum = 0;
    public int MaxCorNum = 20;
    public int TimeOut = 30;
    private Dictionary<string, LoadInfo> _loadingMap = new Dictionary<string, LoadInfo>();
    private List<string> _loadingList = new List<string>();
    public void Download(string path, Action<string, bool, byte[]> onComplete, Action<string, float, ulong> onProgress)
    {
        var info = new LoadInfo();
        info.cmpCb = onComplete;
        info.updateCb = onProgress;
        _loadingMap.Add(path, info);
        _loadingList.Add(path);
        if (_nowCorNum < MaxCorNum)
        {
            CoroutineManager.Singleton.startCoroutine(LoadQueue());
        }
    }
    private IEnumerator LoadQueue()
    {
        _nowCorNum += 1;
        float time = 0;
        while (true)
        {
            if (_loadingList.Count > 0)
            {
                time = 0;
                string path = _loadingList[0];
                _loadingList.RemoveAt(0);
                yield return Loadwww(path);
                yield return null; // wait a frame between download times
            }
            else
            {
                time += Time.deltaTime;
                if (time < 30)
                {
                    yield return null;
                }
                else
                {
                    _nowCorNum -= 1;
                    break;
                }
            }
        }
    }
    private IEnumerator Loadwww(string path)
    {
        if (!_loadingMap.ContainsKey(path))
            yield break;
        var cmpCb = _loadingMap[path].cmpCb;
        var updateCb = _loadingMap[path].updateCb;

        using (UnityWebRequest request = UnityWebRequest.Get(path))
        {
            request.timeout = TimeOut;
            request.SendWebRequest();
            while (true)
            {
                if (!string.IsNullOrEmpty(request.error))
                {
                    Debug.Log("KIET LOG >> UnityWebRequest Error >> " + request.error);
                    _loadingMap.Remove(path);
                    if (cmpCb != null)
                    {
                        cmpCb(path, false, null);
                    }
                    break;
                }
                //if (updateCb != null)
                //{
                //    updateCb(path, request.downloadProgress, 0);
                //}
                if (request.isDone)
                {
                    if (_loadingMap.ContainsKey(path))
                        _loadingMap.Remove(path);
                    if (cmpCb != null)
                    {
                        cmpCb(path, true, request.downloadHandler.data);
                    }
                    yield return null;
                    break;
                }
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}

public class LoadInfo
{
    public Action<string, bool, byte[]> cmpCb;
    public Action<string, float, ulong> updateCb;
}
