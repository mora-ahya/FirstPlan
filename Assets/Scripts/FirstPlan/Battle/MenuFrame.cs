using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IMenuFrameHolder
{
    public void SetContent(int num, GameObject gameObject);
    public void OnClicked(int num, PointerEventData eventData);
}

public interface IMenuContent
{
    public void OnDisplay();
}

// スクロールに対応
// 各要素のアクティブ、非アクティブに対応する
// 要素は縦か横の一時配列にして、数を増やす場合は再帰的に処理するようにしたい
public class MenuFrame : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IScrollHandler
{
    float paddingX = 10.0f;
    float paddingY = 5.0f;
    int widthElementNum;
    float elementWidth;
    float elementWidthOffset;

    int heightElementNum;
    float elementHeight;
    float elementHeightOffset;

    IMenuFrameHolder holder;

    readonly List<GameObject> elements = new List<GameObject>();

    //[SerializeField] List<GameObject> testElements = new List<GameObject>();
    GameObject elementsMaster;

    int currentTopNum = 0;


    int moveID = 0;
    bool isUp;
    TimerManager.TimerOnceEventHandler onEndScroll;

    void Awake()
    {
        onEndScroll = OnEndScroll;
        elementsMaster = transform.Find("ElementMaster").gameObject;
        GameObject tmp = elementsMaster.transform.Find("Element").gameObject;

        if (tmp != null)
        {
            elements.Add(tmp);
        }
    }

    // 同じContentなら自由に使いまわしできるようにしたい、elements配列のメモリ節約的な追加を実装する
    public void Initialize(int widthEleNum, int heightEleNum, float eleWidthOffset = 0.0f, float eleHeightOffset = 0.0f)
    {
        RectTransform firstElement = elements[0].GetComponent<RectTransform>();

        widthElementNum = widthEleNum;
        elementWidth = firstElement.sizeDelta.x;
        elementWidthOffset = eleWidthOffset;

        heightElementNum = heightEleNum;
        elementHeight = firstElement.sizeDelta.y;
        elementHeightOffset = eleHeightOffset;

        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(widthElementNum * (elementWidth + elementWidthOffset) + paddingX * 2.0f, heightElementNum * (elementHeight + elementHeightOffset) + paddingY * 2.0f);

        firstElement.localPosition = new Vector3((-rectTransform.sizeDelta.x + elementWidth) / 2.0f + paddingX, (rectTransform.sizeDelta.y - elementHeight) / 2.0f - paddingY, 0.0f);

        Vector3 diffVec = new Vector3(0.0f, elementHeight + eleHeightOffset, 0.0f);
        RectTransform create;

        for (int i = 0; i < heightEleNum; i++)
        {
            create = Instantiate(firstElement, elementsMaster.transform);
            create.localPosition = firstElement.localPosition - diffVec * (i + 1);
            elements.Add(create.gameObject);
        }

        create = Instantiate(firstElement, elementsMaster.transform);
        create.localPosition = firstElement.localPosition + diffVec;

        elements.Add(create.gameObject);
    }

    public void SetMenuFrameHolder(IMenuFrameHolder menuFrameHolder)
    {
        holder = menuFrameHolder;
    }

    public void ScrollElements(bool isUp)
    {
        Mover.EndMove(moveID);
        this.isUp = isUp;
        float dir = isUp ? 1.0f : -1.0f;
        moveID = Mover.StartMove(elementsMaster, elementsMaster.transform.position + new Vector3(0.0f, 20.0f, 0.0f) * dir, 0.15f, endProcess: onEndScroll);
    }

    void OnEndScroll(int timerID)
    {
        int topElementNum = currentTopNum % elements.Count;
        int elementNum1 = (topElementNum + elements.Count - 1) % elements.Count;
        int elementNum2 = (elementNum1 + elements.Count - 1) % elements.Count;
        int dir = 1;

        if (isUp == false)
        {
            int tmp = elementNum1;
            elementNum1 = elementNum2;
            elementNum2 = tmp;
            dir = -1;
        }

        elements[elementNum1].transform.position = elements[elementNum2].transform.position - new Vector3(0.0f, 20.0f, 0.0f) * dir;
        holder.SetContent(currentTopNum + (dir == 1 ? elements.Count : -1), elements[elementNum1].gameObject);

        currentTopNum += dir;

        // ondisplay
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        holder?.OnClicked(0, eventData);
    }

    Vector2 beginDragPos;

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
        ScrollElements(diff > 0.0f);
    }

    public void OnScroll(PointerEventData eventData)
    {
        ScrollElements(eventData.scrollDelta.y < 0.0f);
    }
}
