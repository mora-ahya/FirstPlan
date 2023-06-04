using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface ITurnBasedBattlerBase
{ 
    public int BattlePriority { get; }
    public bool IsEndBattleProcess { get; }
    public void OnStartTurn();
    public void OnStartProcessingCommand();
    public void OnEndTurn();
}

public interface ITurnBasedBattleSystemEventReceiver
{
    public void OnEndAllBattleProcess();
}

public class TurnBasedBattleSystem : IManagerBase
{
    class TurnBasedBattlerComparer : IComparer<ITurnBasedBattlerBase>
    {
        public int Compare(ITurnBasedBattlerBase mb1, ITurnBasedBattlerBase mb2)
        {
            if (mb1.BattlePriority < mb2.BattlePriority)
            {
                return 1;
            }
            else if (mb1.BattlePriority > mb2.BattlePriority)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
    }

    static readonly TurnBasedBattlerComparer comparer = new TurnBasedBattlerComparer();

    public enum PhaseEnum
    {
        StartTurn,
        CommandProcessing,
        EndTurn,
    }

    public PhaseEnum CurrentPhase { get; private set; } = PhaseEnum.StartTurn;

    readonly List<ITurnBasedBattlerBase> turnBasedBattlers = new List<ITurnBasedBattlerBase>();
    readonly List<ITurnBasedBattlerBase> sortedBattlers = new List<ITurnBasedBattlerBase>();

    ITurnBasedBattleSystemEventReceiver eventReceiver;

    bool isProcessing = false;
    int currentBattlerNumber = 0;

    public void SetEventReceiver(ITurnBasedBattleSystemEventReceiver er)
    {
        eventReceiver = er;
    }

    public void AddTurnBasedBattler(ITurnBasedBattlerBase turnBasedBattler)
    {
        turnBasedBattlers.Add(turnBasedBattler);
        sortedBattlers.Add(turnBasedBattler);
    }

    public void ClearTurnBasedBattlers()
    {
        turnBasedBattlers.Clear();
        sortedBattlers.Clear();
    }

    public void StartBattleProcess()
    {
        isProcessing = true;
        CurrentPhase = PhaseEnum.StartTurn;
        currentBattlerNumber = turnBasedBattlers.Count;
    }

    #region IManagerBase
    public int ActPriority { get; } = 0;

    public void Act()
    {
        if (isProcessing == false)
        {
            return;
        }

        switch (CurrentPhase)
        {
            case PhaseEnum.StartTurn:
                NotifyBattlerOfStartTurn();
                break;
            case PhaseEnum.CommandProcessing:
                ProcessBattlerCommand();
                break;
            case PhaseEnum.EndTurn:
                NotifyBattlerOfEndTurn();
                break;
        }
    }

    void NotifyBattlerOfStartTurn()
    {
        if (sortedBattlers.Count == currentBattlerNumber)
        {
            currentBattlerNumber = 0;
            sortedBattlers[currentBattlerNumber].OnStartTurn();
        }
        else if (turnBasedBattlers[currentBattlerNumber].IsEndBattleProcess)
        {
            currentBattlerNumber++;
            if (turnBasedBattlers.Count != currentBattlerNumber)
            {
                turnBasedBattlers[currentBattlerNumber].OnStartTurn();
            }
            else
            {
                CurrentPhase = PhaseEnum.CommandProcessing;
                sortedBattlers.Sort(comparer);
            }
        }
    }

    void ProcessBattlerCommand()
    {
        if (sortedBattlers.Count == currentBattlerNumber)
        {
            currentBattlerNumber = 0;
            sortedBattlers[currentBattlerNumber].OnStartProcessingCommand();
        }
        else if (sortedBattlers[currentBattlerNumber].IsEndBattleProcess)
        {
            currentBattlerNumber++;
            if (sortedBattlers.Count != currentBattlerNumber)
            {
                sortedBattlers[currentBattlerNumber].OnStartProcessingCommand();
            }
            else
            {
                CurrentPhase = PhaseEnum.EndTurn;
            }
        }
    }

    void NotifyBattlerOfEndTurn()
    {
        if (turnBasedBattlers.Count == currentBattlerNumber)
        {
            currentBattlerNumber = 0;
            turnBasedBattlers[currentBattlerNumber].OnEndTurn();
        }
        else if (turnBasedBattlers[currentBattlerNumber].IsEndBattleProcess)
        {
            currentBattlerNumber++;
            if (turnBasedBattlers.Count != currentBattlerNumber)
            {
                turnBasedBattlers[currentBattlerNumber].OnEndTurn();
            }
            else
            {
                eventReceiver?.OnEndAllBattleProcess();
                isProcessing = false;
                currentBattlerNumber = 0;
            }
        }
    }

    #endregion
}
