using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITurnBasedPlotterBase
{
    public bool IsAlreadyDecided { get; }
    public void OnStartPlotting();
    public void OnEndPlotting();
}

public interface ITurnBasedPlottingSystemEventReceiver
{
    public void OnEndAllPlotting();
}

public class TurnBasedPlottingSystem : IManagerBase
{
    readonly List<ITurnBasedPlotterBase> plotters = new List<ITurnBasedPlotterBase>();
    ITurnBasedPlottingSystemEventReceiver eventReceiver;

    bool isPlotting = false;
    int currentPlotterNumber = 0;

    public void SetEventReceiver(ITurnBasedPlottingSystemEventReceiver er)
    {
        eventReceiver = er;
    }

    public void AddPlotter(ITurnBasedPlotterBase plotter)
    {
        plotters.Add(plotter);
    }

    public void ClearPlotters()
    {
        plotters.Clear();
    }

    public void StartPlotting()
    {
        if (isPlotting)
        {
            return;
        }

        isPlotting = true;
        plotters[currentPlotterNumber].OnStartPlotting();
    }

    public void GoBack()
    {
        if (currentPlotterNumber > 0)
        {
            currentPlotterNumber--;
            plotters[currentPlotterNumber].OnStartPlotting();
        }
    }

    #region IManagerBase
    public int ActPriority { get; } = 0;
    public void Act()
    {
        if (isPlotting == false)
        {
            return;
        }

        if (plotters[currentPlotterNumber].IsAlreadyDecided)
        {
            plotters[currentPlotterNumber].OnEndPlotting();
            currentPlotterNumber++;
            if (plotters.Count != currentPlotterNumber)
            {
                plotters[currentPlotterNumber].OnStartPlotting();
            }
            else
            {
                eventReceiver?.OnEndAllPlotting();
                isPlotting = false;
                currentPlotterNumber = 0;
            }
        }
    }
    #endregion
}
