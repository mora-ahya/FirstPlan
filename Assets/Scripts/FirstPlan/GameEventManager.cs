using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�s�ρA�P����
public interface IGameEvent
{
    public void OnHappeningEvent(IGameEventConfig config);
}

//�ρA�������A�A�b�v�L���X�g�̎g�p��z��
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
