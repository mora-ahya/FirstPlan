using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FPBattleUI : MonoBehaviour, IMenuFrameHolder
{
    public bool IsBattleStrsEmpty => commonUI.IsCommonStrsEmpty;

    [SerializeField] FPCommonUI commonUI;

    [SerializeField] MenuFrame battleCommand; // コマンドとスキル選択で分ける
    [SerializeField] MenuFrame skillMenu;
    [SerializeField] UpdateableTexts enemyStatusUI;

    FPBattlePlayer player;

    void Awake()
    {
        battleCommand.SetMenuFrameHolder(this);
        battleCommand.SetUp(1, 3);
        skillMenu.SetMenuFrameHolder(this);
        skillMenu.SetUp(1, 3);
        ClearTexts();
    }

    public void SetPlayer(FPBattlePlayer p)
    {
        player = p;
    }

    public void SetEnemy(FPBattleEnemy e)
    {
        enemyStatusUI.SetUpdateableTextsHandler(e);
    }

    public void ShowUI(bool b)
    {
        gameObject.SetActive(b);
        commonUI.ShowCommonTexts(b);
    }

    public void StartPlotting()
    {
        battleCommand.gameObject.SetActive(true);
        ChangeCommandPhase(CommandPhase.Common);
    }

    public void EndPlotting()
    {
        battleCommand.gameObject.SetActive(false);
    }

    public void AddBattleStr(string str)
    {
        commonUI.AddCommonStr(str);
    }

    public void ApplyText()
    {
        commonUI.ApplyText();
    }

    public void ClearTexts()
    {
        commonUI.ClearTexts();
    }

    #region UpdateableTexts
    public void UpdatePlayerStatusUI()
    {
        commonUI.UpdatePlayerStatus();
    }

    public void UpdateEnemyStatusUI()
    {
        enemyStatusUI.UpdateTexts();
    }
    #endregion

    #region IMenuFrameHolder Implement

    enum CommandPhase
    {
        Common,
        SkillSelect,
    }

    CommandPhase currentCommandPhase = CommandPhase.Common;

    readonly string[] commandTexts =
    {
        "こうげき",
        "わざ",
        "にげる",
    };

    public int NumOfContent { get; private set; }

    void ChangeCommandPhase(CommandPhase phase)
    {
        currentCommandPhase = phase;

        switch (phase)
        {
            case CommandPhase.Common:
                NumOfContent = commandTexts.Length;
                battleCommand.gameObject.SetActive(true);
                skillMenu.gameObject.SetActive(false);
                break;

            case CommandPhase.SkillSelect:
                NumOfContent = player.GetHavingSkillNum() + 1;
                battleCommand.gameObject.SetActive(false);
                skillMenu.gameObject.SetActive(true);
                skillMenu.UpdateMenu();
                break;
        }
    }

    public void SetContent(int num, GameObject gObject)
    {
        if (gObject.transform.parent.parent.name == battleCommand.gameObject.name)
        {
            SetCommonContent(num, gObject);
        }
        else
        {
            SetSkillContent(num, gObject);
        }
    }

    void SetCommonContent(int num, GameObject gObject)
    {
        if (num < 0 || num >= commandTexts.Length)
        {
            gObject.SetActive(false);
            return;
        }

        gObject.SetActive(true);
        Text text = gObject.GetComponent<Text>();
        text.text = commandTexts[num];
    }

    void SetSkillContent(int num, GameObject gObject)
    {
        if (num < 0 || num >= NumOfContent)
        {
            gObject.SetActive(false);
            return;
        }

        gObject.SetActive(true);
        FPSkillMenu skillMenu = gObject.GetComponent<FPSkillMenu>();

        if (num == 0)
        {
            skillMenu.SetContents("戻る", -1);
        }
        else
        {
            num -= 1;
            int usableTimes = player.GetSkillUsableTimes(num);
            skillMenu.SetContents(player.GetSkillName(num), usableTimes);
        }
    }

    public void OnSelected(int num, PointerEventData eventData)
    {
        switch (currentCommandPhase)
        {
            case CommandPhase.Common:
                OnClickedCommon(num, eventData);
                break;

            case CommandPhase.SkillSelect:
                OnClickedSkill(num, eventData);
                break;
        }
    }

    public void OnClickedCommon(int num, PointerEventData eventData)
    {
        if (num == 0)
        {
            OnPressAttack();
        }
        else if (num == 1)
        {
            OnPressSkill();
        }
        else if (num == 2)
        {
            OnPressRunAway();
        }
    }

    void OnPressAttack()
    {
        Command command = new Command();
        command.OwnerID = 0;
        command.Kind = 0;
        command.TargetID = 1;

        player.SetCommand(command);
        player.DecideCommand();
        EndPlotting();
    }

    void OnPressSkill()
    {
        // スキル欄を開く
        ChangeCommandPhase(CommandPhase.SkillSelect);
    }

    void OnPressRunAway()
    {
        // 今回は確定で成功させる
        // 一歩下がる
        Command command = new Command();
        command.OwnerID = 0;
        command.Kind = 2;

        player.SetCommand(command);
        player.DecideCommand();
        EndPlotting();
    }

    public void OnClickedSkill(int num, PointerEventData eventData)
    {
        if (num == 0)
        {
            ChangeCommandPhase(CommandPhase.Common);
        }
        else
        {
            num -= 1;
            if (player.GetSkillUsableTimes(num) == 0)
            {
                return;
            }
            
            player.DecideSkill(num);
            player.DecideCommand();
            EndPlotting();
        }
    }
    #endregion
}
