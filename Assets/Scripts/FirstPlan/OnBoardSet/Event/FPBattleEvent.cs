using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FPGameEventKind
{
    Battle,
    Item,
    TreasureChest,
}

public class FPBattleEvent : IGameEvent
{
    FPGameScene gameScene;

    public FPBattleEvent(FPGameScene scene)
    {
        gameScene = scene;
    }

    public void OnHappeningEvent(IGameEventConfig config)
    {
        FPBattleEventConfig eventConfig = config as FPBattleEventConfig;

        if (eventConfig == null)
        {
            return;
        }

        gameScene.StartBattleEvent(eventConfig);
    }
}
