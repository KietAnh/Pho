
using TMPro;
using UnityEngine;

public class PopupCheat : BasePopup
{
    [SerializeField] private TMP_InputField _cmd;

    public void CloseOnlick()
    {
        gameObject.SetActive(false);
    }

    public void OkOnclick()
    {
        string[] paramList = _cmd.text.Split(' ');
        if (paramList.Length > 0)
        {
            switch (paramList[0])
            {
                case "money":
                    CheatMoney(paramList);
                    break;
                default:
                    break;
            }
        }
    }

    public void CheatMoney(string[] paramList)
    {
        if (paramList.Length >= 2)
        {
            int amount = int.Parse(paramList[1]);
            GameController.EarnMoney(amount);
        }
        else
        {
            Debug.LogError("kiet log >> Cmd is invalid");
        }
    }
}
