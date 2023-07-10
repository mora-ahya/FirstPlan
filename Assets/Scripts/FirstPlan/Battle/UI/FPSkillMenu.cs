using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class FPSkillMenu : MonoBehaviour
{
    static readonly string countFormat = "x {0,2}";
    [SerializeField] Text nameText;
    [SerializeField] Text countText;

    public void SetContents(string nameStr, int count)
    {
        nameText.text = nameStr;
        countText.text = count < 0 ? string.Empty : string.Format(countFormat, count);

        if (count == 0)
        {
            nameText.color = Color.gray;
            countText.color = Color.gray;
        }
        else
        {
            nameText.color = Color.white;
            countText.color = Color.white;
        }
    }
}
