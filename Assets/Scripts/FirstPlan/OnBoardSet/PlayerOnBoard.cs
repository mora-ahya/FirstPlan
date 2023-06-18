using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 平面のボードを1マスずつ進めるプレイヤー、InitSet(もしくはOnBoardSet)入りを検討

public class PlayerOnBoard : MonoBehaviour, IObjectOnBoard
{
    public DirectionClass.DirectionEnum Direction { get; private set; }

    public GameObject SelfGameObject => gameObject;
    public int PositionNum { get; set; }

    int moverID;
    int rotatorID;

    // Start is called before the first frame update
    void Start()
    {
        SetForward(DirectionClass.DirectionEnum.Forward);
    }

    public void Initialize(Board board, int partNum)
    {
        board.PutOnBoard(partNum, this);
        PositionNum = partNum;
        moverID = Mover.StartMove(gameObject, board.GetPartPosition(PositionNum), 0.1f);
    }

    public void SetForward(DirectionClass.DirectionEnum direction)
    {
        Direction = direction;
        transform.forward = DirectionClass.ConvertDirectionEnumToVector(direction);
    }

    public void Move(Vector3 dest, TimerManager.TimerOnceEventHandler onMoveEnd = null)
    {
        Mover.EndMove(moverID);
        Rotator.EndRotate(rotatorID);

        moverID = Mover.StartMove(gameObject, dest, 0.1f, MyMathf.CubicEasingType.None, onMoveEnd);
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
