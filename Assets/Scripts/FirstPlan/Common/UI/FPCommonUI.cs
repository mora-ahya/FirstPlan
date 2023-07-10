using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPCommonUI : MonoBehaviour
{
    public bool IsCommonStrsEmpty => commonStrs.Count == 0;

    [SerializeField] UpdateableTexts playerStatusUI;
    [SerializeField] GameObject commonTextsParent;
    [SerializeField] Text[] commonTexts;

    int usingTextCount = 0;

    readonly Queue<string> commonStrs = new Queue<string>();

    void Awake()
    {
        usingTextCount = commonTexts.Length;
    }

    public void SetPlayer(FPBattlePlayer player)
    {
        playerStatusUI.SetUpdateableTextsHandler(player);
        playerStatusUI.UpdateTexts();
    }

    public void UpdatePlayerStatus()
    {
        playerStatusUI.UpdateTexts();
    }

    public void ShowCommonTexts(bool b)
    {
        commonTextsParent.SetActive(b);
    }

    public void AddCommonStr(string str)
    {
        commonStrs.Enqueue(str);
    }

    public void ApplyText()
    {
        if (commonStrs.Count == 0)
        {
            return;
        }

        if (commonTexts.Length == usingTextCount)
        {
            ClearTexts();
        }

        commonTexts[usingTextCount++].text = commonStrs.Dequeue();
    }

    public void ClearTexts()
    {
        for (int i = 0; i < usingTextCount; i++)
        {
            commonTexts[i].text = string.Empty;
        }

        usingTextCount = 0;
    }
}
