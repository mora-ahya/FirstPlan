using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.IO;

public interface IObjectOnBoard
{
    public GameObject SelfGameObject { get; }
    public DirectionClass.DirectionEnum Direction { get; }
    public int PositionNum { get; set; }
}

class BoardPart
{
    public int Number;
    public int HavingEventID;
    public IObjectOnBoard HavingObject;
    public int[] LinkedPartNumbers = new int[4];
}

// 使いまわしは考えない
// gameObjectの名前をそのまま、jsonのファイル名にする
public class Board : MonoBehaviour
{
    float partSize = 1.0f;
    int width;
    int height;
    int boardLength;

    readonly List<BoardPart> boardParts = new List<BoardPart>();
    GameObject basePosition;

    readonly List<IObjectOnBoard> objectsOnBoard = new List<IObjectOnBoard>();

    readonly Dictionary<int, IGameEventConfig> boardEvents = new Dictionary<int, IGameEventConfig>();

    public void Initialize(string dataPath)
    {
        basePosition = transform.Find("BasePosition").gameObject;
        LoadBoard(dataPath);

        Transform eventObjectParent = basePosition.transform.Find("EventObjectParent");

        for (int i = 0; i < eventObjectParent.childCount; i++)
        {
            GameObject gObject = eventObjectParent.GetChild(i).gameObject;
            IGameEventConfig gameEventConfig = gObject.GetComponent<IGameEventConfig>();
            if (gameEventConfig != null)
            {
                boardEvents.Add(GetPartNumFromLocalPosition(gObject.transform.localPosition), gameEventConfig);
            }
        }
    }

    public void InitializeDefault()
    {
        basePosition = transform.Find("BasePosition").gameObject;

        SetSize(11, 14);

        for (int i = 0; i < boardLength; i++)
        {
            BoardPart tmp = new BoardPart();
            tmp.Number = i;
            tmp.HavingEventID = -1;

            for (int j = 0; j < 4; j++)
            {
                boardParts[i].LinkedPartNumbers[j] = GetNextPartNumber(i, DirectionClass.DirectionEnum.Forward + j);
            }

            boardParts.Add(tmp);
        }
    }

    void SetSize(int w = 11, int h = 14)
    {
        if (w == 0)
        {
            w = 1;
        }
        if (h == 0)
        {
            h = 1;
        }

        width = w;
        height = h;

        boardLength = width * height;
        boardParts.Capacity = boardLength;
    }

    public virtual Vector3 GetPartPosition(int partNum)
    {
        return new Vector3((partNum % width + 0.5f) * partSize, 0f, (partNum / width + 0.5f) * partSize) + basePosition.transform.position;
    }

    public virtual void GetPartPosition(int partNum, ref Vector3 dest)
    {
        dest.Set((partNum % width + 0.5f) * partSize, 0f, (partNum / width + 0.5f) * partSize);
        dest += basePosition.transform.position;
    }

    public virtual int GetPartNumFromWorldPosition(in Vector3 pos)
    {
        return GetPartNumFromLocalPosition(pos - basePosition.transform.position);
    }

    public virtual int GetPartNumFromLocalPosition(in Vector3 pos)
    {
        int w = (int)(pos.x / partSize);

        if (w > width)
        {
            return -1;
        }

        int h = (int)(pos.z / partSize);

        if (h > height)
        {
            return -1;
        }

        return w + h * width;
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
        if (partNum < 0 || width * height < partNum)
        {
            return false;
        }

        // パーツ間に壁がないかチェック

        return true;
    }

    public bool CheckObjectExisting(int partNum)
    {
        if (partNum < 0 || boardLength < partNum)
        {
            return false;
        }

        return boardParts[partNum].HavingObject != null;
    }

    public int GetNextPartNumber(int partNum, DirectionClass.DirectionEnum direction)
    {
        if (partNum < 0 || boardLength < partNum || CheckPartExisting(partNum, direction) == false)
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

    public IGameEventConfig GetGameEventByPartNumber(int partNum)
    {
        boardEvents.TryGetValue(partNum, out IGameEventConfig eventConfig);
        return eventConfig;
    }

    public virtual void PutOnBoard(int partNum, IObjectOnBoard objectOnBoard)
    {
        if (partNum < 0 || boardLength < partNum)
        {
            return;
        }

        if (boardParts[partNum].HavingObject == null && objectsOnBoard.Contains(objectOnBoard) == false)
        {
            boardParts[partNum].HavingObject = objectOnBoard;
            objectsOnBoard.Add(objectOnBoard);
        }
    }

    public virtual int MoveObject(IObjectOnBoard objectOnBoard, DirectionClass.DirectionEnum moveDir)
    {
        BoardPart tmp = boardParts[objectOnBoard.PositionNum];
        int nextPartNum = tmp.LinkedPartNumbers[(int)moveDir];

        if (nextPartNum >= 0)
        {
            tmp.HavingObject = null;
            boardParts[nextPartNum].HavingObject = objectOnBoard;
        }

        return nextPartNum;
    }

    public virtual int MoveObject(IObjectOnBoard objectOnBoard, int destSquare)
    {
        if (destSquare < 0 || boardLength < destSquare)
        {
            return -1;
        }

        BoardPart tmp = boardParts[objectOnBoard.PositionNum];
        tmp.HavingObject = null;
        boardParts[destSquare].HavingObject = objectOnBoard;

        return destSquare;
    }

    [System.Serializable]
    struct BoardSize
    {
        public int width;
        public int height;

        public BoardSize(int w = 11, int h = 14)
        {
            width = w;
            height = h;
        }
    }

    [System.Serializable]
    struct BoardPartJson
    {
        public int LinkedPartNumber0;
        public int LinkedPartNumber1;
        public int LinkedPartNumber2;
        public int LinkedPartNumber3;

        public int EventID0;

        public void ToBoardPart(BoardPart boardPart)
        {
            boardPart.LinkedPartNumbers[0] = LinkedPartNumber0;
            boardPart.LinkedPartNumbers[1] = LinkedPartNumber1;
            boardPart.LinkedPartNumbers[2] = LinkedPartNumber2;
            boardPart.LinkedPartNumbers[3] = LinkedPartNumber3;
            boardPart.HavingEventID = EventID0;
        }

        public void FromBoardPart(BoardPart boardPart)
        {
            LinkedPartNumber0 = boardPart.LinkedPartNumbers[0];
            LinkedPartNumber1 = boardPart.LinkedPartNumbers[1];
            LinkedPartNumber2 = boardPart.LinkedPartNumbers[2];
            LinkedPartNumber3 = boardPart.LinkedPartNumbers[3];
            EventID0 = boardPart.HavingEventID;
        }
    }

    public void LoadBoard(string dataPath)
    {
        StreamReader reader = null;

        try
        {
            reader = new StreamReader(dataPath);

            BoardSize boardSize = JsonUtility.FromJson<BoardSize>(reader.ReadLine());

            SetSize(boardSize.width, boardSize.height);

            for (int i = 0; i < boardLength; i++)
            {
                BoardPart tmp = new BoardPart();
                tmp.Number = i;

                BoardPartJson partTmp = JsonUtility.FromJson<BoardPartJson>(reader.ReadLine());
                partTmp.ToBoardPart(tmp);

                boardParts.Add(tmp);
            }
        }
        finally
        {
            if (reader != null)
            {
                reader.Close();
            }
        }
    }
}
