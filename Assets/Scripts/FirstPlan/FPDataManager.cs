using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPDataManager : MonoBehaviour
{
    public static FPDataManager Instance { get; private set; }

    readonly Dictionary<int, EnemyConfig> enemyConfigs = new Dictionary<int, EnemyConfig>();
    readonly Dictionary<int, FPBattleSkillConfig> skillConfigs = new Dictionary<int, FPBattleSkillConfig>();
    readonly Dictionary<int, FPItemConfig> itemConfigs = new Dictionary<int, FPItemConfig>();

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
        skillConfigs.Add(1, new FPBattleSkillConfig("疾風突き", 1, 0));
        skillConfigs.Add(2, new FPBattleSkillConfig("ガードスルー", 2, 0));
        skillConfigs.Add(10, new FPBattleSkillConfig("ブロック", 10, 1));
    }

    public FPBattleSkillConfig GetSkillConfig(int num)
    {
        skillConfigs.TryGetValue(num, out FPBattleSkillConfig skillConfig);
        return skillConfig;
    }

    public void LoadItemConfigs()
    {
        itemConfigs.Add(0, new FPItemConfig("きずぐすり", 0, 0, 10));
        itemConfigs.Add(10, new FPItemConfig("いのちの玉", 10, 1, 10));
        itemConfigs.Add(20, new FPItemConfig("木のけん", 20, 2, 3));
        itemConfigs.Add(30, new FPItemConfig("木のたて", 30, 3, 3));
        itemConfigs.Add(40, new FPItemConfig("木のくつ", 40, 4, 3));
    }

    public FPItemConfig GetItemConfig(int num)
    {
        itemConfigs.TryGetValue(num, out FPItemConfig skillConfig);
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

public class FPItemConfig
{
    // ItemKind
    // 0 ... 回復
    // 1 ... ステ強化HP
    // 2 ... ステ強化OF
    // 3 ... ステ強化DF
    // 4 ... ステ強化SP
    public string Name { get; protected set; }
    public int ItemID { get; protected set; }
    public int ItemKind { get; protected set; }
    public int Amount { get; protected set; }

    public FPItemConfig(string name, int itemID, int itemKind, int amount)
    {
        Name = name;
        ItemID = itemID;
        ItemKind = itemKind;
        Amount = amount;
    }
}
