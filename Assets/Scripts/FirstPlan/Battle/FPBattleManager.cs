using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public interface IDamageable
{
    
}

// �o�g���J�n���C�x���g�R���ɂ���
public class FPBattleManager : MonoBehaviour, MyInitSet.ISceneActable, IFPGameSceneChild, ITurnBasedPlottingSystemEventReceiver, ITurnBasedBattleSystemEventReceiver
{
    static readonly string OpeningString = "{0}�@�������ꂽ";
    static readonly string AttackString = "{0}�@�̂�������";
    static readonly string SkillString = "{0}�@�� {1} ��������";
    static readonly string RunAwayString = "{0}�@�ɂ�������";
    static readonly string DamageString = "{0}�@��{1}�̃_���[�W����������";
    static readonly string DamagedString = "{0}�@��{1}�̃_���[�W��������";

    enum Phase
    {
        Opening,
        Plotting,
        Processing,
        Ending,
    }

    Phase currentPhase = Phase.Opening;

    public int ActPriority { get; } = 0;
    bool isActive = false;

    readonly TurnBasedBattleSystem battleSystem = new TurnBasedBattleSystem();
    readonly TurnBasedPlottingSystem plottingSystem = new TurnBasedPlottingSystem();

    FPGameScene gameScene;

    [SerializeField] FPBattleUI battleUI;
    [SerializeField] FPBattlePlayer player;
    [SerializeField] FPBattleEnemy enemy;

    FPBattleEventConfig holdingEvent;

    void Awake()
    {
        battleUI.SetPlayer(player);
        battleUI.SetEnemy(enemy);

        plottingSystem.SetEventReceiver(this);
        battleSystem.SetEventReceiver(this);
    }

    public void SetScene(FPGameScene gScene)
    {
        gameScene = gScene;
    }

    public void StartBattle(FPBattleEventConfig battleEventConfig)
    {
        // enemyID����enemyConfig���擾����enemy�ɃZ�b�g����
        holdingEvent = battleEventConfig;
        enemy.SetConfig(FPDataManager.Instance.GetEnemyConfig(battleEventConfig.EnemyID));

        // �o�g��UI�̕\��
        battleUI.ShowUI(true);
        battleUI.AddBattleStr(string.Format(OpeningString, enemy.Name));
        battleUI.ApplyText();
        battleUI.UpdatePlayerStatusUI();
        battleUI.UpdateEnemyStatusUI();

        isActive = true;
        currentPhase = Phase.Opening;
        battleSystem.AddTurnBasedBattler(player);
        battleSystem.AddTurnBasedBattler(enemy);
        plottingSystem.AddPlotter(player);
    }

    public void StartPlotting()
    {
        battleUI.StartPlotting();
    }

    void EndBattle()
    {
        if (enemy.IsDead)
        {
            holdingEvent.IsActive = false;
            holdingEvent.gameObject.SetActive(false);
        }

        // �o�g��UI�̕\��
        battleUI.ShowUI(false);
        isActive = false;
        battleSystem.ClearTurnBasedBattlers();
        plottingSystem.ClearPlotters();

        gameScene.EndBattleEvent();
    }

    public void OnPressCancel()
    {
        
    }

    public void ApplyCommand(Command command)
    {
        FPBattleCharacter attacker;
        FPBattleCharacter defender;

        if (command.OwnerID == 0)
        {
            attacker = player;
            defender = enemy;
        }
        else
        {
            attacker = enemy;
            defender = player;
        }

        int offense = attacker.Offense;
        int defense = defender.Defense;

        defender.OnDamage(Mathf.Max(offense - defense, 0));

        battleUI.UpdatePlayerStatusUI();
        battleUI.UpdateEnemyStatusUI();
    }

    #region ISceneActable
    public void ActSceneChild()
    {
        if (isActive == false)
        {
            return;
        }

        switch (currentPhase)
        {
            case Phase.Opening:
                ActOpening();
                break;

            case Phase.Plotting:
                ActPlotting();
                break;

            case Phase.Processing:
                ActProcessing();
                break;

            case Phase.Ending:
                ActEnding();
                break;
        }
    }

    void ActOpening()
    {
        if (Input.GetMouseButtonDown(0) && battleUI.IsBattleStrsEmpty)
        {
            battleUI.ClearTexts();
            currentPhase = Phase.Plotting;
            plottingSystem.StartPlotting();
        }
    }

    void ActPlotting()
    {
        plottingSystem.Act();
    }

    void ActProcessing()
    {
        battleSystem.Act();
    }

    void ActEnding()
    {
        if (Input.GetMouseButtonDown(0))
        {
            EndBattle();
        }
    }
    #endregion

    #region ITurnBasedPlottingSystemEventReceiver
    public void OnEndAllPlotting()
    {
        currentPhase = Phase.Processing;
        battleSystem.StartBattleProcess();
    }
    #endregion

    #region ITurnBasedBattleSystemEventReceiver
    public void OnEndAllBattleProcess()
    {
        if (player.IsOutOfBattle || enemy.IsOutOfBattle)
        {
            currentPhase = Phase.Ending;
        }
        else
        {
            currentPhase = Phase.Plotting;
            plottingSystem.StartPlotting();
        }
    }
    #endregion
}
