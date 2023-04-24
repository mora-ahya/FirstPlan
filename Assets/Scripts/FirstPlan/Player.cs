using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 平面のボードを位置マスずつ進めるプレイヤー、InitSet入りを検討

public class Player : MonoBehaviour
{
    public enum DirectionEnum
    {
        Forward,
        Right,
        Back,
        Left,
    }

    public static Vector3 ConvertDirectionEnumToVector(DirectionEnum direction)
    {
        switch(direction)
        {
            case DirectionEnum.Forward:
                return Vector3.forward;

            case DirectionEnum.Right:
                return Vector3.right;

            case DirectionEnum.Back:
                return Vector3.back;

            case DirectionEnum.Left:
                return Vector3.left;

            default:
                return Vector3.forward;
        }
    }

    public static DirectionEnum RotateDirectionEnum(DirectionEnum currentDirection, int rotateDirection)
    {
        if (currentDirection == DirectionEnum.Forward && rotateDirection < 0)
        {
            return DirectionEnum.Left;
        }
        else if (currentDirection == DirectionEnum.Left && rotateDirection > 0)
        {
            return DirectionEnum.Forward;
        }

        return currentDirection + rotateDirection;
        
    }

    public DirectionEnum Direction { get; private set; }

    int moverID;
    int rotatorID;

    // Start is called before the first frame update
    void Start()
    {
        SetForward(DirectionEnum.Forward);
    }

    public void SetForward(DirectionEnum direction)
    {
        Direction = direction;
        transform.forward = Player.ConvertDirectionEnumToVector(direction);
    }

    public void GoForward()
    {
        Mover.EndMove(moverID);
        Rotator.EndRotate(rotatorID);
        moverID = Mover.StartMove(gameObject, gameObject.transform.position + transform.forward * 1f, 0.1f);
    }

    public void GoBack()
    {
        Mover.EndMove(moverID);
        Rotator.EndRotate(rotatorID);
        moverID = Mover.StartMove(gameObject, transform.position - transform.forward * 1f, 0.1f);
    }

    public void Turn(bool isLeft)
    {
        if (Mover.IsMoving(moverID))
        {
            return;
        }

        Rotator.EndRotate(rotatorID);
        int tmp = isLeft ? -1 : 1;
        Direction = Player.RotateDirectionEnum(Direction, tmp);
        rotatorID = Rotator.StartRotate(gameObject, new Vector3(0.0f, 90.0f * tmp, 0.0f), 1.0f);
    }
}
