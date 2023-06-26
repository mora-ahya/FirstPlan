using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// statusUI表示は暫定的な作成、後程対応
public class FPBattlePlayer : FPBattleCharacter, ITurnBasedPlotterBase
{
    struct PlayerSkill
    {
        public int SkillID;
        public int UsableTimes;

        public PlayerSkill(int skillID, int count)
        {
            SkillID = skillID;
            UsableTimes = count;
        }
    }

    readonly List<PlayerSkill> skills = new List<PlayerSkill>();

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

        skills.Add(new PlayerSkill(0, 5));
    }

    public void SetCommand(Command playerCommand)
    {
        command = playerCommand;
    }

    public void DecideCommand()
    {
        IsAlreadyDecided = true;
    }

    public void DecideSkill(int skillNum)
    {
        Command command = new Command();
        command.OwnerID = 0;
        command.Kind = 1;
        command.TargetID = 1;
        command.SkillID = skills[skillNum].SkillID;

        SetCommand(command);
    }

    void OnPressRunAway()
    {
        // 今回は確定で成功させる
        // 一歩下がる
    }

    public override void Damaged(int amount)
    {
        base.Damaged(amount);
        
    }

    public int GetHavingSkillNum()
    {
        return skills.Count;
    }

    public string GetSkillName(int num)
    {
        if (skills.Count < num)
        {
            return null;
        }

        int skillNum = skills[num].SkillID;

        return FPDataManager.Instance.GetSkillConfig(skillNum).Name;
    }

    public int GetSkillID(int num)
    {
        if (skills.Count < num)
        {
            return -1;
        }

        return skills[num].SkillID;
    }

    public int GetSkillUsableTimes(int num)
    {
        if (skills.Count < num)
        {
            return -1;
        }

        return skills[num].UsableTimes;
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

    public void GrowUpStatus(int aPoint, int dPoint, int sPoint)
    {
        status.Offense += aPoint;
        status.Defense += dPoint;
        status.Speed += sPoint;

        changedStatusFlag |= 1 << (int)StatusKind.Offense;
        changedStatusFlag |= 1 << (int)StatusKind.Defense;
        changedStatusFlag |= 1 << (int)StatusKind.Speed;
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
    public override void OnStartTurn()
    {
        IsEndBattleProcess = false;

        BattlePriority = status.Speed;

        if (command.Kind == 2)
        {
            BattlePriority += 2048;
        }

        IsEndBattleProcess = true;
    }
    #endregion
}
