using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IMenuFrameHolder
{
    public int NumOfContent { get; }
    public void SetContent(int num, GameObject gameObject);
    public void OnSelected(int num, PointerEventData eventData);
}

public class MenuFrame : MonoBehaviour
{
    public int CurrentTopLeftNum { get; private set; } = 0;

    protected RectTransform selfRectTransform;

    protected float paddingX = 10.0f;
    protected float paddingY = 5.0f;
    protected int widthElementNum;
    protected float elementWidth;
    protected float elementWidthOffset;

    protected int heightElementNum;
    protected float elementHeight;
    protected float elementHeightOffset;

    protected IMenuFrameHolder holder;

    protected int usedElementCount = 1;
    protected readonly List<RectTransform> elements = new List<RectTransform>();
    protected GameObject elementsMaster;


    protected int moveID = 0;
    protected bool isUp;
    protected TimerManager.TimerOnceEventHandler onEndScroll;

    // 同じContentなら自由に使いまわしできるようにしたい、elements配列のメモリ節約的な追加を実装する
    public void SetUp(int widthEleNum, int heightEleNum, float eleWidthOffset = 0.0f, float eleHeightOffset = 0.0f)
    {
        if (selfRectTransform == null)
        {
            selfRectTransform = GetComponent<RectTransform>();
            onEndScroll = OnEndScroll;
            elementsMaster = transform.Find("ElementMaster").gameObject;
            GameObject tmp = elementsMaster.transform.Find("Element").gameObject;
            if (tmp != null)
            {
                RectTransform tmp2 = tmp.GetComponent<RectTransform>();
                if (tmp2 != null)
                {
                    elementWidth = tmp2.sizeDelta.x;
                    elementHeight = tmp2.sizeDelta.y;
                    elements.Add(tmp2);
                }
            }
        }

        usedElementCount = widthEleNum * (heightEleNum + 2);
        if (elements.Capacity < usedElementCount)
        {
            elements.Capacity = usedElementCount;
        }

        widthElementNum = widthEleNum;
        elementWidthOffset = eleWidthOffset;

        heightElementNum = heightEleNum;
        elementHeightOffset = eleHeightOffset;

        selfRectTransform.sizeDelta = new Vector2(widthElementNum * (elementWidth + elementWidthOffset) + paddingX * 2.0f, heightElementNum * (elementHeight + elementHeightOffset) + paddingY * 2.0f);

        UpdateMenuWithResetElementPos();
    }

    public void UpdateMenuWithResetElementPos()
    {
        elementsMaster.transform.localPosition = Vector3.zero;
        CurrentTopLeftNum = 0;

        Vector3 topLeftPos = new Vector3((-selfRectTransform.sizeDelta.x + elementWidth) / 2.0f + paddingX, (selfRectTransform.sizeDelta.y - elementHeight) / 2.0f - paddingY, 0.0f);
        Vector3 wDiffVec = new Vector3(elementWidth + elementWidthOffset, 0.0f, 0.0f);
        Vector3 hDiffVec = new Vector3(0.0f, elementHeight + elementHeightOffset, 0.0f);

        for (int i = 0; i < usedElementCount; i++)
        {
            int hNum = i / widthElementNum;
            int wNum = i % widthElementNum;

            RectTransform rectTransform = GetOrCreateElement((i + CurrentTopLeftNum) % usedElementCount);

            if (i < usedElementCount - widthElementNum)
            {
                rectTransform.localPosition = topLeftPos + wDiffVec * wNum - hDiffVec * hNum;
                holder?.SetContent(i + CurrentTopLeftNum, rectTransform.gameObject);
            }
            else
            {
                rectTransform.localPosition = topLeftPos + wDiffVec * wNum + hDiffVec;
                holder?.SetContent(CurrentTopLeftNum + wNum - widthElementNum, rectTransform.gameObject);
            }
        }

        for (int wNum = usedElementCount; wNum < elements.Count; wNum++)
        {
            elements[wNum].gameObject.SetActive(false);
        }
    }

    public void UpdateMenu()
    {
        for (int i = 0; i < usedElementCount; i++)
        {
            int wNum = i % widthElementNum;

            RectTransform rectTransform = GetOrCreateElement((i + CurrentTopLeftNum) % usedElementCount);

            if (i < usedElementCount - widthElementNum)
            {
                holder?.SetContent(i + CurrentTopLeftNum, rectTransform.gameObject);
            }
            else
            {
                holder?.SetContent(CurrentTopLeftNum + wNum - widthElementNum, rectTransform.gameObject);
            }
        }
    }

    protected RectTransform GetOrCreateElement(int num)
    {
        if (num < 0 || usedElementCount <= num)
        {
            return null;
        }
        else if (elements.Count <= num)
        {
            RectTransform tmp = null;

            while (elements.Count <= num)
            {
                tmp = Instantiate(elements[0], elementsMaster.transform);
                elements.Add(tmp);
            }

            return tmp;
        }
        else
        {
            return elements[num];
        }
    }

    public void SetCurrentTopLeftNum(int num)
    {
        if (num < 0 || num > usedElementCount)
        {
            return;
        }
        usedElementCount = num / widthElementNum - num % widthElementNum;
    }

    public void SetMenuFrameHolder(IMenuFrameHolder menuFrameHolder)
    {
        holder = menuFrameHolder;
    }

    public void ScrollElements(bool isUp)
    {
        Mover.EndMove(moveID);

        if (holder == null || CurrentTopLeftNum == 0 && isUp == false || CurrentTopLeftNum + (widthElementNum * heightElementNum) >= holder.NumOfContent && isUp)
        {
            return;
        }

        this.isUp = isUp;
        float dir = isUp ? 1.0f : -1.0f;
        moveID = Mover.StartMove(elementsMaster, elementsMaster.transform.position + new Vector3(0.0f, elementHeight, 0.0f) * dir, 0.15f, endProcess: onEndScroll);
    }

    protected void OnEndScroll(int timerID)
    {
        int topElementNum = CurrentTopLeftNum % usedElementCount;
        int elementNum1 = (topElementNum + usedElementCount - widthElementNum) % usedElementCount;
        int elementNum2 = (elementNum1 + usedElementCount - widthElementNum) % usedElementCount;
        int dir = 1;

        if (isUp == false)
        {
            int tmp = elementNum1;
            elementNum1 = elementNum2;
            elementNum2 = tmp;
            dir = -1;
        }

        for (int i = 0; i < widthElementNum; i++)
        {
            elements[elementNum1 + i].transform.position = elements[elementNum2 + i].transform.position - new Vector3(0.0f, elementHeight, 0.0f) * dir;
            holder.SetContent(CurrentTopLeftNum + i - widthElementNum + (dir == 1 ? usedElementCount : -widthElementNum), elements[elementNum1 + i].gameObject);
        }

        CurrentTopLeftNum += dir * widthElementNum;
    }
}
