using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WonState : IState
{
    public void OnEnter(BotAI botAI)
    {
        botAI.Agent.isStopped = true;
        Quaternion target = Quaternion.Euler(0, 180, 0);
        botAI.transform.rotation = Quaternion.Slerp(botAI.transform.rotation, target, Time.deltaTime * 10);
    }

    public void OnExecute(BotAI botAI)
    {
        
    }

    public void OnExit(BotAI botAI)
    {

    }
}
