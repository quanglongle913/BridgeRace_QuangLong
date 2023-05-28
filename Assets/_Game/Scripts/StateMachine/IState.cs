using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public interface IState
    {
        void OnEnter(BotAI botAI);

        void OnExecute(BotAI botAI);

        void OnExit(BotAI botAI);
    }

