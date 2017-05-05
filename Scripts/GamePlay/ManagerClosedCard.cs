using System.Collections.Generic;
using UnityEngine;
using Hexagonal;
using Cards;
using Combination;

public class ManagerClosedCard : MonoBehaviour {

    public static ManagerClosedCard Instance;

    public int currentProgress = 0;

    private int startClosed = 2;   
    private HudUI hudUI;
    // Use this for initialization
    void Start ()
    {
        if (Instance == null)
            Instance = this;
        hudUI = FindObjectOfType<HudUI>();
        if (LoadManager.Instance.needLoad)
            currentProgress = LoadManager.Instance.SaveInfo.untilClosed;
        hudUI.SetTextClosedCard(currentProgress);
    }  

    public void UpCurrentProgress()
    {
        if (ManagerCombination.Instance.GetLenghtAllCombination() > 0)
        {
            currentProgress++;            
            if (currentProgress >= GlobalInfo.periodicityCreateClosed)
            {
                CreateClosedCardNearFull();
            }
            int until = GlobalInfo.periodicityCreateClosed - currentProgress;
            hudUI.SetTextClosedCard(until);
        }
    }

    public void NullCurrentProgress()
    {
        currentProgress = 0;
        int until = GlobalInfo.periodicityCreateClosed - currentProgress;
        hudUI.SetTextClosedCard(until);
    }

    public void CreateStartClosedCard()
    {
        List<int> _randomList = new List<int>();
        for (int i=0;i <= startClosed;i++)
        {
            int _random = Random.Range(0,GlobalInfo.row);
            if (_randomList.Contains(_random)) 
            {
                i--;
            }else
            {
                if(i == startClosed && Random.Range(0,100) < 50)
                {
                    return;
                }
                _randomList.Add(_random);
                HexagonalGrid.Instance.GetCell(_random, 0).GetCard().SetIsClosed(true);
            }           
        }
    }

    private void CreateClosedCardNearFull()
    { 
        int lenght = ManagerCombination.Instance.GetLenghtAllCombination();
        for (int i=0;i< lenght;i++)
        {
            HexagonalCell _cell = HexagonalGrid.Instance.GetRandomEmptyCellNearFull();
            if (_cell != null)
            {
                CardData _card = PollCards.Instance.GetNewCard();
                _cell.InitCardInCell((int)_card.myColor, _card.myNumCard, _card.mySpecialCard);
                _cell.GetCard().SetIsClosed(true);
            }
        }     
        currentProgress = 0;
    }
	
}
