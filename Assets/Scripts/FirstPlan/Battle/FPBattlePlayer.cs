using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// statusUI�\���͎b��I�ȍ쐬�A����Ή�
public class FPBattlePlayer : FPBattleCharacter, ITurnBasedPlotterBase, ITurnBasedBattlerBase
{
    public enum StatusKind
    {
        MaxHp,
        Hp,
        Attack,
        Defense,
        Speed,
        Max,
    }

    int maxHP;

    public void Initialize()
    {
        status = new CharacterStatus();
        maxHP = 30;
        status.Hp = 30;
        status.Offense = 3;
        status.Defense = 3;
        status.Speed = 3;
        changedStatusFlag = (1 << (int)StatusKind.Max) - 1;
    }

    public void SetCommand(Command playerCommand)
    {
        command = playerCommand;
    }

    public void DecideCommand()
    {
        IsAlreadyDecided = true;
    }

    void OnPressRunAway()
    {
        // ����͊m��Ő���������
        // ���������
    }

    public override void OnDamage(int amount)
    {
        base.OnDamage(amount);
        changedStatusFlag |= 1 << (int)StatusKind.Hp;
    }

    public string GetSkillInfo(int num)
    {
        return "�Z" + num;
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
            case StatusKind.MaxHp:
                return maxHP.ToString();
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

    #region ITurnBasedPlotterBase
    public bool IsAlreadyDecided { get; protected set; }
    public void OnStartPlotting()
    {
        IsAlreadyDecided = false;
        BattleManager.StartPlotting();
    }

    public void OnEndPlotting()
    {

    }
    #endregion


    #region ITurnBasedBattlerBase Player
    public bool IsEndBattleProcess { get; protected set; }
    public int BattlePriority { get; protected set; }

    public void OnStartTurn()
    {
        IsEndBattleProcess = false;

        BattlePriority = status.Speed;

        if (command.Kind == 2)
        {
            BattlePriority += 1024;
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
