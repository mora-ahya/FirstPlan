using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyConfig
{
    Sprite enemySprite;

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

    public EnemyConfig(int hp, int attack, int defence, int speed)
    {
        HP = hp;
        Attack = attack;
        Defence = defence;
        Speed = speed;
    }
}
