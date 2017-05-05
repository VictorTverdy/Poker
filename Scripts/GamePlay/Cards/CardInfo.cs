using System;
using System.Collections.Generic;

namespace Cards
{
    [Serializable]
    public class CardData
    {
        public eCards myColor;
        public eSpecialCard mySpecialCard; 
        public int myNumCard;
        public int myX;
        public int myY;
        public bool  isClosed;
    }

    [Serializable]
    public class CardsInfo
    {
        public List<CardsInfo> DataRecord = new List<CardsInfo>();
    }
}
