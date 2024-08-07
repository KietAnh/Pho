/*
 * file SingletonBehaviour.cs
 *
 * author: Pengmian
 * date:   2014/09/16 
 */

using JetBrains.Annotations;
using UnityEngine;


/// <summary>
/// 单件模版 
/// 所有派生的单件对象都需要挂到GameObject上
/// </summary>
/// <typeparam name="T"></typeparam>
public class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Singleton;

    protected virtual void Awake()
    {
        Debug.Log("Awake Manager: " + gameObject.name);
        Singleton = this as T;
        DontDestroyOnLoad(gameObject);  // KIET ADD 220323
    }

    protected virtual void OnDestroy()
    {
        Singleton = null;
    }
}
