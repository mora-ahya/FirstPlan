using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFourDirectionInputReceiver
{
    public void OnPressDirectionButton(int dir);
}

public class FourDirectionInputManager : MonoBehaviour
{
    IFourDirectionInputReceiver receiver;

    public void SetReceiver(IFourDirectionInputReceiver receiver)
    {
        this.receiver = receiver;
    }
    
    public void OnPressDirectionButton(int dir)
    {
        receiver?.OnPressDirectionButton(dir);
    }
}
