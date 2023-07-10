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

// statusUI•\Ž¦‚ÍŽb’è“I‚Èì¬AŒã’ö‘Î‰ž
public class FPBattleEnemy : FPBattleCharacter
{
    public string Name => enemyConfig == null ? string.Empty : enemyConfig.Name;

    EnemyConfig enemyConfig;

    [SerializeField] Image image;

    public void SetConfig(EnemyConfig config)
    {
        enemyConfig = config;

        status = new CharacterStatus();
        status.MaxHp = config.HP;
        status.Hp = config.HP;
        status.Offense = config.Attack;
        status.Defense = config.Defence;
        status.Speed = config.Speed;
        changedStatusFlag = (1 << (int)StatusKindEnum.Max) - 1;
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
        switch (StatusKindEnum.MaxHp + num)
        {
            case StatusKindEnum.Name:
                return Name;
            case StatusKindEnum.MaxHp:
                return status.MaxHp.ToString();
            case StatusKindEnum.Hp:
                return status.Hp.ToString();
            case StatusKindEnum.Offense:
                return status.Offense.ToString();
            case StatusKindEnum.Defense:
                return status.Defense.ToString();
            case StatusKindEnum.Speed:
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
