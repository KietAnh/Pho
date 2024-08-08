using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayUIAudio : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private string _audioName = "ui_click";
    [SerializeField] [Range(0f, 1f)] private float volume = 1f;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (AudioManager.Singleton.isInit)
            AudioManager.Singleton.PlayUIAudio(_audioName, volume);
    }
}
