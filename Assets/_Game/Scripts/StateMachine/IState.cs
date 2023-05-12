using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public interface IState
    {
        void OnEnter(BotAI enemy);

        void OnExecute(BotAI enemy);

        void OnExit(BotAI enemy);
    }

