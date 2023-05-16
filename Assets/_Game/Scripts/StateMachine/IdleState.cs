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
        randomTime = Random.Range(0f,0.5f);
        //Debug.Log("Idle");
    }

    public void OnExecute(BotAI botAI)
    {
        
        timer += Time.deltaTime;
        if (timer > randomTime)
        {
            //Debug.Log("Idle OnExcute");
            botAI.ChangeState(new PatrolState());
        }
        
    }

    public void OnExit(BotAI botAI)
    {

    }
}

