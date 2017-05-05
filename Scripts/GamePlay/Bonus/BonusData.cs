using System;
[Serializable]
public struct BonusData
{
    public string ComboName;
    public string[] MySpecial;
    public int[] ChangeDrop;	
}
[Serializable]
public class BonusInfo
{
    public BonusData[] Info;
}

