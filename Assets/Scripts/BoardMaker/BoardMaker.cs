using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

// ï«Çå¯ó¶âªÇµÇΩÉ{Å[ÉhÇçÏê¨Ç∑ÇÈä÷êîÇçÏÇÈ
public class BoardMaker : MonoBehaviour
{
    [SerializeField] int width = 11;
    [SerializeField] int height = 14;

    [SerializeField] float PartSize = 1.0f;
    [SerializeField] string mapName = "";
    int boardLength;

    List<BoardPart> boardParts = new List<BoardPart>();
    GameObject basePosition;

    public virtual Vector3 GetPartPosition(int partNum)
    {
        return new Vector3((partNum % width + 0.5f) * PartSize, 0f, (partNum / width + 0.5f) * PartSize) + basePosition.transform.position;
    }

    public virtual void GetPartPosition(int partNum, ref Vector3 dest)
    {
        dest.Set((partNum % width + 0.5f) * PartSize, 0f, (partNum / width + 0.5f) * PartSize);
        dest += basePosition.transform.position;
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

    public void OnPressInitializeButton()
    {
        Transform wallBase = transform.Find("wallBase");

        if (wallBase != null)
        {
            DestroyImmediate(wallBase.gameObject);
        }

        if (mapName == null || mapName == "")
        {
            InitializeDefault();
        }
        else
        {
            Initialize();
        }
    }

    public virtual void Initialize()
    {
        basePosition = transform.Find("BasePosition").gameObject;
        LoadBoard();
        GenerateWalls();
    }

    public void InitializeDefault()
    {
        basePosition = transform.Find("BasePosition").gameObject;

        Resize(width, height);

        for (int i = 0; i < boardLength; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                boardParts[i].LinkedPartNumbers[j] = -1;
            }
        }

        GenerateWalls();
    }

    void GenerateWalls()
    {
        GameObject parent = new GameObject("wallBase");
        parent.transform.parent = transform;

        GameObject wall = GenerateWall(0, width - 1, true, parent.transform);
        Vector3 tmp = wall.transform.position;
        tmp.z = basePosition.transform.position.z;
        wall.transform.position = tmp;
        wall.GetComponent<WallConfig>().enabled = false;

        wall = GenerateWall(0, width - 1, true, parent.transform);
        tmp.z += height * PartSize;
        wall.transform.position = tmp;
        wall.GetComponent<WallConfig>().enabled = false;

        wall = GenerateWall(0, width * (height - 1), false, parent.transform);
        tmp = wall.transform.position;
        tmp.x = basePosition.transform.position.x;
        wall.transform.position = tmp;
        wall.GetComponent<WallConfig>().enabled = false;

        wall = GenerateWall(0, width * (height - 1), false, parent.transform);
        tmp.x += width * PartSize;
        wall.transform.position = tmp;
        wall.GetComponent<WallConfig>().enabled = false;

        for (int i = 0; i < height - 1; i++)
        {
            for (int j = 0; j < width; j++)
            {
                int num = i * width + j;
                if (boardParts[num].LinkedPartNumbers[0] == -1)
                {
                    wall = GenerateWall(num, num, true, parent.transform);
                    wall.GetComponent<WallConfig>().BackOrLeftPart = num;
                    wall.GetComponent<WallConfig>().ForwardOrRightPart = num + width;
                    wall.GetComponent<WallConfig>().IsHorizon = true;
                }
            }
        }

        for (int i = 0; i < width - 1; i++)
        {
            for (int j = 0; j < height; j++)
            {
                int num = i + j * width;
                if (boardParts[num].LinkedPartNumbers[1] == -1)
                {
                    wall = GenerateWall(num, num, false, parent.transform);
                    wall.GetComponent<WallConfig>().BackOrLeftPart = num;
                    wall.GetComponent<WallConfig>().ForwardOrRightPart = num + 1;
                    wall.GetComponent<WallConfig>().IsHorizon = false;
                }
            }
        }
    }

    GameObject GenerateWall(int startNum, int endNum, bool isHorizon, Transform parent)
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
        wall.transform.parent = parent;
        wall.AddComponent<WallConfig>();

        return wall;
    }

    void GenerateWallsOptimized()
    {
        GameObject wall = GenerateWallOptimized(0, width - 1, true);
        Vector3 tmp = wall.transform.position;
        tmp.z = basePosition.transform.position.z;
        wall.transform.position = tmp;

        wall = GenerateWallOptimized(0, width - 1, true);
        tmp.z += height * PartSize;
        wall.transform.position = tmp;

        wall = GenerateWallOptimized(0, width * (height - 1), false);
        tmp = wall.transform.position;
        tmp.x = basePosition.transform.position.x;
        wall.transform.position = tmp;

        wall = GenerateWallOptimized(0, width * (height - 1), false);
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
                        GenerateWallOptimized(startWall, num - 1, true);
                        startWall = -1;
                    }
                }
            }

            if (startWall != -1)
            {
                GenerateWallOptimized(startWall, (i + 1) * width - 1, true);
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
                        GenerateWallOptimized(startWall, num - width, false);
                        startWall = -1;
                    }
                }
            }

            if (startWall != -1)
            {
                GenerateWallOptimized(startWall, width * (height - 1) + i, false);
            }
        }
    }

    GameObject GenerateWallOptimized(int startNum, int endNum, bool isHorizon)
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

    public void LoadBoard()
    {
        StreamReader reader = null;

        string dataPath = Application.dataPath + "/Scripts/BoardMaker/" + mapName + ".json";

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

    public void SaveBoard()
    {
        Transform wallBase = transform.Find("wallBase");

        if (wallBase != null)
        {
            WallConfig[] wallConfigs = wallBase.GetComponentsInChildren<WallConfig>(false);

            foreach (WallConfig wallConfig in wallConfigs)
            {
                if (wallConfig.IsActive == false)
                {
                    if (wallConfig.IsHorizon)
                    {
                        boardParts[wallConfig.BackOrLeftPart].LinkedPartNumbers[0] = wallConfig.ForwardOrRightPart;
                        boardParts[wallConfig.ForwardOrRightPart].LinkedPartNumbers[2] = wallConfig.BackOrLeftPart;
                    }
                    else
                    {
                        boardParts[wallConfig.BackOrLeftPart].LinkedPartNumbers[1] = wallConfig.ForwardOrRightPart;
                        boardParts[wallConfig.ForwardOrRightPart].LinkedPartNumbers[3] = wallConfig.BackOrLeftPart;
                    }
                    
                    continue;
                }

                switch (wallConfig.wallType)
                {
                    default:
                        if (wallConfig.IsHorizon)
                        {
                            boardParts[wallConfig.BackOrLeftPart].LinkedPartNumbers[0] = -1;
                            boardParts[wallConfig.ForwardOrRightPart].LinkedPartNumbers[2] = -1;
                        }
                        else
                        {
                            boardParts[wallConfig.BackOrLeftPart].LinkedPartNumbers[1] = -1;
                            boardParts[wallConfig.ForwardOrRightPart].LinkedPartNumbers[3] = -1;
                        }
                        break;
                }
            }
        }

        StreamWriter writer = null;

        string dataPath;

        if (mapName == null || mapName == "")
        {
            dataPath = Application.dataPath + "/Scripts/BoardMaker/test.json";
        }
        else
        {
            dataPath = Application.dataPath + "/Scripts/BoardMaker/" + mapName + ".json";
        }

        try
        {
            writer = new StreamWriter(dataPath, false);
            writer.WriteLine(JsonUtility.ToJson(new BoardSize(width, height)));

            for (int i = 0; i < boardLength; i++)
            {
                BoardPartJson boardPartJson = new BoardPartJson();
                boardPartJson.FromBoardPart(boardParts[i]);

                writer.WriteLine(JsonUtility.ToJson(boardPartJson));
            }
        }
        finally
        {
            if (writer != null)
            {
                writer.Close();
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        OnPressInitializeButton();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
