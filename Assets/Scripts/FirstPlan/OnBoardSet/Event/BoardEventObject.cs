using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BoardEventObjectInfo
{
    public int SquareNum { get; private set; }
    public IReadOnlyList<IGameEventConfig> BoardEventConfigs { get; private set; }

    

    public void Initialize(int squareNum, IReadOnlyList<IGameEventConfig> boardEventConfigs)
    {
        SquareNum = squareNum;
        BoardEventConfigs = boardEventConfigs;
    }

    public void Copy(ref BoardEventObjectInfo other)
    {
        this.SquareNum = other.SquareNum;
        this.BoardEventConfigs = other.BoardEventConfigs;
    }
}
