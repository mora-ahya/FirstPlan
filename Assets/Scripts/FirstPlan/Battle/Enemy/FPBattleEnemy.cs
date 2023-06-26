using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public interface IEnemyOnUI : IPointerClickHandler
{
    public int ID { get; }
    public Sprite Image { get; }
}

public struct EnemyStatus
{
    public int Hp;

    public int Offense;
    public int Defense;
    public int Speed;
}

// statusUI•\Ž¦‚ÍŽb’è“I‚Èì¬AŒã’ö‘Î‰ž
public class FPBattleEnemy : FPBattleCharacter
{
    public string Name => enemyConfig == null ? string.Empty : enemyConfig.Name;

    public int MaxHp => enemyConfig == null ? 0 : enemyConfig.HP;

    EnemyConfig enemyConfig;

    [SerializeField] Image image;

    public void SetConfig(EnemyConfig config)
    {
        enemyConfig = config;

        status = new CharacterStatus();
        status.Hp = config.HP;
        status.Offense = config.Attack;
        status.Defense = config.Defence;
        status.Speed = config.Speed;
        changedStatusFlag = (1 << (int)StatusKind.Max) - 1;
    }

    public EnemyConfig GetConfig()
    {
        return enemyConfig;
    }

    public override string GetUpdateableTextString(int num, GameObject gObject)
    {
        changedStatusFlag &= ~(1 << num);
        return GetStatusString(num);
    }

    string GetStatusString(int num)
    {
        switch (StatusKind.MaxHp + num)
        {
            case StatusKind.Name:
                return Name;
            case StatusKind.MaxHp:
                return MaxHp.ToString();
            case StatusKind.Hp:
                return status.Hp.ToString();
            case StatusKind.Offense:
                return status.Offense.ToString();
            case StatusKind.Defense:
                return status.Defense.ToString();
            case StatusKind.Speed:
                return status.Speed.ToString();
            default:
                return string.Empty;
        }
    }

    public override void Damaged(int amount)
    {
        base.Damaged(amount);
    }
}
