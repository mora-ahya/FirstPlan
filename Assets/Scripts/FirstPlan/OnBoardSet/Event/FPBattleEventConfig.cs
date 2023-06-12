using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPBattleEventConfig : MonoBehaviour, IGameEventConfig
{
    public bool IsActive { get; }
    public int EventID { get; }
    public int EnemyID { get; protected set; }

    public void Initialize(int enemyID)
    {
        
        //Image image = GetComponent<Image>();
        EnemyConfig enemyConfig = FPDataManager.Instance.GetEnemyConfig(enemyID);

        EnemyID = enemyID;
        //image.sprite = enemyConfig.EnemySprite;
    }
}
