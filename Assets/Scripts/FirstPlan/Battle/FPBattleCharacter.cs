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

public class FPBattleCharacter : MonoBehaviour, IUpdateableTextsHandler
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

    protected CharacterStatus status;

    protected Command command;

    protected int changedStatusFlag = 0;

    public virtual void OnDamage(int amount)
    {
        status.Hp -= amount;
        Debug.Log("" + gameObject + "HP : " + status.Hp);
    }

    #region Implement IUpdateableTextsHandler
    public int UpdateTextFlag => changedStatusFlag;

    public virtual string GetUpdateableTextString(int num, GameObject gObject)
    {
        return null;
    }
    #endregion
}
