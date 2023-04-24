using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover
{
    static readonly TimerManager.TimerUpdateEventHandler OnMoveUpdate = MoveUpdate;
    static readonly TimerManager.TimerOnceEventHandler OnMoveEnd = MoveEnd;

    static readonly Dictionary<int, MoverConfig> moverConfigs = new Dictionary<int, MoverConfig>();

    struct MoverConfig
    {
        public GameObject moveObject;
        public Vector3 startPosition;
        public Vector3 destPosition;
        public MyMathf.CubicEasingType easingType;
        public int moveTimerID;
        public float moveTime;
    }

    private Mover() { }

    public static int StartMove(GameObject target, Vector3 dest, float moveTime, MyMathf.CubicEasingType easingType = MyMathf.CubicEasingType.None)
    {
        MoverConfig moverConfig = new MoverConfig();
        moverConfig.moveObject = target;
        moverConfig.easingType = easingType;
        moverConfig.startPosition = target.transform.position;
        moverConfig.destPosition = dest;
        moverConfig.moveTime = moveTime;
        moverConfig.moveTimerID = TimerManager.Instance.CreateTimer(moveTime, 0.0f, null, OnMoveUpdate, OnMoveEnd);

        moverConfigs.Add(moverConfig.moveTimerID, moverConfig);

        return moverConfig.moveTimerID;
    }

    public static bool IsMoving(int moverID)
    {
        return moverConfigs.ContainsKey(moverID);
    }

    public static void ClearAll()
    {
        foreach (int moverID in moverConfigs.Keys)
        {
            TimerManager.Instance.DeleteTimer(moverID);
        }
        moverConfigs.Clear();
    }

    public static void EndMove(int timerID, bool isEndThere = false)
    {
        if (moverConfigs.ContainsKey(timerID) == false)
        {
            return;
        }

        if (isEndThere)
        {
            TimerManager.Instance.DeleteTimer(timerID);
        }
        else
        {
            TimerManager.Instance.EndTimer(timerID);
        }
        moverConfigs.Remove(timerID);
    }

    public static void StopMove(int timerID)
    {
        if (moverConfigs.ContainsKey(timerID) == false)
        {
            return;
        }

        TimerManager.Instance.StopTimer(timerID);
    }

    public static void RestartMove(int timerID)
    {
        if (moverConfigs.ContainsKey(timerID) == false)
        {
            return;
        }

        TimerManager.Instance.RestartTimer(timerID);
    }

    //public static void CancelMove(int timerID)
    //{
    //    moverConfigs.TryGetValue(timerID, out MoverConfig moverConfig);

    //    if (moverConfig.moveTimerID != timerID)
    //    {
    //        return;
    //    }

    //    TimerManager.Instance.DeleteTimer(timerID);
    //    moverConfig.moveObject.transform.position = moverConfig.startPosition;
    //    moverConfigs.Remove(timerID);
    //}

    static void MoveUpdate(int timerID, float elapsedTime, float dt)
    {
        moverConfigs.TryGetValue(timerID, out MoverConfig moverConfig);
        float t = elapsedTime / moverConfig.moveTime;
        moverConfig.moveObject.transform.position = Vector3.Lerp(moverConfig.startPosition, moverConfig.destPosition, MyMathf.CubicEasing(t, moverConfig.easingType));
    }

    static void MoveEnd(int timerID)
    {
        moverConfigs.TryGetValue(timerID, out MoverConfig moverConfig);
        moverConfig.moveObject.transform.position = moverConfig.destPosition;
        moverConfigs.Remove(timerID);
    }
}
