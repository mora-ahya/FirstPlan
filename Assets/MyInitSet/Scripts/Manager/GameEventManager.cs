using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//������Ӗ��������H

//�s�ρA�P����
public interface IGameEvent
{
    public void OnHappeningEvent(IGameEventConfig config);
}

//�ρA�������A�A�b�v�L���X�g�̎g�p��z��
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
