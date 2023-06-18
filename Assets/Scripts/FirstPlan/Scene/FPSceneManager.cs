using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FPSceneKind
{
    GameScene,
}

public class FPSceneManager : MyInitSet.MySceneManager
{
    public static FPSceneManager Instance { get; protected set; }

    protected override void OnAwake()
    {
        Instance = this;
    }
}
