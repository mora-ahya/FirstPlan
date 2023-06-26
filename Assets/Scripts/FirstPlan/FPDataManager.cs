using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPDataManager : MonoBehaviour
{
    public static FPDataManager Instance { get; private set; }

    readonly Dictionary<int, EnemyConfig> enemyConfigs = new Dictionary<int, EnemyConfig>();
    readonly Dictionary<int, FPBattleSkillConfig> skillConfigs = new Dictionary<int, FPBattleSkillConfig>();

    void Awake()
    {
        Instance = this;
        LoadEnemyConfigs();
        LoadSkillConfigs();
    }

    public void LoadEnemyConfigs()
    {
        enemyConfigs.Add(0, new EnemyConfig("からかさおばけ", 10, 4, 1, 1, 0, 0, 0, 1, 0, 0, 0));
    }

    public EnemyConfig GetEnemyConfig(int num)
    {
        enemyConfigs.TryGetValue(num, out EnemyConfig enemyConfig);
        return enemyConfig;
    }

    public void LoadSkillConfigs()
    {
        skillConfigs.Add(0, new FPBattleSkillConfig("パワーアタック", 0, 0));
    }

    public FPBattleSkillConfig GetSkillConfig(int num)
    {
        skillConfigs.TryGetValue(num, out FPBattleSkillConfig skillConfig);
        return skillConfig;
    }
}

public class EnemyConfig
{
    public Sprite EnemySprite { get; protected set; }
    public string Name { get; protected set; }
    public int HP { get; protected set; }
    public int Attack { get; protected set; }
    public int Defence { get; protected set; }
    public int Speed { get; protected set; }
    public int Skill1 { get; protected set; }
    public int Skill2 { get; protected set; }
    public int Skill3 { get; protected set; }
    public int DropAttackPoint { get; protected set; }
    public int DropDefencePoint { get; protected set; }
    public int DropSpeedPoint { get; protected set; }
    public int DropSkill { get; protected set; }

    public EnemyConfig(string name, int hp, int attack, int defence, int speed, int skill1, int skill2, int skill3, int dropAPoint, int dropDPoint, int dropSPoint, int dropSkill)
    {
        Name = name;
        HP = hp;
        Attack = attack;
        Defence = defence;
        Speed = speed;
        Skill1 = skill1;
        Skill2 = skill2;
        Skill3 = skill3;
        DropAttackPoint = dropAPoint;
        DropDefencePoint = dropDPoint;
        DropSpeedPoint = dropSPoint;
        DropSkill = dropSkill;
    }
}

public class FPBattleSkillConfig
{
    public string Name { get; protected set; }
    public int SkillID { get; protected set; }
    public int SkillKind { get; protected set; }

    public FPBattleSkillConfig(string name, int skillID, int skillKind)
    {
        Name = name;
        SkillID = skillID;
        SkillKind = skillKind;
    }
}
