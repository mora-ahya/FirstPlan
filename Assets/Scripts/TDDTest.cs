using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TDDTest : MonoBehaviour, IMenuFrameHolder
{
    public static TDDTest Instance { get; private set; }

    public FPGameScene GameScene => gameScene;

    [SerializeField] FourDirectionInputManager directionButton;
    [SerializeField] PlayerOnBoard player;
    [SerializeField] Board board;
    [SerializeField] MenuFrame menuFrame;
    [SerializeField] FPBattleEnemy enemy;
    [SerializeField] FPBattlePlayer player2;
    [SerializeField] FPBattleEventConfig battleEventConfig;
    [SerializeField] FPGameScene gameScene;
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        board.Initialize(Application.dataPath + "/MyResource/StageConfigs" + "/stage1.json");
        player.Initialize(board, 0);

        BoardManager.Instance.SetBoard(board);

        player2.Initialize();

        //FPBattleManager.Instance.SetEnemy(enemy);
        //FPBattleManager.Instance.StartBattle();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            gameScene.StartBattleEvent(battleEventConfig);
        }
        //Debug.Log(board.CheckObjectExisting(3));
        //menuFrame.ScrollElements(true);
    }

    public int NumOfContent { get; private set; } = 36;
    public void SetContent(int num, GameObject gameObject)
    {
        TestMenuElement tmp = gameObject.GetComponent<TestMenuElement>();

        if (tmp == null)
        {
            return;
        }

        tmp.SetText(num.ToString());
    }
    public void OnClicked(int num, PointerEventData eventData)
    {
        Debug.Log(num);
    }
}
