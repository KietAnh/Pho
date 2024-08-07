using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;

[RequireComponent(typeof(ScrollRect))]
public class LoopScrollView : MonoBehaviour       // refactor: Loop object
{
    private ScrollRect _scrollRect;
    private GameObject _itemPrefab;
    private Transform _content;
    private int _itemCount;

    public Action<LoopViewItem, int> OnItemShowHandler;

    private void Awake()
    {
        _scrollRect = GetComponent<ScrollRect>();
        _itemPrefab = transform.Find("Viewport/Pool").GetChild(0).gameObject;
        _content = transform.Find("Viewport/Content");
    }
    public void Init(int count, Type itemType)
    {
        _itemCount = count;
        for (int i = 0; i < _itemCount; i++)
        {
            var itemObj = Instantiate(_itemPrefab, _content);
            var item = Activator.CreateInstance(itemType) as LoopViewItem;
            item.trans = itemObj.transform;
            item.Init();
            itemObj.SetActive(true);
            OnItemShowHandler(item, i);
        }
    }
}
