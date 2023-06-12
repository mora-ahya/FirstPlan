using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EventKind
{
    Battle,
    Item,
    TreasureChest,
}

public class FPBoardEvent : IGameEvent
{
    EventKind eventKind;

    public void OnHappeningEvent(IGameEventConfig config)
    {
        switch (eventKind)
        {
            case EventKind.Battle:
                OnHappeningBattleEvent(config);
                break;
        }
    }

    void OnHappeningBattleEvent(IGameEventConfig config)
    {
        FPBattleEventConfig eventConfig = config as FPBattleEventConfig;

        if (eventConfig == null)
        {
            return;
        }

        FPBattleManager.Instance.StartBattle(eventConfig.EnemyID);
    }

}
