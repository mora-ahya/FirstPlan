using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CharacterStatus
{
    public int MaxHp;
    public int Hp;

    public int Offense;
    public int Defense;
    public int Speed;
}

public enum StatusKindEnum
{
    MaxHp,
    Hp,
    Offense,
    Defense,
    Speed,
    Name,
    Max,
}

public struct Command
{
    public int OwnerID;
    public int TargetID;
    public int Kind;
    public int SkillID;
}

public enum FPCharacterCondition
{
    Block,
    GuardThrough,
    Charge,
}

public class FPBattleCharacter : MonoBehaviour, ITurnBasedBattlerBase, IUpdateableTextsHandler
{
    protected static FPBattleManager BattleManager;

    public static void SetBattleManager(FPBattleManager bm)
    {
        BattleManager = bm;
    }

    public int CharacterID { get; protected set; }
    public bool IsOutOfBattle => IsRunningAway || IsDead;
    public bool IsRunningAway { get; protected set; }
    public bool IsDead => status.Hp <= 0;
    public int Offense => status.Offense;
    public int Defense => status.Defense;

    public readonly CharacterConditions conditions = new CharacterConditions();

    protected CharacterStatus status;

    protected Command command;

    protected int changedStatusFlag = 0;

    public virtual void Damaged(int amount)
    {
        status.Hp -= amount;
        status.Hp = Mathf.Max(status.Hp, 0);
        BattleManager.ReportCharacterDamaged(this, amount);
        changedStatusFlag |= 1 << (int)StatusKindEnum.Hp;
    }

    public virtual void Healed(int amount)
    {
        status.Hp += amount;
        status.Hp = Mathf.Min(status.Hp, status.MaxHp);
        //BattleManager.ReportCharacterDamaged(this, amount);
        changedStatusFlag |= 1 << (int)StatusKindEnum.Hp;
    }

    public int CalculateOffenseDamage(int amount)
    {
        if (conditions.HasCondition((int)FPCharacterCondition.Charge))
        {
            amount *= 3;
            conditions.RemoveCondition((int)FPCharacterCondition.Charge);
        }

        return amount;
    }

    public int CalculateDefenseDamage(FPBattleCharacter attacker, int amount)
    {
        if (conditions.HasCondition((int)FPCharacterCondition.Block))
        {
            amount = (int)(amount * 0.25f);
        }

        if (conditions.HasCondition((int)FPCharacterCondition.GuardThrough))
        {
            conditions.RemoveCondition((int)FPCharacterCondition.GuardThrough);
        }
        else
        {
            amount = Mathf.Max(amount - status.Defense, 1);
        }

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
                BattleManager.DealDamageToCharacter(this, CalculateOffenseDamage(Offense));
            }
            else if (command.Kind == 1)
            {
                OnUseSkill();
                FPBattleSkillConfig skillConfig = FPDataManager.Instance.GetSkillConfig(command.SkillID);
                BattleManager.SkillManager.GetBattleSkill(skillConfig.SkillKind)?.DoProcess(skillConfig.SkillID, this);
            }
        }

        IsEndBattleProcess = true;
    }

    protected virtual void OnUseSkill()
    {
        
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
