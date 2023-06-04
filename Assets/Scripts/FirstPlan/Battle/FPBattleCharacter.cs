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
    public int SkillNum;
}

public class FPBattleCharacter : MonoBehaviour
{
    public int CharacterID { get; protected set; }
    public bool IsDead => status.Hp <= 0;
    public int Offense => status.Offense;
    public int Defense => status.Defense;

    protected CharacterStatus status;

    protected Command command;

    public void OnDamage(int amount)
    {
        status.Hp -= amount;
        Debug.Log("" + gameObject + "HP : " + status.Hp);
    }
}
