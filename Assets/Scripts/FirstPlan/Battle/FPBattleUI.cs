using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FPBattleUI : MonoBehaviour, IMenuFrameHolder
{
    [SerializeField] MenuFrame battleCommand;

    FPBattlePlayer player;

    void Awake()
    {
        battleCommand.SetMenuFrameHolder(this);
    }

    public void SetPlayer(FPBattlePlayer p)
    {
        player = p;
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
                NumOfContent = 30;
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
            text.text = "";
        }
        else
        {
            num -= 3;
            text.text = player.GetSkillInfo(num);
            if (num % 4 == 0)
            {
                text.color = Color.gray;
            }
            else
            {
                text.color = Color.white;
            }
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
            Command command = new Command();
            command.OwnerID = 0;
            command.Kind = 1;
            command.SkillNum = num;

            player.SetCommand(command);
            player.DecideCommand();
            EndPlotting();
        }
    }
    #endregion
}
