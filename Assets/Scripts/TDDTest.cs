using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TDDTest : MonoBehaviour, IMenuFrameHolder
{
    [SerializeField] FourDirectionInputManager directionButton;
    [SerializeField] PlayerOnBoard player;
    [SerializeField] Board board;
    [SerializeField] MenuFrame menuFrame;
    // Start is called before the first frame update
    void Start()
    {
        if (directionButton != null)
        {
            player?.StartControl(directionButton);
        }

        board.Initialize(Application.dataPath + "/MyResource/StageConfigs" + "/stage1.json");
        player.Initialize(board, 0);

        BoardManager.Instance.SetBoard(board);
        menuFrame.Initialize(1, 5);
        menuFrame.SetMenuFrameHolder(this);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(board.CheckObjectExisting(3));
        //menuFrame.ScrollElements(true);
    }

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
