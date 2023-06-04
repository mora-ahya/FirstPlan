using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FPBattlePlayer : FPBattleCharacter, ITurnBasedPlotterBase, ITurnBasedBattlerBase
{
    int maxHP;

    public void Initialize()
    {
        status = new CharacterStatus();
        maxHP = 30;
        status.Hp = 30;
        status.Offense = 3;
        status.Defense = 3;
        status.Speed = 3;
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
        // ç°âÒÇÕämíËÇ≈ê¨å˜Ç≥ÇπÇÈ
        // àÍï‡â∫Ç™ÇÈ
    }

    public string GetSkillInfo(int num)
    {
        return "ãZ" + num;
    }


    #region ITurnBasedPlotterBase
    public bool IsAlreadyDecided { get; protected set; }
    public void OnStartPlotting()
    {
        IsAlreadyDecided = false;
        FPBattleManager.Instance.StartPlotting();
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
