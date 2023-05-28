using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IMenuFrameHolder
{
    public int NumOfContent { get; }
    public void SetContent(int num, GameObject gameObject);
    public void OnClicked(int num, PointerEventData eventData);
}

public interface IMenuContent
{
    public void OnDisplay();
}

// 各要素のアクティブ、非アクティブに対応する
// 要素は縦か横の一時配列にして、数を増やす場合は再帰的に処理するようにしたい
public class MenuFrame : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IScrollHandler
{
    public int CurrentTopLeftNum { get; private set; } = 0;

    RectTransform selfRectTransform;

    float paddingX = 10.0f;
    float paddingY = 5.0f;
    int widthElementNum;
    float elementWidth;
    float elementWidthOffset;

    int heightElementNum;
    float elementHeight;
    float elementHeightOffset;

    IMenuFrameHolder holder;

    int usedElementCount = 1;
    readonly List<RectTransform> elements = new List<RectTransform>();

    //[SerializeField] List<GameObject> testElements = new List<GameObject>();
    GameObject elementsMaster;


    int moveID = 0;
    bool isUp;
    TimerManager.TimerOnceEventHandler onEndScroll;

    void Awake()
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

    // 同じContentなら自由に使いまわしできるようにしたい、elements配列のメモリ節約的な追加を実装する
    public void Initialize(int widthEleNum, int heightEleNum, float eleWidthOffset = 0.0f, float eleHeightOffset = 0.0f)
    {
        CurrentTopLeftNum = 0;
        usedElementCount = widthEleNum * (heightEleNum + 2);
        if (elements.Capacity < usedElementCount)
        {
            elements.Capacity = usedElementCount;
        }
        elementsMaster.transform.localPosition = Vector3.zero;

        widthElementNum = widthEleNum;
        elementWidthOffset = eleWidthOffset;

        heightElementNum = heightEleNum;
        elementHeightOffset = eleHeightOffset;

        selfRectTransform.sizeDelta = new Vector2(widthElementNum * (elementWidth + elementWidthOffset) + paddingX * 2.0f, heightElementNum * (elementHeight + elementHeightOffset) + paddingY * 2.0f);

        UpdateMenuFrame();
    }

    void UpdateMenuFrame()
    {
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

    RectTransform GetOrCreateElement(int num)
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

        if (holder == null || CurrentTopLeftNum == 0 && isUp == false || CurrentTopLeftNum + (widthElementNum * heightElementNum) > holder.NumOfContent && isUp)
        {
            return;
        }

        this.isUp = isUp;
        float dir = isUp ? 1.0f : -1.0f;
        moveID = Mover.StartMove(elementsMaster, elementsMaster.transform.position + new Vector3(0.0f, 20.0f, 0.0f) * dir, 0.15f, endProcess: onEndScroll);
    }

    void OnEndScroll(int timerID)
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
            elements[elementNum1 + i].transform.position = elements[elementNum2 + i].transform.position - new Vector3(0.0f, 20.0f, 0.0f) * dir;
            holder.SetContent(CurrentTopLeftNum + i - widthElementNum + (dir == 1 ? usedElementCount : -widthElementNum), elements[elementNum1 + i].gameObject);
        }

        CurrentTopLeftNum += dir * widthElementNum;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isDrag)
        {
            isDrag = false;
            return;
        }

        int eleNum = GetPressElementNum(eventData.pressPosition);

        if (eleNum < 0 || eleNum > widthElementNum * heightElementNum || eleNum != GetPressElementNum(eventData.position))
        {
            return;
        }

        holder?.OnClicked(eleNum + CurrentTopLeftNum, eventData);
    }

    int GetPressElementNum(Vector2 pos)
    {
        Vector2 topLeft = selfRectTransform.position;
        topLeft.x -= selfRectTransform.sizeDelta.x / 2.0f - paddingX;
        topLeft.y += selfRectTransform.sizeDelta.y / 2.0f - paddingY;

        topLeft = pos - topLeft;

        if (topLeft.x < 0.0f || topLeft.x > selfRectTransform.sizeDelta.x - paddingX * 2.0f || topLeft.y > 0.0f || topLeft.y < -(selfRectTransform.sizeDelta.y - paddingY * 2.0f))
        {
            return -1;
        }

        topLeft.y = Mathf.Abs(topLeft.y);

        return Mathf.FloorToInt(topLeft.x / (elementWidth + elementWidthOffset)) + Mathf.FloorToInt(topLeft.y / (elementHeight + elementHeightOffset)) * widthElementNum;
    }

    Vector2 beginDragPos;
    bool isDrag;

    public void OnBeginDrag(PointerEventData eventData)
    {
        beginDragPos = eventData.pressPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        float diff = eventData.position.y - beginDragPos.y;
        if (Mathf.Abs(diff) < 20.0f)
        {
            return;
        }

        beginDragPos = eventData.position;
        isDrag = true;
        ScrollElements(diff > 0.0f);
    }

    public void OnScroll(PointerEventData eventData)
    {
        ScrollElements(eventData.scrollDelta.y < 0.0f);
    }
}
