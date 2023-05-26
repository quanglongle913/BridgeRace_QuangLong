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
        timer = 0;
        randomTime = Random.Range(0.5f,1.0f);
        //Debug.Log("IdleState");
    }

    public void OnExecute(BotAI botAI)
    {
        if (!botAI.isEnoughBrick()) 
        {
            timer += Time.deltaTime;
            if (timer > randomTime)
            {
                //Debug.Log("Idle OnExcute");
                botAI.ChangeState(new PatrolState());
            }
        }
    }

    public void OnExit(BotAI botAI)
    {

    }
}

