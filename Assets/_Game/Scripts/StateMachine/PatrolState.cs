using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : IState
{
    public void OnEnter(BotAI botAI)
    {
        //Debug.Log("PatrolState");
    }

    public void OnExecute(BotAI botAI)
    {
        //Debug.Log("PatrolState OnExecute: "+ botAI.isEnoughBrick());
        if (!botAI.isEnoughBrick())
        {
            if (botAI.isTarget())
            {
                botAI.Moving();
            }
            if (botAI.isDestination() && !botAI.isTarget())
            {
                botAI.ChangeState(new IdleState());
            }
            else 
            {
                botAI.ChangeState(new IdleState());
            }
           
        }
        else if (botAI.isEnoughBrick())
        {
            botAI.ChangeState(new IdleState());
        }
        
    }

    public void OnExit(BotAI botAI)
    {

    }
}

