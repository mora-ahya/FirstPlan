using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public interface IDamageable
{
    
}

public class FPBattleManager : MonoBehaviour, IManagerBase, ITurnBasedPlottingSystemEventReceiver, ITurnBasedBattleSystemEventReceiver
{
    public static FPBattleManager Instance { get; private set; }

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

    [SerializeField] FPBattleUI battleUI;
    [SerializeField] FPBattlePlayer player;
    FPBattleEnemy enemy;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        battleUI.SetPlayer(player);

        plottingSystem.SetEventReceiver(this);
        battleSystem.SetEventReceiver(this);

        ManagerParent.Instance.AddManager(plottingSystem);
        ManagerParent.Instance.AddManager(battleSystem);
    }

    public void SetEnemy(FPBattleEnemy fpBattleEnemy)
    {
        enemy = fpBattleEnemy;
        battleSystem.AddTurnBasedBattler(fpBattleEnemy);
    }

    public void SetPlayer(FPBattlePlayer player)
    {
        this.player = player;
    }

    public void StartBattle()
    {
        // バトルUIの表示
        battleUI.ShowUI(true);
        isActive = true;
        currentPhase = Phase.Opening;
        battleSystem.AddTurnBasedBattler(player);
        plottingSystem.AddPlotter(player);
    }

    public void StartPlotting()
    {
        battleUI.StartPlotting();
    }

    void EndBattle()
    {
        // バトルUIの表示
        battleUI.ShowUI(false);
        isActive = false;
        battleSystem.ClearTurnBasedBattlers();
        plottingSystem.ClearPlotters();
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
    }

    #region IManagerBase
    public void Act()
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
        if (Input.GetMouseButtonDown(0))
        {
            currentPhase = Phase.Plotting;
            plottingSystem.StartPlotting();
        }
    }

    void ActPlotting()
    {

    }

    void ActProcessing()
    {

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
        if (player.IsDead || enemy.IsDead)
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
