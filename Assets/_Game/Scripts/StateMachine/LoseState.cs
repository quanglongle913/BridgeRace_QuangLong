using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseState : IState
{
    public void OnEnter(BotAI botAI)
    {
        botAI.StopMoving();
        botAI.Agent.isStopped = true;
    }

    public void OnExecute(BotAI botAI)
    {
        
    }

    public void OnExit(BotAI botAI)
    {

    }
}
