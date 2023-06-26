using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FPBattleUI : MonoBehaviour, IMenuFrameHolder
{
    public bool IsBattleStrsEmpty => battleStrs.Count == 0;

    [SerializeField] MenuFrame battleCommand;
    [SerializeField] Text[] battleTexts;
    [SerializeField] UpdateableTexts playerStatusUI;
    [SerializeField] UpdateableTexts enemyStatusUI;

    Queue<string> battleStrs = new Queue<string>();

    int usingTextCount = 0;

    FPBattlePlayer player;

    void Awake()
    {
        battleCommand.SetMenuFrameHolder(this);
        usingTextCount = battleTexts.Length;
        ClearTexts();
    }

    public void SetPlayer(FPBattlePlayer p)
    {
        player = p;
        playerStatusUI.SetUpdateableTextsHandler(p);
    }

    public void SetEnemy(FPBattleEnemy e)
    {
        enemyStatusUI.SetUpdateableTextsHandler(e);
    }

    public void ShowUI(bool b)
    {
        gameObject.SetActive(b);
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
        battleStrs.Enqueue(str);
    }

    public void ApplyText()
    {
        if (battleStrs.Count == 0)
        {
            return;
        }

        if (battleTexts.Length == usingTextCount)
        {
            ClearTexts();
        }

        battleTexts[usingTextCount++].text = battleStrs.Dequeue();
    }

    public void ClearTexts()
    {
        for (int i = 0; i < usingTextCount; i++)
        {
            battleTexts[i].text = string.Empty;
        }

        usingTextCount = 0;
    }

    #region UpdateableTexts
    public void UpdatePlayerStatusUI()
    {
        playerStatusUI.UpdateTexts();
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
                battleCommand.SetUp(1, 3);
                break;

            case CommandPhase.SkillSelect:
                NumOfContent = player.GetHavingSkillNum() + 3;
                battleCommand.SetUp(3, 4);
                break;
        }
    }

    public void SetContent(int num, GameObject gObject)
    {
        switch (currentCommandPhase)
        {
            case CommandPhase.Common:
                SetCommonContent(num, gObject);
                break;

            case CommandPhase.SkillSelect:
                SetSkillContent(num, gObject);
                break;
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
        text.color = Color.white;
    }

    void SetSkillContent(int num, GameObject gObject)
    {
        if (num < 0 || num >= NumOfContent)
        {
            gObject.SetActive(false);
            return;
        }

        gObject.SetActive(true);
        Text text = gObject.GetComponent<Text>();

        if (num == 0)
        {
            text.color = Color.white;
            text.text = "戻る";
        }
        else if (num < 3)
        {
            text.text = null;
        }
        else
        {
            num -= 3;
            text.text = player.GetSkillName(num);
            //if (num % 4 == 0)
            //{
            //    text.color = Color.gray;
            //}
            //else
            //{
            //    text.color = Color.white;
            //}
        }
    }

    public void OnClicked(int num, PointerEventData eventData)
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
        else if (num < 3)
        {
            // 何もしない
        }
        else
        {
            num -= 3;
            player.DecideSkill(num);
            player.DecideCommand();
            EndPlotting();
        }
    }
    #endregion
}
