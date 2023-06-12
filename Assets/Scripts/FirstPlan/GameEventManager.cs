using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//不変、単一情報
public interface IGameEvent
{
    public void OnHappeningEvent(IGameEventConfig config);
}

//可変、複数情報、アップキャストの使用を想定
public interface IGameEventConfig
{
    public bool IsActive { get; }
    public int EventID { get; }
}

public class GameEventManager
{
    readonly Dictionary<int, IGameEvent> boardEvents = new Dictionary<int, IGameEvent>();

    public void RegistBoardEvent(int num, IGameEvent boardEvent)
    {
        boardEvents.Add(num, boardEvent);
    }

    public void HappenBoardEvent(IGameEventConfig eventConfig)
    {
        boardEvents.TryGetValue(eventConfig.EventID, out IGameEvent boardEvent);
        boardEvent?.OnHappeningEvent(eventConfig);
    }
}
