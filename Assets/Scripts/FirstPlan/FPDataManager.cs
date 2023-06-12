using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPDataManager : MonoBehaviour
{
    public static FPDataManager Instance { get; private set; }

    readonly Dictionary<int, EnemyConfig> enemyConfigs = new Dictionary<int, EnemyConfig>();

    void Awake()
    {
        Instance = this;
    }

    public void LoadEnemyConfigs()
    {
        enemyConfigs.Add(0, new EnemyConfig("‚©‚ç‚©‚³‚¨‚Î‚¯", 10, 4, 1, 1));
    }

    public EnemyConfig GetEnemyConfig(int num)
    {
        enemyConfigs.TryGetValue(num, out EnemyConfig enemyConfig);
        return enemyConfig;
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

    public EnemyConfig(string name, int hp, int attack, int defence, int speed)
    {
        Name = name;
        HP = hp;
        Attack = attack;
        Defence = defence;
        Speed = speed;
    }
}
