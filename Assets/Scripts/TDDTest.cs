using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDDTest : MonoBehaviour
{
    [SerializeField] FourDirectionInputManager directionButton;
    [SerializeField] PlayerOnBoard player;
    [SerializeField] Board board;
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
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(board.CheckObjectExisting(3));
    }
}
