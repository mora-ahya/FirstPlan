using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public interface IDamageable
{
    
}

// バトル開始をイベント由来にする
public class FPBattleManager : MonoBehaviour, MyInitSet.ISceneActable, IFPGameSceneChild, ITurnBasedPlottingSystemEventReceiver, ITurnBasedBattleSystemEventReceiver
{
    static readonly string OpeningString = "{0}　があらわれた";
    static readonly string AttackString = "{0}　のこうげき";
    static readonly string SkillString = "{0}　は {1} をつかった";
    static readonly string RunAwayPlayerString = "あなたはにげだした";
    static readonly string RunAwayEnemyString = "{0}　はにげだした";
    static readonly string DamageString = "{0}　に{1}のダメージをあたえた";
    static readonly string DamagedString = "{0}　は{1}のダメージをうけた";
    static readonly string KilledPlayerString = "あなたはちからつきた...";
    static readonly string KillEnemyString = "{0}　をたおした";
    static readonly string StatusUpString = "ステータスがあがった！";

    static readonly string ObservationString = "{0}　を注意深く観察することで";
    static readonly string ObservationString2 = "スキルを覚えられるかもしれない";
    static readonly string ObservationString3 = "観察しますか？";
    static readonly string ObservationSuccessString = "{0}　を覚えた！";
    static readonly string ObservationFailureString = "視界がぼやけて観察できなかった...";

    struct TurnResult
    {
        int actKind; // どんな行動をしたか
        int amount; // どれくらいダメージを与えたか
    }

    enum Phase
    {
        Opening,
        Plotting,
        Processing,
        Ending,
    }

    enum BattleResult
    {
        Win,
        Lose,
        PlayerRunAway,
        EnemyRunAway,
    }

    Phase currentPhase = Phase.Opening;
    BattleResult battleResult = BattleResult.Win;

    public int ActPriority { get; } = 0;
    bool isActive = false;

    public FPBattleSkillManager SkillManager => skillManager;

    readonly TurnBasedBattleSystem battleSystem = new TurnBasedBattleSystem();
    readonly TurnBasedPlottingSystem plottingSystem = new TurnBasedPlottingSystem();
    readonly FPBattleSkillManager skillManager = new FPBattleSkillManager();

    FPGameScene gameScene;

    [SerializeField] FPBattleUI battleUI;
    [SerializeField] FPBattlePlayer player;
    [SerializeField] FPBattleEnemy enemy;

    FPBattleEventConfig holdingEvent;

    int actPhase = 0;

    void Awake()
    {
        battleUI.SetPlayer(player);
        battleUI.SetEnemy(enemy);

        plottingSystem.SetEventReceiver(this);
        battleSystem.SetEventReceiver(this);
        skillManager.Initialize(this);
    }

    public void SetScene(FPGameScene gScene)
    {
        gameScene = gScene;
    }

    public void StartBattle(FPBattleEventConfig battleEventConfig)
    {
        // enemyIDからenemyConfigを取得してenemyにセットする
        holdingEvent = battleEventConfig;
        enemy.SetConfig(FPDataManager.Instance.GetEnemyConfig(battleEventConfig.EnemyID));

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
        if (enemy.IsDead)
        {
            holdingEvent.IsActive = false;
            holdingEvent.gameObject.SetActive(false);
        }

        // バトルUIの表示
        battleUI.ShowUI(false);
        isActive = false;
        battleSystem.ClearTurnBasedBattlers();
        plottingSystem.ClearPlotters();

        gameScene.EndBattleEvent();
    }

    public void OnPressCancel()
    {
        
    }

    // マネージャーはUI表示のために、キャラクターがどんな行動をしたのか、どのような結果になったかを保持しておきたい
    // しかし、現状のスキルの仕組みとマネージャーの処理ではかみ合わない
    public void ReportCharacterStartTurn(FPBattleCharacter character)
    {
        // ○○の攻撃
    }

    public void ReportCharacterUseSkill(FPBattleCharacter character, int skillID)
    {
        // ○○の目にもとまらぬ攻撃 など
    }

    // レポート系をバトルマネージャーから分離する
    public void ReportCharacterDamaged(FPBattleCharacter damager, int amount)
    {
        // ○○は amount のダメージをうけた
    }

    FPBattleCharacter GetCharacterByID(int id)
    {
        FPBattleCharacter chara;

        if (id == 0)
        {
            chara = player;
        }
        else
        {
            chara = enemy;
        }

        return chara;
    }

    public void DealDamageToCharacter(FPBattleCharacter dealer, int amount)
    {
        FPBattleCharacter defender = GetCharacterByID(dealer.GetCommand().TargetID);

        int damageAmount = defender.CalculateDefenseDamage(dealer, amount);
        defender.Damaged(damageAmount);

        battleUI.UpdatePlayerStatusUI();
        battleUI.UpdateEnemyStatusUI();
    }

    public void DealConditionToCharacter(FPBattleCharacter dealer, int conditionNum)
    {
        FPBattleCharacter affected = GetCharacterByID(dealer.GetCommand().TargetID);

        affected.conditions.AddCondition(conditionNum);
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
            if (actPhase == 0)
            {
                if (battleResult == BattleResult.Win)
                {
                    // ステータス上昇反映
                    EnemyConfig enemyConfig = enemy.GetConfig();
                    player.GrowUpStatus(StatusKindEnum.Offense, enemyConfig.DropAttackPoint);
                    player.GrowUpStatus(StatusKindEnum.Defense, enemyConfig.DropDefencePoint);
                    player.GrowUpStatus(StatusKindEnum.Speed, enemyConfig.DropSpeedPoint);
                    battleUI.UpdatePlayerStatusUI();
                }
                actPhase++;
            }
            else
            {
                EndBattle();
            }
        }
    }

    void ActEndingForWinResult()
    {
        //switch (actPhase)
        //{
        //    case 0:
        //        battleUI.ClearTexts();
        //        battleUI.AddBattleStr(string.Format(KillEnemyString, enemy.Name));
        //        battleUI.AddBattleStr(StatusUpString);
        //        battleUI.AddBattleStr(ObservationString);
        //        battleUI.AddBattleStr(ObservationString2);
        //        battleUI.AddBattleStr(ObservationString3);
        //        actPhase++;
        //        break;

        //    case 1:
        //        break;
        //}

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
            actPhase = 0;
            CheckBattleResult();
        }
        else
        {
            currentPhase = Phase.Plotting;
            plottingSystem.StartPlotting();
        }
    }

    void CheckBattleResult()
    {
        if (enemy.IsDead)
        {
            battleResult = BattleResult.Win;
        }
        else if (player.IsRunningAway)
        {
            battleResult = BattleResult.PlayerRunAway;
        }
        else if (player.IsDead)
        {
            battleResult = BattleResult.Lose;
        }
        else if (enemy.IsRunningAway)
        {
            battleResult = BattleResult.EnemyRunAway;
        }
    }
    #endregion
}
