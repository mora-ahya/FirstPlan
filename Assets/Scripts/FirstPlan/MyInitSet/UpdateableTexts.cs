using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IUpdateableTextsHandler
{
    public string GetUpdateableTextString(int num, GameObject gObject);
}

public class UpdateableTexts : MonoBehaviour
{
    readonly List<Text> texts = new List<Text>();

    IUpdateableTextsHandler updateableTextsHandler;

    void Awake()
    {
        Text[] t = GetComponentsInChildren<Text>(true);
        string tmp = "Updateable";

        foreach (Text te in t)
        {
            if (te.gameObject.name.Contains(tmp))
            {
                texts.Add(te);
            }
        }
    }

    public void SetUpdateableTextsHandler(IUpdateableTextsHandler handler)
    {
        updateableTextsHandler = handler;
    }

    public void UpdateTexts()
    {
        if (updateableTextsHandler == null)
        {
            return;
        }

        for (int i = 0; i < texts.Count; i++)
        {
            string tmp = updateableTextsHandler.GetUpdateableTextString(i, texts[i].gameObject);
            if (tmp == null)
            {
                continue;
            }
            texts[i].text = tmp;
        }
    }
}
