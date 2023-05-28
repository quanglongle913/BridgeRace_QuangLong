using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IState
{
    float timer;
    float randomTime;
    public void OnEnter(BotAI botAI)
    {
        botAI.StopMoving();
        if (botAI.Agent.isStopped)
        {
            botAI.Agent.isStopped = false;
        }
        timer = 0;
        randomTime = Random.Range(0.5f,1.0f);
        //Debug.Log("IdleState");
    }

    public void OnExecute(BotAI botAI)
    {
        //Debug.Log("Idle OnExcute"+ timer);
        if (!botAI.isEnoughBrick()) 
        {
            //Debug.Log("Idle OnExcute" + timer);
            timer += Time.deltaTime;
            if (timer > randomTime)
            {
                //Debug.Log("Idle OnExcute" + timer);
                botAI.ChangeState(new PatrolState());
            }
        }
        
    }

    public void OnExit(BotAI botAI)
    {

    }
}

