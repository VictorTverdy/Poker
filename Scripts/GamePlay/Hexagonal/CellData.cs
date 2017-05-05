using System;
[Serializable]
public struct CellData
{
    public bool IsFull;
    public int myX;
    public int myY;
}

[Serializable]
public struct CellsInfo
{
    public CellData[] Cells;
}
