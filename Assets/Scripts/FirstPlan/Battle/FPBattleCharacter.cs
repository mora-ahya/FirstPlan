using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CharacterStatus
{
    public int Hp;

    public int Offense;
    public int Defense;
    public int Speed;
}

public struct Command
{
    public int OwnerID;
    public int TargetID;
    public int Kind;
    public int SkillID;
}

public class FPBattleCharacter : MonoBehaviour, ITurnBasedBattlerBase, IUpdateableTextsHandler
{
    protected static FPBattleManager BattleManager;

    public static void SetBattleManager(FPBattleManager bm)
    {
        BattleManager = bm;
    }

    public enum StatusKind
    {
        MaxHp,
        Hp,
        Offense,
        Defense,
        Speed,
        Name,
        Max,
    }

    public int CharacterID { get; protected set; }
    public bool IsOutOfBattle => IsRunningAway || IsDead;
    public bool IsRunningAway { get; protected set; }
    public bool IsDead => status.Hp <= 0;
    public int Offense => status.Offense;
    public int Defense => status.Defense;

    protected CharacterStatus status;

    protected Command command;

    protected int changedStatusFlag = 0;

    public virtual void Damaged(int amount)
    {
        status.Hp -= amount;
        status.Hp = Mathf.Max(status.Hp, 0);
        BattleManager.ReportCharacterDamaged(this, amount);
        changedStatusFlag |= 1 << (int)StatusKind.Hp;
    }

    public int CalculateDamage(FPBattleCharacter attacker, int amount)
    {
        amount = Mathf.Max(amount - status.Defense, 1);

        return amount;
    }

    public Command GetCommand()
    {
        return command;
    }

    #region ITurnBasedBattlerBase
    public bool IsEndBattleProcess { get; protected set; }
    public int BattlePriority { get; set; }

    public virtual void OnStartTurn()
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

        if (IsOutOfBattle == false)
        {
            BattleManager.ReportCharacterStartTurn(this);
            if (command.Kind == 0)
            {
                BattleManager.DealDamageToCharacter(this, Offense);
            }
            else if (command.Kind == 1)
            {
                FPBattleSkillConfig skillConfig = FPDataManager.Instance.GetSkillConfig(command.SkillID);
                BattleManager.SkillManager.GetBattleSkill(skillConfig.SkillKind)?.DoProcess(skillConfig.SkillID, this);
            }
        }

        IsEndBattleProcess = true;
    }

    public void OnEndTurn()
    {
        IsEndBattleProcess = false;
        IsEndBattleProcess = true;
    }
    #endregion

    #region Implement IUpdateableTextsHandler
    public int UpdateTextFlag => changedStatusFlag;

    public virtual string GetUpdateableTextString(int num, GameObject gObject)
    {
        return null;
    }
    #endregion
}
