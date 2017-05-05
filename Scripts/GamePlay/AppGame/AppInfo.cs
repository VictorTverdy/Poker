using System;
using System.Collections.Generic;
using Cards;

[Serializable]
public struct AppInfo
{
    public int Gold;
    public int Chips;
    public int Keys;
    public List<eSpecialCard> Bonus;
}
