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
    readonly FPGameScene gameScene;

    public FPBattleEvent(FPGameScene scene)
    {
        gameScene = scene;
    }

    public void OnHappeningEvent(IGameEventConfig config)
    {
        FPBattleEventConfig eventConfig = config as FPBattleEventConfig;

        gameScene.StartBattleEvent(eventConfig);
    }
}

public class FPItemUseEvent : IGameEvent
{
    readonly FPBattlePlayer fpBattlePlayer;

    public FPItemUseEvent(FPBattlePlayer player)
    {
        fpBattlePlayer = player;
    }

    public void OnHappeningEvent(IGameEventConfig config)
    {
        FPItemUseEventConfig eventConfig = config as FPItemUseEventConfig;

        FPItemConfig itemConfig = FPDataManager.Instance.GetItemConfig(eventConfig.ItemID);

        switch (itemConfig.ItemKind)
        {
            case 0: // 回復
                fpBattlePlayer.Healed(itemConfig.Amount);
                break;

            case 1: // ステ強化HP
                fpBattlePlayer.GrowUpStatus(StatusKindEnum.MaxHp, itemConfig.Amount);
                break;

            case 2: // ステ強化OF
                fpBattlePlayer.GrowUpStatus(StatusKindEnum.Offense, itemConfig.Amount);
                break;

            case 3: // ステ強化DF
                fpBattlePlayer.GrowUpStatus(StatusKindEnum.Defense, itemConfig.Amount);
                break;

            case 4: // ステ強化SP
                fpBattlePlayer.GrowUpStatus(StatusKindEnum.Speed, itemConfig.Amount);
                break;
        }
    }
}

