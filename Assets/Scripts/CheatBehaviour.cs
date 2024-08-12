using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheatBehaviour : MonoBehaviour
{
    private int _numClick = 0;
    private float _lastTimeClick = 0;
    private const int MAX_NUM_CLICK = 7;

    private void Update()
    {
        if (_numClick > 0)
        {
            if (Time.time - _lastTimeClick > 0.5f)
            {
                _numClick = 0;
            }
        }
    }

    public void OnClickCheat()
    {
        _numClick += 1;
        _lastTimeClick = Time.time;
        if (_numClick >= MAX_NUM_CLICK)
        {
            PopupManager.Singleton.OpenPopup(PopupType.Cheat);
            _numClick = 0;
        }
    }
}
