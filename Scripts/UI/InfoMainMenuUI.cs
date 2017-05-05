using UnityEngine;
using UnityEngine.UI;

public class InfoMainMenuUI : MonoBehaviour {

    public Text textGold;
    public Text textChips;

    // Use this for initialization
    void Start ()
    {
        SetTextGold();
        SetTextChips();
    }

    public void SetTextChips()
    {
        textChips.text = AppGame.Instance.Info.Chips.ToString() + " chips";
    }

    public void SetTextGold()
    {
        textGold.text = AppGame.Instance.Info.Gold.ToString() + " gold";
    }
}
