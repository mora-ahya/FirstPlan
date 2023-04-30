using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IObjectOnBoard
{
    public GameObject SelfGameObject { get; }
    public DirectionClass.DirectionEnum Direction { get; }
    public int positionNum { get; set; }

    public void OnHappendBoardEvent(int boardEventID);
}

struct BoardPart
{
    public int Number;
    public int HavingEventID;
    public IObjectOnBoard HavingObject;
}

public class Board : MonoBehaviour
{
    public float PartSize { get; } = 1.0f;
    int width;
    int height;

    BoardPart[] boardParts;
    GameObject basePosition;

    List<IObjectOnBoard> objectsOnBoard = new List<IObjectOnBoard>();

    public virtual Vector3 PartNumberToWorldPosition(int partNum)
    {
        return new Vector3((partNum % width + 0.5f) * PartSize, 0f, (partNum / width + 0.5f) * PartSize) + basePosition.transform.position;
    }

    public virtual void PartNumberToWorldPosition(int partNum, ref Vector3 dest)
    {
        dest.Set((partNum % width + 0.5f) * PartSize, 0f, (partNum / width + 0.5f) * PartSize);
        dest += basePosition.transform.position;
    }

    public virtual int ManhattanDistance(int partNum1, int partNum2)
    {
        return Mathf.Abs(partNum1 / width - partNum2 / width)
            + Mathf.Abs(partNum1 % width - partNum2 % width);
    }

    public virtual bool CheckPartExisting(int partNum, DirectionClass.DirectionEnum dir)
    {
        switch (dir)
        {
            case DirectionClass.DirectionEnum.Forward:
                return (partNum < (height - 1) * width);

            case DirectionClass.DirectionEnum.Right:
                return (partNum + 1) % width != 0;

            case DirectionClass.DirectionEnum.Back:
                return (partNum >= width);

            case DirectionClass.DirectionEnum.Left:
                return partNum % width != 0;
        }

        return false;
    }

    public bool CheckPathExisting(int partNum, DirectionClass.DirectionEnum direction)
    {
        if (partNum < 0 || boardParts.Length < partNum)
        {
            return false;
        }

        // パーツ間に壁がないかチェック

        return true;
    }

    public bool CheckObjectExisting(int partNum)
    {
        if (partNum < 0 || boardParts.Length < partNum)
        {
            return false;
        }

        return boardParts[partNum].HavingObject != null;
    }

    public int GetNextPartNumber(int partNum, DirectionClass.DirectionEnum direction)
    {
        if (partNum < 0 || boardParts.Length < partNum || CheckPartExisting(partNum, direction) == false)
        {
            return -1;
        }

        switch (direction)
        {
            case DirectionClass.DirectionEnum.Forward:
                return partNum + width;

            case DirectionClass.DirectionEnum.Right:
                return partNum + 1;

            case DirectionClass.DirectionEnum.Back:
                return partNum - width;

            case DirectionClass.DirectionEnum.Left:
                return partNum - 1;
        }

        return -1;
    }

    public int GetEventID(int partNum)
    {
        if (partNum < 0 || boardParts.Length < partNum)
        {
            return -1;
        }

        return boardParts[partNum].HavingEventID;
    }

    public virtual void Initialize(int w = 11, int h = 14)
    {
        width = w;
        height = h;
        basePosition = transform.Find("BasePosition").gameObject;
        boardParts = new BoardPart[width * height];

        for (int i = 0; i < height * width; i++)
        {
            boardParts[i].Number = i;
        }
    }

    public virtual void PutOnBoard(int partNum, IObjectOnBoard objectOnBoard)
    {
        if (partNum < 0 || boardParts.Length < partNum)
        {
            return;
        }

        if (boardParts[partNum].HavingObject == null && objectsOnBoard.Contains(objectOnBoard) == false)
        {
            boardParts[partNum].HavingObject = objectOnBoard;
            objectsOnBoard.Add(objectOnBoard);
        }
    }

    public virtual void MoveObjectOnBoard(IObjectOnBoard objectOnBoard, int nextNum)
    {
        boardParts[objectOnBoard.positionNum].HavingObject = null;
        boardParts[nextNum].HavingObject = objectOnBoard;
    }
}
