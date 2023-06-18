using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPBattleEventConfig : MonoBehaviour, IGameEventConfig
{
    public bool IsActive { get { return isActive; } set { isActive = value; } }
    public int EventID => (int)FPGameEventKind.Battle;
    public int EnemyID => enemyId;

    [SerializeField] bool isActive = true;
    [SerializeField] int enemyId = 0;
}
