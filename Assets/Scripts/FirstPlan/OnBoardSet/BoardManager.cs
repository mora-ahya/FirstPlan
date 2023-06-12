using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionClass
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
        switch (direction)
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
}

public interface IBoardManager
{

}

public class BoardManager : MonoBehaviour//, IFourDirectionInputReceiver
{
    public static BoardManager Instance { get; private set; }

    public Board CurrentBoard { get; private set; }

    PlayerOnBoard controllingCharacter;

    void Awake()
    {
        Instance = this;
    }

    public void SetBoard(Board board)
    {
        CurrentBoard = board;
    }

    //public virtual void OnPressDirectionButton(int dir)
    //{

    //}
}
