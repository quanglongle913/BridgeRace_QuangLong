using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : IState
{
    public void OnEnter(BotAI botAI)
    {
        //Debug.Log("PatrolState");
        botAI.IsBrickTarget = false;
    }

    public void OnExecute(BotAI botAI)
    {
        //Debug.Log("PatrolState OnExecute: "+ botAI.isEnoughBrick());
        if (!botAI.isEnoughBrick())
        {
            if (!botAI.IsBrickTarget)
            {
                botAI.TargetPoint = botAI.getTarget();
                //Debug.Log("Moving");
                botAI.Moving();
            }
            if (botAI.isDestination())
            {
                botAI.ChangeState(new IdleState());
            }
        }
        else if (botAI.isEnoughBrick())
        {
            //Debug.Log("isEnoughBrick");
            botAI.ChangeState(new AttackState());
        }
       
    }
    public void OnExit(BotAI botAI)
    {

    }
}

