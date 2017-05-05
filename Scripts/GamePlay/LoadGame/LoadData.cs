using Cards;
using System;

[Serializable]
public struct LoadData
{
    public CardData[] Cards;
    public int Score;
    public int Turn;
    public int untilClosed;
    public CardData ActicCard;
    public CardData BonusSlot1;
    public CardData BonusSlot2;
    public CardData ReplacementCard;
}
