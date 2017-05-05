using System;
[Serializable]
public struct RecordData
{
    public int score;	
    public string namePlayer;
}

[Serializable]
public struct RecordInfo
{
    public RecordData[] Info;
}
