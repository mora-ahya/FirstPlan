using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 平面のボードを位置マスずつ進めるプレイヤー、InitSet(もしくはOnBoardSet)入りを検討

public class PlayerOnBoard : MonoBehaviour, IFourDirectionInputReceiver, IObjectOnBoard
{
    public DirectionClass.DirectionEnum Direction { get; private set; }

    public GameObject SelfGameObject => gameObject;
    public int PositionNum { get; private set; }

    public bool OnHappendBoardEvent(int boardEventID)
    {
        return true;
    }

    int moverID;
    int rotatorID;
    TimerManager.TimerOnceEventHandler OnMoveEnd;

    // Start is called before the first frame update
    void Start()
    {
        SetForward(DirectionClass.DirectionEnum.Forward);
        OnMoveEnd = MoveEnd;
    }

    public void Initialize(Board board, int partNum)
    {
        board.PutOnBoard(partNum, this);
        PositionNum = partNum;
        moverID = Mover.StartMove(gameObject, board.GetPartPosition(PositionNum), 0.1f);
    }

    public void StartControl(FourDirectionInputManager directionButton)
    {
        directionButton.SetReceiver(this);
    }

    public void OnPressDirectionButton(int dir)
    {
        switch (DirectionClass.DirectionEnum.Forward + dir)
        {
            case DirectionClass.DirectionEnum.Forward:
                MoveInDirection(Direction);
                break;
            case DirectionClass.DirectionEnum.Right:
                Turn(false);
                break;
            case DirectionClass.DirectionEnum.Back:
                DirectionClass.DirectionEnum d = DirectionClass.RotateDirectionEnum(Direction, 1);
                d = DirectionClass.RotateDirectionEnum(d, 1);
                MoveInDirection(d);
                break;
            case DirectionClass.DirectionEnum.Left:
                Turn(true);
                break;
        }
    }

    public void SetForward(DirectionClass.DirectionEnum direction)
    {
        Direction = direction;
        transform.forward = DirectionClass.ConvertDirectionEnumToVector(direction);
    }

    void MoveInDirection(DirectionClass.DirectionEnum direction)
    {
        Mover.EndMove(moverID);
        Rotator.EndRotate(rotatorID);

        Board board = BoardManager.Instance.CurrentBoard;

        int nextPosNum = board.MoveObject(this, direction);

        if (nextPosNum < 0)
        {
            return;
        }

        PositionNum = nextPosNum;
        moverID = Mover.StartMove(gameObject, board.GetPartPosition(PositionNum), 0.1f, MyMathf.CubicEasingType.None, OnMoveEnd);
    }

    void MoveEnd(int timerID)
    {
        Board board = BoardManager.Instance.CurrentBoard;
        int eventID = board.GetEventID(PositionNum);
        
    }

    public void Turn(bool isLeft)
    {
        Mover.EndMove(moverID);
        Rotator.EndRotate(rotatorID);
        int tmp = isLeft ? -1 : 1;
        Direction = DirectionClass.RotateDirectionEnum(Direction, tmp);
        rotatorID = Rotator.StartRotate(gameObject, new Vector3(0.0f, 90.0f * tmp, 0.0f), 0.1f);
    }
}
