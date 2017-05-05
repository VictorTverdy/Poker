using UnityEngine;

namespace Cards
{
    public class ReplacementCard : MonoBehaviour
    {
        [SerializeField]
        private Card myCard;
        [SerializeField]
        private ActivCardMoving activCard;

        public bool canSwap;
        private bool canReplacement;
        private bool isActiv;

        private void Start()
        {
            if (LoadManager.Instance.needLoad)
            {
                var save = JsonUtility.FromJson<CardData>(PlayerPrefs.GetString("ReplacementCard"));
                InitCard(save.myColor, save.myNumCard, save.mySpecialCard);
            }
        }

        void OnDisable()
        {
            string jsonSave = JsonUtility.ToJson(myCard.Info);
            PlayerPrefs.SetString("ReplacementCard", jsonSave);
        }

        public void ActivateImageBack(bool isActivate)
        {
            isActiv = isActivate;
            myCard.ActivateImageBack(isActivate);
        }

        public void AddCardReplacement()
        {
            if (!canSwap) 
            {
                if (activCard.canSwap && isActiv)
                {
                    AddCard();
                }
            }      
            else
            {
                if (canReplacement)
                    SwapCard();

            }
        }

        public void AddCard()
        {
            if (!canReplacement) {
                InitCard(activCard.myCard.Info.myColor, activCard.myCard.Info.myNumCard, activCard.myCard.Info.mySpecialCard);  
                activCard.CreateNewCard();
            }else
            {
                CardData swapCard = new CardData();
                swapCard.myColor = myCard.Info.myColor;
                swapCard.myNumCard = myCard.Info.myNumCard;
                swapCard.mySpecialCard = myCard.Info.mySpecialCard;

                myCard.Info.myColor = activCard.myCard.Info.myColor;
                myCard.Info.myNumCard = activCard.myCard.Info.myNumCard;
                myCard.Info.mySpecialCard = activCard.myCard.Info.mySpecialCard;
                myCard.InitCard((int)myCard.Info.myColor, myCard.Info.myNumCard, myCard.Info.mySpecialCard);
                myCard.ActivateImageBack(false);

                activCard.InitCard((int)swapCard.myColor, swapCard.myNumCard, swapCard.mySpecialCard);
                activCard.myCard.ActivateImageBack(false);
            }
        }

        private void InitCard(eCards _color,int _num,eSpecialCard _spec)
        {
            canReplacement = true;
            canSwap = true;
            myCard.Info.myColor = _color;
            myCard.Info.myNumCard = _num;
            myCard.Info.mySpecialCard = _spec;
            myCard.InitCard((int)myCard.Info.myColor, myCard.Info.myNumCard, myCard.Info.mySpecialCard);
            myCard.ActivateImageBack(false);
            activCard.myCard.ActivateImageBack(false);
        }

        private void SwapCard()
        {
            canReplacement = false;
            canSwap = false;
            activCard.SetReserveCardToPoll();
            activCard.InitCard((int)myCard.Info.myColor, myCard.Info.myNumCard, myCard.Info.mySpecialCard);
            myCard.ClearFromCell();         
        }

    }
}
