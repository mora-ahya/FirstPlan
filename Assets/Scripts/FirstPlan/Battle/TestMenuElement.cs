using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestMenuElement : MonoBehaviour
{
    [SerializeField] Text text;

    public void SetText(string str)
    {
        text.text = str;
    }
}
