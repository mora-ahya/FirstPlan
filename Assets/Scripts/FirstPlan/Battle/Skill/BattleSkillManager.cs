using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 複数ターゲットはIDをbit演算にして指定したい
// チョコランのときのスキル仕様を参考にしたい
public interface IBattleSkill<BattleSkillUserT>
{
    public int BattleSkillKind { get; }

    public void DoPreProcess(int skillID, BattleSkillUserT skillUserT);
    public void DoProcess(int skillID, BattleSkillUserT skillUserT);
    public void DoPostProcess(int skillID, BattleSkillUserT skillUserT);
}

public class BattleSkillManager<BattleSkillT>
{
    //public static BattleSkillManager<BattleSkillT> Instance { get; protected set; }

    //public static void Initialize()
    //{
    //    if (Instance == null)
    //    {
    //        Instance = new BattleSkillManager<BattleSkillT>();
    //        Instance.OnInitialize();
    //    }
    //}

    //public static void Deinitialize()
    //{
    //    if (Instance != null)
    //    {
    //        Instance.OnDeinitialize();
    //        Instance.battleSkills.Clear();
    //        Instance = null;
    //    }
    //}

    protected readonly Dictionary<int, BattleSkillT> battleSkills = new Dictionary<int, BattleSkillT>();

    protected void RegistBattleSkill(int num, BattleSkillT skill)
    {
        battleSkills.Add(num, skill);
    }

    public BattleSkillT GetBattleSkill(int num)
    {
        battleSkills.TryGetValue(num, out BattleSkillT skill);
        return skill;
    }
}
