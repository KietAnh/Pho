using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupManager : SingletonBehaviour<PopupManager>
{
    [SerializeField] private GameObject _popupCheat;




    public void OpenPopup(PopupType type)
    {
        GameObject popupGO = null;
        switch (type)
        {
            case PopupType.Cheat:
                popupGO = _popupCheat;
                break;
        }

        popupGO.SetActive(true);
    }

    public void ClosePopup(PopupType type)
    {
        GameObject popupGO = null;
        switch (type)
        {
            case PopupType.Cheat:
                popupGO = _popupCheat;
                break;
        }

        popupGO.SetActive(false);
    }
}

public enum PopupType
{
    Cheat,
}
