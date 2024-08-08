using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameManager gameManager;
    public TMP_InputField timeScaleInputField;
    public TextMeshProUGUI textMoney;

    private void Start()
    {
        timeScaleInputField.text = 1.ToString();
    }

    public void OnTimeScaleInputFieldChange()
    {
        Time.timeScale = float.Parse(timeScaleInputField.text);
    }

    public void UpdateMoney()
    {
        textMoney.text = gameManager.money.ToString();
    }
}
