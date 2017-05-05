using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cards;
using Hexagonal;

public class TurnBack : MonoBehaviour {


    public List<LoadData> SaveInfo = new List<LoadData>();

    public ActivCardMoving activCard;
    public ReplacementCard replacementCard;
    public BonusSlot BonusSlot1;
    public BonusSlot BonusSlot2;
    public HudUI score;  


    public void AddNewTurnInfo()
    {
        LoadData newTurn = new LoadData();
        HexagonalCell[] cells = HexagonalGrid.Instance.GetAllCells();
        int lenght = cells.Length;
        newTurn.Cards = new CardData[lenght];
        for (int i = 0; i < lenght; i++)
        {
            CardData newCard = new CardData();
            newCard.myNumCard = cells[i].GetCard().Info.myNumCard;
            newCard.myColor = cells[i].GetCard().Info.myColor;
            newCard.mySpecialCard = cells[i].GetCard().Info.mySpecialCard;
            newCard.isClosed = cells[i].GetCard().Info.isClosed;
            newTurn.Cards[i] = newCard;
        }
       
        CardData _acticCardCard = new CardData();
        _acticCardCard.myNumCard = activCard.myCard.Info.myNumCard;
        _acticCardCard.myColor = activCard.myCard.Info.myColor;
        _acticCardCard.mySpecialCard = activCard.myCard.Info.mySpecialCard;
        newTurn.ActicCard = _acticCardCard;

        newTurn.Score = score.GetScore();
        newTurn.Turn = score.GetTurn();
        newTurn.untilClosed = ManagerClosedCard.Instance.currentProgress;


        //  newTurn.BonusSlot1 = BonusSlot1.GetMyCard().Info;
        //  newTurn.BonusSlot2 = BonusSlot2.GetMyCard().Info;

        SaveInfo.Add(newTurn);
    }
    //invoke from buttton scene
    public void SetTurnBack()
    {
        if (SaveInfo.Count == 0)
            return;
       
        HexagonalCell[] cells = HexagonalGrid.Instance.GetAllCells();
        int lenght = cells.Length;
        int count = SaveInfo.Count-2;
        for (int i = 0; i < lenght; i++)
        {
            CardData card = SaveInfo[count].Cards[i];           
            cells[i].InitCardInCellWithCheckClosed((int)card.myColor, card.myNumCard, card.mySpecialCard, card.isClosed);
        }

        CardData _activCard = SaveInfo[count].ActicCard;
        activCard.InitCard((int)_activCard.myColor, _activCard.myNumCard, _activCard.mySpecialCard);

        score.SetScore(SaveInfo[count].Score);
        score.SetTurn(SaveInfo[count].Turn);
        ManagerClosedCard.Instance.currentProgress = SaveInfo[count].untilClosed;
        score.SetTextClosedCard(SaveInfo[count].untilClosed);

        SaveInfo.RemoveAt(SaveInfo.Count -1);
        Debug.Log("turn back");
    }


}
