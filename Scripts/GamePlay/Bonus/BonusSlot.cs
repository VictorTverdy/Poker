using UnityEngine;
using UnityEngine.UI;
using Cards;
using System;
public class BonusSlot : MonoBehaviour {   

    private Button myButton;
    private eSpecialCard myBonus;
    private bool isFull;
    private Card myCard;
    private ActivCardMoving activCard;
    // Use this for initialization
    void Start()
    {
        activCard = FindObjectOfType<ActivCardMoving>();
        myCard = transform.GetComponentInChildren<Card>();
        myButton = GetComponent<Button>();
        myButton.onClick.AddListener(ActivateBonus);
        if (LoadManager.Instance.needLoad)
        {
            string saveBonus = PlayerPrefs.GetString(name);
            if (!string.IsNullOrEmpty(saveBonus))
            {
                eSpecialCard saveSpecialCard = (eSpecialCard)Enum.Parse(typeof(eSpecialCard), saveBonus);
                InitSlot(saveSpecialCard);
            }
        }
    }

    void OnDisable()
    {
        if (isFull)
          PlayerPrefs.SetString(name, myBonus.ToString());
        else
            PlayerPrefs.SetString(name, "");
    }

    public eSpecialCard GetBonus() 
    {
        return myBonus;
    }

    public void SetisFull()
    {
         isFull = true;
    }

    public bool GetisFull()
    {
        return isFull;
    }

    public void SwapSlot(eSpecialCard _bonus)
    {
        myBonus = _bonus;
        isFull = true;
        myCard.InitBonusInfo(myBonus);
    }

    public void ActivateBonus()
    {
        if (isFull)
        {
            BonusManager.Instance.ActivateBonus(GetComponent<BonusSlot>());
        }       
    }

    public void ClearSlot()
    {
        BonusManager.Instance.ClearCurrentActivSlot();
        activCard.SetEnableImageCard(true);
        myCard.AnimatorDesable();
        ActivateBackImageCard(false);
        isFull = false;
        myBonus = eSpecialCard.none;
        myCard.ClearFromCell();
    } 

    public void ActivateBackImageCard(bool _enable)
    {
        myCard.ActivateImageBack(_enable);
    } 
    
    public Card GetMyCard()
    {
        return myCard;
    }  

    public void InitSlot(eSpecialCard _bonus)
    {       
        /*
        if (!isFull)
        {           
            myBonus = _bonus;
            isFull = true;
        }
        else
        {
            if (_bonus.ToString().Contains("upend") || myBonus.ToString().Contains("bullet") || myBonus.ToString().Contains("fireworks") || myBonus.ToString().Contains("wind"))
            {
                int curentLevel = (int)myBonus;
                if (curentLevel >= (int)_bonus)
                {
                    curentLevel++;
                    myBonus = (eSpecialCard)curentLevel;
                }
                else
                {
                    myBonus = _bonus;
                }             
            }
            else
            {
                myBonus = _bonus;
            }          
        }
        */
        myCard.AnimatorDesable();
        ActivateBackImageCard(false);
        myBonus = _bonus;
        isFull = true;
        myCard.InitBonusInfo(myBonus);
    }
}
