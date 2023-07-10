using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFPBattleSkill : IBattleSkill<FPBattleCharacter>
{
    /// IBattleSkill<BattleSkillUserT>
    //public int BattleSkillKind { get; }

    //public void DoPreProcess(int skillID, BattleSkillUserT skillUserT);
    //public void DoProcess(int skillID, BattleSkillUserT skillUserT);
    //public void DoPostProcess(int skillID, BattleSkillUserT skillUserT);
}

public enum FPBattleSkillKind
{
    Attack,
    Enhanced,
    Heal,
}

public class FPBattleSkillManager : BattleSkillManager<IFPBattleSkill>
{
    public void Initialize(FPBattleManager battleManager)
    {
        RegistBattleSkill((int)FPBattleSkillKind.Attack, new AttackSkill(battleManager));
    }
}

public class AttackSkill : IFPBattleSkill
{
    public int BattleSkillKind { get; } = (int)FPBattleSkillKind.Attack;

    readonly FPBattleManager battleManager;

    public AttackSkill(FPBattleManager bm)
    {
        battleManager = bm;
    }

    // スキルID 00 パワーアタック 攻撃力の1.3倍のダメージ
    // スキルID 01 疾風突き 攻撃力の0.75倍のダメージ 必ず先制
    // スキルID 02 ガードスルー 攻撃力の1.0倍ダメージ 防御無視
    // スキルID 03 無心攻撃 攻撃力の1.0倍ダメージ 集中力を消費しない

    public void DoPreProcess(int skillID, FPBattleCharacter skillUserT)
    {
        if (skillID == 1)
        {
            skillUserT.BattlePriority += 1024;
        }
    }

    public void DoProcess(int skillID, FPBattleCharacter skillUserT)
    {
        float skillRate = 1.0f;

        switch (skillID)
        {
            case 0:
                skillRate = 1.3f;
                break;
            case 1:
                skillRate = 0.75f;
                break;
            case 2:
                battleManager.DealConditionToCharacter(skillUserT, (int)FPCharacterCondition.GuardThrough);
                break;
        }

        int damagePoint = Mathf.CeilToInt(skillUserT.CalculateOffenseDamage(skillUserT.Offense) * skillRate);

        battleManager.ReportCharacterUseSkill(skillUserT, skillID);
        battleManager.DealDamageToCharacter(skillUserT, damagePoint);
    }

    public void DoPostProcess(int skillID, FPBattleCharacter skillUserT)
    {

    }
}

public class EnhancedSkill : IFPBattleSkill
{
    public int BattleSkillKind { get; } = (int)FPBattleSkillKind.Enhanced;

    readonly FPBattleManager battleManager;

    public EnhancedSkill(FPBattleManager bm)
    {
        battleManager = bm;
    }

    // スキルID 10 ブロック 被ダメージを75%軽減
    // スキルID 11 メタル討伐の証 全ステータスが上昇する(永続)

    public void DoPreProcess(int skillID, FPBattleCharacter skillUserT)
    {
        switch (skillID)
        {
            case 10:
                skillUserT.conditions.AddCondition((int)FPCharacterCondition.Block);
                break;
        }
    }

    public void DoProcess(int skillID, FPBattleCharacter skillUserT)
    {
        battleManager.ReportCharacterUseSkill(skillUserT, skillID);
    }

    public void DoPostProcess(int skillID, FPBattleCharacter skillUserT)
    {
        switch (skillID)
        {
            case 10:
                skillUserT.conditions.RemoveCondition((int)FPCharacterCondition.Block);
                break;
        }
    }
}

public class HealSkill : IFPBattleSkill
{
    public int BattleSkillKind { get; } = (int)FPBattleSkillKind.Heal;

    // スキルID 20 自己修復 最大HPの50%回復
    // スキルID 21 精神統一 集中力を回復

    public void DoPreProcess(int skillID, FPBattleCharacter skillUserT)
    {

    }

    public void DoProcess(int skillID, FPBattleCharacter skillUserT)
    {

    }

    public void DoPostProcess(int skillID, FPBattleCharacter skillUserT)
    {

    }
}


