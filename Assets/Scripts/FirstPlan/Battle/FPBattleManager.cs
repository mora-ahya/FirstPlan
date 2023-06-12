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

    static readonly string OpeningString = "{0}　があらわれた";
    static readonly string AttackString = "{0}　のこうげき";
    static readonly string SkillString = "{0}　は {1} をつかった";
    static readonly string RunAwayString = "{0}　にげだした";
    static readonly string DamageString = "{0}　に{1}のダメージをあたえた";
    static readonly string DamagedString = "{0}　は{1}のダメージをうけた";

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
        battleUI.SetEnemy(fpBattleEnemy);
    }

    public void SetPlayer(FPBattlePlayer player)
    {
        this.player = player;
    }

    public void StartBattle()
    {
        // バトルUIの表示
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

    public void StartBattle(int enemyID)
    {
        // enemyIDからenemyConfigを取得してenemyにセットする

        // バトルUIの表示
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

        battleUI.UpdatePlayerStatusUI();
        battleUI.UpdateEnemyStatusUI();
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
        if (Input.GetMouseButtonDown(0) && battleUI.IsBattleStrsEmpty)
        {
            battleUI.ClearTexts();
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
