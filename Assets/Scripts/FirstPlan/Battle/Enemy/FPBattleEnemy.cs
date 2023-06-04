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

public class FPBattleEnemy : FPBattleCharacter, ITurnBasedBattlerBase
{

    EnemyConfig enemyConfig;

    [SerializeField]Image image;

    public void SetConfig(EnemyConfig config)
    {
        enemyConfig = config;

        status = new CharacterStatus();
        status.Hp = config.HP;
        status.Offense = config.Attack;
        status.Defense = config.Defence;
        status.Speed = config.Speed;
    }

    #region ITurnBasedBattlerBase Enemy
    public bool IsEndBattleProcess { get; protected set; }
    public int BattlePriority { get; private set; }

    public void OnStartTurn()
    {
        IsEndBattleProcess = false;

        command = new Command();
        command.OwnerID = 1;
        command.Kind = 0;

        IsEndBattleProcess = true;
    }

    public void OnStartProcessingCommand()
    {
        IsEndBattleProcess = false;
        FPBattleManager.Instance.ApplyCommand(command);
        IsEndBattleProcess = true;
    }

    public void OnEndTurn()
    {
        IsEndBattleProcess = false;
        IsEndBattleProcess = true;
    }
    #endregion
}
