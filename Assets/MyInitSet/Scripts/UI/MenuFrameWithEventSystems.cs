using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuFrameWithEventSystems : MenuFrame, IPointerClickHandler, IBeginDragHandler, IDragHandler, IScrollHandler
{
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

        holder?.OnSelected(eleNum + CurrentTopLeftNum, eventData);
    }

    protected int GetPressElementNum(Vector2 pos)
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
