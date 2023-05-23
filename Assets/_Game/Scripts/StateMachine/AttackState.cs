using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : IState
{
    public void OnEnter(BotAI botAI)
    {
       
    }

    public void OnExecute(BotAI botAI)
    {
        //Debug.Log("AttackState");
        if (botAI.isWin)
        {
            //Debug.Log("WIN");
            botAI.Win();
        }
        else if (botAI.BrickCount>0)
        {
            //TODO Move TP
            botAI.Attack();
        }
        else
        {
            botAI.ChangeState(new IdleState());
        }
    }

    public void OnExit(BotAI botAI)
    {

    }
}