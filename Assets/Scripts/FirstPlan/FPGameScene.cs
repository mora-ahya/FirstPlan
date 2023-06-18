using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFPGameSceneChild : MyInitSet.ISceneChild<FPGameScene>
{

}

public class FPGameScene : MyInitSet.MySceneBase, IFourDirectionInputReceiver
{
    readonly GameEventManager gameEventManager = new GameEventManager();

    public override int SceneKind { get; } = (int)FPSceneKind.GameScene;

    [SerializeField] FourDirectionInputManager directionButton;
    [SerializeField] PlayerOnBoard player;
    [SerializeField] FPBattleManager fPBattleManager;

    TimerManager.TimerOnceEventHandler onMovePlayerEnd;

    protected override void OnAwake()
    {
        onMovePlayerEnd = MovePlayerEnd;

        gameEventManager.RegistBoardEvent((int)FPGameEventKind.Battle, new FPBattleEvent(this));

        directionButton.SetReceiver(this);
        SetUpChildren(this);

        FPBattleCharacter.SetBattleManager(fPBattleManager);
    }

    public void HappenGameEvent(IGameEventConfig eventConfig)
    {
        gameEventManager.HappenGameEvent(eventConfig);
    }

    public void StartBattleEvent(FPBattleEventConfig eventConfig)
    {
        directionButton.gameObject.SetActive(false);
        fPBattleManager.StartBattle(eventConfig);
    }

    public void EndBattleEvent()
    {
        directionButton.gameObject.SetActive(true);
    }

    void MovePlayerOnBoard(DirectionClass.DirectionEnum dir)
    {
        Board board = BoardManager.Instance.CurrentBoard;

        int nextPosNum = board.MoveObject(player, dir);

        if (nextPosNum < 0)
        {
            return;
        }

        player.PositionNum = nextPosNum;
        player.Move(board.GetPartPosition(nextPosNum), onMovePlayerEnd);
    }

    void MovePlayerEnd(int timerId)
    {
        Board board = BoardManager.Instance.CurrentBoard;
        IGameEventConfig eventConfig = board.GetGameEventByPartNumber(player.PositionNum);

        if (eventConfig != null)
        {
            gameEventManager.HappenGameEvent(eventConfig);
        }
    }

    #region IFourDirectionInputReceiver Implement
    public void OnPressDirectionButton(int dir)
    {
        switch (DirectionClass.DirectionEnum.Forward + dir)
        {
            case DirectionClass.DirectionEnum.Forward:
                MovePlayerOnBoard(player.Direction);
                break;
            case DirectionClass.DirectionEnum.Right:
                player.Turn(false);
                break;
            case DirectionClass.DirectionEnum.Back:
                DirectionClass.DirectionEnum d = DirectionClass.RotateDirectionEnum(player.Direction, 1);
                d = DirectionClass.RotateDirectionEnum(d, 1);
                MovePlayerOnBoard(d);
                break;
            case DirectionClass.DirectionEnum.Left:
                player.Turn(true);
                break;
        }
    }
    #endregion
}
