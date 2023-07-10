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

    // �X�L��ID 00 �p���[�A�^�b�N �U���͂�1.3�{�̃_���[�W
    // �X�L��ID 01 �����˂� �U���͂�0.75�{�̃_���[�W �K���搧
    // �X�L��ID 02 �K�[�h�X���[ �U���͂�1.0�{�_���[�W �h�䖳��
    // �X�L��ID 03 ���S�U�� �U���͂�1.0�{�_���[�W �W���͂�����Ȃ�

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

    // �X�L��ID 10 �u���b�N ��_���[�W��75%�y��
    // �X�L��ID 11 ���^�������̏� �S�X�e�[�^�X���㏸����(�i��)

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

    // �X�L��ID 20 ���ȏC�� �ő�HP��50%��
    // �X�L��ID 21 ���_���� �W���͂���

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


