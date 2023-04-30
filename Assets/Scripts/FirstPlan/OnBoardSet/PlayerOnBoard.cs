using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 平面のボードを位置マスずつ進めるプレイヤー、InitSet(もしくはOnBoardSet)入りを検討

public class PlayerOnBoard : MonoBehaviour, IFourDirectionInputReceiver, IObjectOnBoard
{
    public DirectionClass.DirectionEnum Direction { get; private set; }

    public GameObject SelfGameObject => gameObject;
    public int positionNum { get; set; }

    public void OnHappendBoardEvent(int boardEventID)
    {

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
        positionNum = partNum;
        moverID = Mover.StartMove(gameObject, board.PartNumberToWorldPosition(positionNum), 0.1f);
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
        int nextPosNum = board.GetNextPartNumber(positionNum, direction);

        if (nextPosNum < 0)
        {
            return;
        }

        board.MoveObjectOnBoard(this, nextPosNum);
        positionNum = nextPosNum;

        moverID = Mover.StartMove(gameObject, board.PartNumberToWorldPosition(positionNum), 0.1f, MyMathf.CubicEasingType.None, OnMoveEnd);
    }

    void MoveEnd(int timerID)
    {
        Board board = BoardManager.Instance.CurrentBoard;
        int eventID = board.GetEventID(positionNum);
        
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
