using UnityEngine;
using System.Collections.Generic;
namespace Cards
{
    public class PollCards : MonoBehaviour
    {
        public static PollCards Instance;

        private List<CardData> cards = new List<CardData>();
        private CardData reserveCard;

        void Awake()
        {
            if (Instance == null)
                Instance = this;
            CreatePackCards();
        }

        public CardData GetNewCard()
        {             
            CardData _card = new CardData();
            if (cards.Count <= 1)
            {
                CreatePackCards();
            }
            int _random = Random.Range(0, cards.Count);
            _card = cards[_random];
            cards.RemoveAt(_random);
            return _card;
        }

        public CardData GetReserveCard()
        {
            return reserveCard;
        }

        public void SetReserveCard(eCards _color,int _num,eSpecialCard _special)
        {
            reserveCard = new CardData();
            reserveCard.myColor = _color;
            reserveCard.myNumCard = _num;
            reserveCard.mySpecialCard = _special;
        }

        private void CreatePackCards() 
        {
            for (int i = GlobalInfo.startPoolPackCard; i <= GlobalInfo.maxPoolPackCard; i++)
            {
                for (int y = 1; y <= GlobalInfo.colorsCards; y++)
                {
                    CardData _card = new CardData();
                    _card.myNumCard = i;
                    _card.myColor = (eCards)y;
                    _card.mySpecialCard = eSpecialCard.none;
                    cards.Add(_card);
                }
            }
        }


    }
}
