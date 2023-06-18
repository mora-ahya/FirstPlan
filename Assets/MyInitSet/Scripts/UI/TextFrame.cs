using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

//まずは一文字ずつ表示などを考えずに実装する(いずれ実装、やっぱりSubstringだろうか)
//text mesh proは今のところ英語だけだからやめる
// menuFrameと同様の方法で複数行を実現する
//一文字ずつ表示を行うには1.通常のtextでSubStringする 2.通常のテキストでMaskを使う 3.日本語対応フォントを引っ張ってきてmesh proを使ってcharaMaxを使う
//普通のtextを使うものと、メッシュプロを使うものの両方作っておく
public class TextFrame : MonoBehaviour, IManagerBase
{
    class TextEvent
    {
        int eventID;
    }

    readonly string[] escapeSecences = { "$", ":" };
    readonly string[] triggerString = { "$name", "$wait", "$stay" }; // 別の方法で管理できるかもしれない
    // 例えばこのような処理はロード時にやってイベント関数を生成する

    public int ActPriority { get; } = 0;

    [SerializeField] Text mainText = default;

    List<string> multipleText = null;
    int indexNum = 0;
    int displayNum = 0;
    int counter = 0;
    bool isStayed = false;
    string[] splitted;
    public int displaySpeed = 3;
    WaitForSeconds wait;

    void Awake()
    {
        mainText.text = null;
        gameObject.SetActive(false);
    }

    public void Act()
    {
        if (!gameObject.activeSelf)
            return;

        if (multipleText != null)
        {
            MultipleText();
        }
        else
        {
            SingleText();
        }
    }

    public void Set(string str)
    {
        mainText.text = str;
    }

    public void Set(List<string> mStr)
    {
        multipleText = mStr;
    }

    public void ResetObject()
    {
        multipleText = null;
        mainText.text = null;
        indexNum = 0;
        displayNum = 0;
        gameObject.SetActive(false);
    }

    void SingleText()
    {
        if (Input.GetMouseButtonDown(0))
        {

        }
    }

    void MultipleText()
    {
        if (multipleText.Count > indexNum)
        {
            if (multipleText[indexNum].Length > 0 && multipleText[indexNum].Substring(0, 1) == escapeSecences[0])
            {
                EscapeProcess();
                indexNum++;
                return;
            }

            if (multipleText[indexNum].Length >= displayNum)
            {
                if (counter++ == displaySpeed)
                {
                    mainText.text = multipleText[indexNum].Substring(0, displayNum++);
                    counter = 0;
                }

                if (Input.GetMouseButtonDown(0))
                {
                    displayNum = multipleText[indexNum].Length;
                    mainText.text = multipleText[indexNum];
                }
                return;
            }
        }

        if (!Input.GetMouseButtonDown(0))
            return;

        if (multipleText.Count <= indexNum + 1)
        {
            multipleText = null;
            indexNum = 0;
            displayNum = 0;
        }
        else
        {
            indexNum++;
            displayNum = 0;
        }
    }

    void EscapeProcess()
    {
        splitted = multipleText[indexNum].Split(escapeSecences[1][0]);
        if (splitted[0] == triggerString[0])
        {
            return;
        }

        if (splitted[0] == triggerString[1])
        {
            gameObject.SetActive(false);
            mainText.text = null;
            return;
        }

        if (splitted[0] == triggerString[2])
        {
            isStayed = true;
            return;
        }
    }
}
