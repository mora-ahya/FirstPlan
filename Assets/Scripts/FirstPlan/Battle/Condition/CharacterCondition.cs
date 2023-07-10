using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterConditions
{
    protected int conditionFlag;

    public bool HasCondition(int conditionNum)
    {
        return (conditionFlag & (1 << conditionNum)) != 0;
    }

    public void AddCondition(int conditionNum)
    {
        conditionFlag |= 1 << conditionNum;
    }

    public void RemoveCondition(int conditionNum)
    {
        conditionFlag &= ~(1 << conditionNum);
    }

    public void ClearConditions()
    {
        conditionFlag = 0;
    }
}
