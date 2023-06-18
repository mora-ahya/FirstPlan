using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

//�܂��͈ꕶ�����\���Ȃǂ��l�����Ɏ�������(����������A����ς�Substring���낤��)
//text mesh pro�͍��̂Ƃ���p�ꂾ���������߂�
// menuFrame�Ɠ��l�̕��@�ŕ����s����������
//�ꕶ�����\�����s���ɂ�1.�ʏ��text��SubString���� 2.�ʏ�̃e�L�X�g��Mask���g�� 3.���{��Ή��t�H���g�����������Ă���mesh pro���g����charaMax���g��
//���ʂ�text���g�����̂ƁA���b�V���v�����g�����̗̂�������Ă���
public class TextFrame : MonoBehaviour, IManagerBase
{
    class TextEvent
    {
        int eventID;
    }

    readonly string[] escapeSecences = { "$", ":" };
    readonly string[] triggerString = { "$name", "$wait", "$stay" }; // �ʂ̕��@�ŊǗ��ł��邩������Ȃ�
    // �Ⴆ�΂��̂悤�ȏ����̓��[�h���ɂ���ăC�x���g�֐��𐶐�����

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
