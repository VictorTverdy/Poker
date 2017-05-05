using UnityEngine;
using UnityEngine.UI;

public class StoreBonus : MonoBehaviour
{
    public Text textGold;
    public Text textChips;

    public Button gold200, gold500, gold1000, gold5000;

    // Use this for initialization
    void Start ()
    {
        SetTextGold();
        SetTextChips();
        gold200.onClick.AddListener(BuyGold200);
        gold500.onClick.AddListener(BuyGold500);
        gold1000.onClick.AddListener(BuyGold1000);
        gold5000.onClick.AddListener(BuyGold5000);
    }

    // invoke from button 
    public void BuyBonus(GameObject _name)
    {
        int costBonus = 5;
        if (AppGame.Instance.Info.Gold >= costBonus && BonusManager.Instance.GetIsFreeBonusSlot())
        {
            SoundManager.Instance.PlayGold();
            int _bonus = int.Parse(_name.name); 
            BonusManager.Instance.BuyBonus(_bonus);
            AppGame.Instance.Info.Gold -= costBonus;
            SetTextGold();
        }
    }

    public void BuyGold200()
    {
        if (AppGame.Instance.Info.Chips >= 10)
        {
            SoundManager.Instance.PlayMoney();
            int _gold = 200;
            AppGame.Instance.Info.Gold += _gold;
            AppGame.Instance.Info.Chips -= 10;
            SetTextChips();
            SetTextGold();
        }
    }

    public void BuyGold500()
    {
        if (AppGame.Instance.Info.Chips >= 25)
        {
            SoundManager.Instance.PlayMoney();
            int _gold = 500;
            AppGame.Instance.Info.Gold += _gold;
            AppGame.Instance.Info.Chips -= 25;
            SetTextChips();
            SetTextGold();
        }
    }

    public void BuyGold1000()
    {
        if (AppGame.Instance.Info.Chips >= 50)
        {
            SoundManager.Instance.PlayMoney();
            int _gold = 1000;
            AppGame.Instance.Info.Gold += _gold;
            AppGame.Instance.Info.Chips -= 50;
            SetTextChips();
            SetTextGold();
        }
    }

    public void BuyGold5000()
    {
        if (AppGame.Instance.Info.Chips >=100)
        {
            SoundManager.Instance.PlayMoney();
            int _gold = 5000;
            AppGame.Instance.Info.Gold += _gold;
            AppGame.Instance.Info.Chips -= 100;
            SetTextChips();
            SetTextGold();
        }
    }

    public void BuyChips(GameObject _name)
    {
        SoundManager.Instance.PlayMoney();
        int _chips = int.Parse(_name.name);
        AppGame.Instance.Info.Chips += _chips; 
        SetTextChips();
    }

    private void SetTextChips()
    {
        textChips.text = AppGame.Instance.Info.Chips.ToString() + " chips";
    }

    private void SetTextGold()
    {
        textGold.text = AppGame.Instance.Info.Gold.ToString() + " gold";
    }

   
}
