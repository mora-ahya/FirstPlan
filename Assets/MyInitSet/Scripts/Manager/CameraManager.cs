using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour, IManagerBase
{
    enum CameraMode
    {
        Free,
        Follow,
    }

    public static CameraManager Instance { get; private set; } = default;
    public int ActPriority { get; } = -1024;
    public GameObject CameraObject => cameraObject;

    [SerializeField] GameObject cameraObject = null;

    int moverID;

    public void AwakeInitialize()
    {
        if (cameraObject == null)
        {
            cameraObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        }

        Instance = this;
    }

    public void LateAwakeInitialize()
    {

    }

    public void Act()
    {

    }

    public void SetPosition(Vector3 position)
    {
        cameraObject.transform.position = position;
    }

    public Vector3 GetPosition()
    {
        return cameraObject.transform.position;
    }

    public void MovePosition(Vector3 position, float moveTime, MyMathf.CubicEasingType easingType = MyMathf.CubicEasingType.None)
    {
        if (Mover.IsMoving(moverID))
        {
            return;
        }

        if (Mathf.Approximately(moveTime, 0.0f))
        {
            SetPosition(position);
            return;
        }

        moverID = Mover.StartMove(cameraObject, position, moveTime, easingType);
    }

    public void StopMove(bool isCancel = false)
    {
        if (Mover.IsMoving(moverID) == false)
        {
            return;
        }

        Mover.EndMove(moverID, isCancel);
    }
}
