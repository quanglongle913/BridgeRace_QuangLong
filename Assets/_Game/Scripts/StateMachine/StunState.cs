using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunState : IState
{
    float timer;
    float randomTime;
    public void OnEnter(BotAI botAI)
    {

        botAI.Stun();
        botAI.Agent.isStopped = true;
        botAI.Agent.ResetPath();
        timer = 0;
        randomTime = Random.Range(2.0f, 3.5f);
    }

    public void OnExecute(BotAI botAI)
    {

        timer += Time.deltaTime;
        if (timer > randomTime)
        {
            botAI.ChangeState(new IdleState());
        }
    }
    public void OnExit(BotAI botAI)
    {
       
    }

   
}
