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
public class FPBattleEnemy : FPBattleCharacter, ITurnBasedBattlerBase
{
    public enum StatusKind
    {
        Name,
        Hp,
        Attack,
        Defense,
        Speed,
        Max,
    }

    public string Name => enemyConfig == null ? string.Empty : enemyConfig.Name;

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

    public override string GetUpdateableTextString(int num, GameObject gObject)
    {
        changedStatusFlag &= ~(1 << num);
        return GetStatusString(num);
    }

    string GetStatusString(int num)
    {
        switch (StatusKind.Name + num)
        {
            case StatusKind.Name:
                return Name;
            case StatusKind.Hp:
                return status.Hp.ToString();
            case StatusKind.Attack:
                return status.Offense.ToString();
            case StatusKind.Defense:
                return status.Defense.ToString();
            case StatusKind.Speed:
                return status.Speed.ToString();
            default:
                return string.Empty;
        }
    }

    public override void OnDamage(int amount)
    {
        base.OnDamage(amount);
        changedStatusFlag |= 1 << (int)StatusKind.Hp;
    }

    #region ITurnBasedBattlerBase Enemy
    public bool IsEndBattleProcess { get; protected set; }
    public int BattlePriority { get; private set; }

    public void OnStartTurn()
    {
        IsEndBattleProcess = false;

        if (IsDead == false)
        {
            command = new Command();
            command.OwnerID = 1;
            command.Kind = 0;
        }

        IsEndBattleProcess = true;
    }

    public void OnStartProcessingCommand()
    {
        IsEndBattleProcess = false;

        if (IsDead == false)
        {
            BattleManager.ApplyCommand(command);
        }
        
        IsEndBattleProcess = true;
    }

    public void OnEndTurn()
    {
        IsEndBattleProcess = false;
        IsEndBattleProcess = true;
    }
    #endregion
}
