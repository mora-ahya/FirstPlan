﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.IO;

public interface IObjectOnBoard
{
    public GameObject SelfGameObject { get; }
    public DirectionClass.DirectionEnum Direction { get; }
    public int PositionNum { get;}

    public bool OnHappendBoardEvent(int boardEventID);
}

class BoardPart
{
    public int Number;
    public int HavingEventID;
    public IObjectOnBoard HavingObject;
    public int[] LinkedPartNumbers = new int[4];
}

public class Board : MonoBehaviour
{
    public float PartSize { get; } = 1.0f;
    int width;
    int height;
    int boardLength;

    List<BoardPart> boardParts = new List<BoardPart>();
    GameObject basePosition;

    List<IObjectOnBoard> objectsOnBoard = new List<IObjectOnBoard>();

    public virtual Vector3 GetPartPosition(int partNum)
    {
        return new Vector3((partNum % width + 0.5f) * PartSize, 0f, (partNum / width + 0.5f) * PartSize) + basePosition.transform.position;
    }

    public virtual void GetPartPosition(int partNum, ref Vector3 dest)
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

    public int GetEventID(int partNum)
    {
        if (partNum < 0 || boardLength < partNum)
        {
            return -1;
        }

        return boardParts[partNum].HavingEventID;
    }

    public virtual void Initialize(string dataPath)
    {
        basePosition = transform.Find("BasePosition").gameObject;
        LoadBoard(dataPath);
        GenerateWalls();
    }

    public void InitializeDefault()
    {
        basePosition = transform.Find("BasePosition").gameObject;

        Resize(11, 14);

        for (int i = 0; i < boardLength; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                boardParts[i].LinkedPartNumbers[j] = GetNextPartNumber(i, DirectionClass.DirectionEnum.Forward + j);
            }
        }

        GenerateWalls();
    }

    void GenerateWalls()
    {
        GameObject wall = GenerateWall(0, width - 1, true);
        Vector3 tmp = wall.transform.position;
        tmp.z = basePosition.transform.position.z;
        wall.transform.position = tmp;

        wall = GenerateWall(0, width - 1, true);
        tmp.z += height * PartSize;
        wall.transform.position = tmp;

        wall = GenerateWall(0, width * (height - 1), false);
        tmp = wall.transform.position;
        tmp.x = basePosition.transform.position.x;
        wall.transform.position = tmp;

        wall = GenerateWall(0, width * (height - 1), false);
        tmp.x += (width - 1) + PartSize;
        wall.transform.position = tmp;

        for (int i = 0; i < height - 1; i++)
        {
            int startWall = -1;
            for (int j = 0; j < width; j++)
            {
                int num = i * width + j;
                if (boardParts[num].LinkedPartNumbers[0] == -1)
                {
                    if (startWall == -1)
                    {
                        startWall = num;
                    }
                }
                else
                {
                    if (startWall != -1)
                    {
                        GenerateWall(startWall, num - 1, true);
                        startWall = -1;
                    }
                }
            }

            if (startWall != -1)
            {
                GenerateWall(startWall, (i + 1) * width - 1, true);
            }
        }

        for (int i = 0; i < width - 1; i++)
        {
            int startWall = -1;
            for (int j = 0; j < height; j++)
            {
                int num = i + j * width;
                if (boardParts[num].LinkedPartNumbers[1] == -1)
                {
                    if (startWall == -1)
                    {
                        startWall = num;
                    }
                }
                else
                {
                    if (startWall != -1)
                    {
                        GenerateWall(startWall, num - width, false);
                        startWall = -1;
                    }
                }
            }

            if (startWall != -1)
            {
                GenerateWall(startWall, width * (height - 1) + i, false);
            }
        }
    }

    GameObject GenerateWall(int startNum, int endNum, bool isHorizon)
    {
        Vector3 wallOffset;
        Vector3 wallScale;

        if (isHorizon)
        {
            wallOffset = new Vector3(0.0f, 0.5f, PartSize * 0.5f);
            wallScale = new Vector3(PartSize * (endNum - startNum + 1), 1.0f, PartSize / 10.0f);
        }
        else
        {
            wallOffset = new Vector3(PartSize * 0.5f, 0.5f, 0.0f);
            wallScale = new Vector3(PartSize / 10.0f, 1.0f, PartSize * ((endNum - startNum) / width + 1));
        }

        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.transform.position = (GetPartPosition(startNum) + GetPartPosition(endNum)) / 2.0f + wallOffset;
        wall.transform.localScale = wallScale;
        wall.transform.parent = transform;

        return wall;
    }

    public void Resize(int w = 11, int h = 14)
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

        if (boardParts.Capacity < boardLength)
        {
            boardParts.Capacity = boardLength;
        }

        for (int i = boardParts.Count; i < boardLength; i++)
        {
            BoardPart tmp = new BoardPart();
            tmp.Number = i;
            boardParts.Add(tmp);
        }
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
            Resize(boardSize.width, boardSize.height);

            for (int i = 0; i < boardLength; i++)
            {
                BoardPartJson partTmp = JsonUtility.FromJson<BoardPartJson>(reader.ReadLine());
                partTmp.ToBoardPart(boardParts[i]);
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
