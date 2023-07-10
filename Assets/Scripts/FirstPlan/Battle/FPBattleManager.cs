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
    static readonly string RunAwayPlayerString = "���Ȃ��͂ɂ�������";
    static readonly string RunAwayEnemyString = "{0}�@�͂ɂ�������";
    static readonly string DamageString = "{0}�@��{1}�̃_���[�W����������";
    static readonly string DamagedString = "{0}�@��{1}�̃_���[�W��������";
    static readonly string KilledPlayerString = "���Ȃ��͂��������...";
    static readonly string KillEnemyString = "{0}�@����������";
    static readonly string StatusUpString = "�X�e�[�^�X�����������I";

    static readonly string ObservationString = "{0}�@�𒍈Ӑ[���ώ@���邱�Ƃ�";
    static readonly string ObservationString2 = "�X�L�����o�����邩������Ȃ�";
    static readonly string ObservationString3 = "�ώ@���܂����H";
    static readonly string ObservationSuccessString = "{0}�@���o�����I";
    static readonly string ObservationFailureString = "���E���ڂ₯�Ċώ@�ł��Ȃ�����...";

    struct TurnResult
    {
        int actKind; // �ǂ�ȍs����������
        int amount; // �ǂꂭ�炢�_���[�W��^������
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

    // �}�l�[�W���[��UI�\���̂��߂ɁA�L�����N�^�[���ǂ�ȍs���������̂��A�ǂ̂悤�Ȍ��ʂɂȂ�������ێ����Ă�������
    // �������A����̃X�L���̎d�g�݂ƃ}�l�[�W���[�̏����ł͂��ݍ���Ȃ�
    public void ReportCharacterStartTurn(FPBattleCharacter character)
    {
        // �����̍U��
    }

    public void ReportCharacterUseSkill(FPBattleCharacter character, int skillID)
    {
        // �����̖ڂɂ��Ƃ܂�ʍU�� �Ȃ�
    }

    // ���|�[�g�n���o�g���}�l�[�W���[���番������
    public void ReportCharacterDamaged(FPBattleCharacter damager, int amount)
    {
        // ������ amount �̃_���[�W��������
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
                    // �X�e�[�^�X�㏸���f
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
