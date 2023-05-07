using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITurnBasedBattlerBase
{

}

public class TurnBasedBattleManager : MonoBehaviour, IManagerBase
{
    List<ITurnBasedBattlerBase> turnBasedBattlers = new List<ITurnBasedBattlerBase>();
    public int ActPriority { get; } = 0;
    public void AwakeInitialize()
    {

    }

    public void LateAwakeInitialize()
    {

    }

    public void Act()
    {

    }

    public void AddTurnBasedBattler(ITurnBasedBattlerBase turnBasedBattler)
    {
        turnBasedBattlers.Add(turnBasedBattler);
    }
}
