using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameManager gameManager;
    public TMP_InputField timeScaleInputField;
    public TextMeshProUGUI textMoney;

    private void Awake()
    {
        GED.ED.addListener(EventID.OnEarnMoney, OnEarnMoney_UpdateUI);
    }

    private void OnDestroy()
    {
        GED.ED.removeListener(EventID.OnEarnMoney, OnEarnMoney_UpdateUI);
    }

    private void Start()
    {
        timeScaleInputField.text = 1.ToString();
    }

    public void OnTimeScaleInputFieldChange()
    {
        Time.timeScale = float.Parse(timeScaleInputField.text);
    }

    #region  event

    private void OnEarnMoney_UpdateUI(GameEvent gameEvent)
    {
        textMoney.text = ServiceManager.Singleton.GetService<MainService>().money.ToString();
    }

    #endregion
}
