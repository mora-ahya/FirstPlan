using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

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
    readonly Dictionary<int, int> skillLinks = new Dictionary<int, int>();

    public void Initialize()
    {
        status = new CharacterStatus();
        status.MaxHp = 30;
        status.Hp = 30;
        status.Offense = 3;
        status.Defense = 3;
        status.Speed = 3;
        changedStatusFlag = (1 << (int)StatusKindEnum.Max) - 1;

        GainSkill(0);
        GainSkill(0);
        GainSkill(2);
        GainSkill(10);
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

    protected override void OnUseSkill()
    {
        int skillIndex = skillLinks.GetValueOrDefault(command.SkillID);

        PlayerSkill skill = skills[skillIndex];
        skill.UsableTimes--;
        skills[skillIndex] = skill;
    }

    void GainSkill(int skillId)
    {
        if (skillLinks.TryGetValue(skillId, out int skillIndex))
        {
            PlayerSkill skill = skills[skillIndex];
            skill.UsableTimes++;
            skills[skillIndex] = skill;
        }
        else
        {
            skills.Add(new PlayerSkill(skillId, 1));
            skillLinks.Add(skillId, skills.Count - 1);
        }
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
        switch (StatusKindEnum.MaxHp + num)
        {
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

    public void GrowUpStatus(StatusKindEnum statusKindEnum, int amount)
    {
        switch (statusKindEnum)
        {
            case StatusKindEnum.MaxHp:
                status.MaxHp += amount;
                status.Hp += amount;
                break;
            case StatusKindEnum.Offense:
                status.Offense += amount;
                break;
            case StatusKindEnum.Defense:
                status.Defense += amount;
                break;
            case StatusKindEnum.Speed:
                status.Speed += amount;
                break;
        }
        changedStatusFlag |= 1 << (int)statusKindEnum;
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
