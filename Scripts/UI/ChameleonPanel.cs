using Hexagonal;
using UnityEngine;
using Cards;
using Combination;

public class ChameleonPanel : MonoBehaviour {
   
    [SerializeField]
    private GameObject panel;
    [SerializeField]
    private GameObject chirwa, pika, bubna, cross;      

    private HexagonalCell currentCell;  

    public void SetNewChirwa()
    {
        panel.SetActive(false);
        currentCell.InitCardInCell((int)eCards.Chirwa, currentCell.GetCard().Info.myNumCard,eSpecialCard.none);
        ManagerCombination.Instance.ClearMatchCells();
    }
    public void SetNewPika()
    {
        panel.SetActive(false);
        currentCell.InitCardInCell((int)eCards.Pika, currentCell.GetCard().Info.myNumCard, eSpecialCard.none);
        ManagerCombination.Instance.ClearMatchCells();
    }
    public void SetNewBubna()
    {
        panel.SetActive(false);
        currentCell.InitCardInCell((int)eCards.Bubna, currentCell.GetCard().Info.myNumCard, eSpecialCard.none);
        ManagerCombination.Instance.ClearMatchCells();
    }
    public void SetNewCross()
    {
        panel.SetActive(false);
        currentCell.InitCardInCell((int)eCards.Cross, currentCell.GetCard().Info.myNumCard, eSpecialCard.none);
        ManagerCombination.Instance.ClearMatchCells();
    }

    public void ActivatePanel(HexagonalCell _cell)
    {
        currentCell = _cell;
        panel.SetActive(true);
        eCards color = currentCell.GetCard().Info.myColor;
        bubna.SetActive(true);
        chirwa.SetActive(true);
        cross.SetActive(true);
        pika.SetActive(true);
        if (color == eCards.Bubna)        
            bubna.SetActive(false);
        if (color == eCards.Chirwa)
            chirwa.SetActive(false);
        if (color == eCards.Cross)
            cross.SetActive(false);
        if (color == eCards.Pika)
            pika.SetActive(false);
    }
}
