using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    static readonly TimerManager.TimerUpdateEventHandler OnRotateUpdate = RotateUpdate;
    static readonly TimerManager.TimerOnceEventHandler OnRotateEnd = RotateEnd;

    static readonly Dictionary<int, RotatorConfig> rotatorConfigs = new Dictionary<int, RotatorConfig>();

    struct RotatorConfig
    {
        public GameObject rotateObject;
        public Vector3 startEulerAngles;
        public Vector3 endEulerAngles;
        public int rotateTimerID;
        public float rotateTime;
        public bool isLocal;
    }

    private Rotator() { }

    public static int StartRotate(GameObject target, Vector3 rotAmo, float rotateTime = 0.0f, bool isLocal = false)
    {
        RotatorConfig rotatorConfig = new RotatorConfig();
        rotatorConfig.rotateObject = target;
        rotatorConfig.startEulerAngles = isLocal ? target.transform.localEulerAngles : target.transform.eulerAngles;
        rotatorConfig.endEulerAngles = rotatorConfig.startEulerAngles + rotAmo;
        rotatorConfig.rotateTime = rotateTime;
        rotatorConfig.isLocal = isLocal;
        rotatorConfig.rotateTimerID = TimerManager.Instance.CreateTimer(rotateTime, 0.0f, null, OnRotateUpdate, OnRotateEnd);

        rotatorConfigs.Add(rotatorConfig.rotateTimerID, rotatorConfig);

        return rotatorConfig.rotateTimerID;
    }

    public static bool IsRotating(int rotatorID)
    {
        return rotatorConfigs.ContainsKey(rotatorID);
    }

    public static void ClearAll()
    {
        foreach (int rotatorID in rotatorConfigs.Keys)
        {
            TimerManager.Instance.DeleteTimer(rotatorID);
        }
        rotatorConfigs.Clear();
    }

    public static void EndRotate(int timerID, bool isEndThere = false)
    {
        if (rotatorConfigs.ContainsKey(timerID) == false)
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
        rotatorConfigs.Remove(timerID);
    }

    public static void StopRotate(int timerID)
    {
        if (rotatorConfigs.ContainsKey(timerID) == false)
        {
            return;
        }

        TimerManager.Instance.StopTimer(timerID);
    }

    public static void RestartRotate(int timerID)
    {
        if (rotatorConfigs.ContainsKey(timerID) == false)
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

    static void RotateUpdate(int timerID, float elapsedTime, float dt)
    {
        rotatorConfigs.TryGetValue(timerID, out RotatorConfig rotatorConfig);
        float t = elapsedTime / rotatorConfig.rotateTime;
        
        if (rotatorConfig.isLocal)
        {
            rotatorConfig.rotateObject.transform.localEulerAngles = Vector3.Lerp(rotatorConfig.startEulerAngles, rotatorConfig.endEulerAngles, t);
        }
        else
        {
            rotatorConfig.rotateObject.transform.eulerAngles = Vector3.Lerp(rotatorConfig.startEulerAngles, rotatorConfig.endEulerAngles, t);
        }
    }

    static void RotateEnd(int timerID)
    {
        rotatorConfigs.TryGetValue(timerID, out RotatorConfig rotatorConfig);
        if (rotatorConfig.isLocal)
        {
            rotatorConfig.rotateObject.transform.localEulerAngles = rotatorConfig.endEulerAngles;
        }
        else
        {
            rotatorConfig.rotateObject.transform.eulerAngles = rotatorConfig.endEulerAngles;
        }
        rotatorConfigs.Remove(timerID);
    }
}
