using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//分ける意味あった？

//不変、単一情報
public interface IGameEvent
{
    public void OnHappeningEvent(IGameEventConfig config);
}

//可変、複数情報、アップキャストの使用を想定
public interface IGameEventConfig
{
    public bool IsActive { get; set; }
    public int EventID { get; }
}

public class GameEventManager
{
    readonly Dictionary<int, IGameEvent> gameEvents = new Dictionary<int, IGameEvent>();

    public void RegistBoardEvent(int num, IGameEvent boardEvent)
    {
        gameEvents.Add(num, boardEvent);
    }

    public void HappenGameEvent(IGameEventConfig eventConfig)
    {
        if (eventConfig.IsActive)
        {
            gameEvents.TryGetValue(eventConfig.EventID, out IGameEvent boardEvent);
            boardEvent?.OnHappeningEvent(eventConfig);
        }
    }
}
