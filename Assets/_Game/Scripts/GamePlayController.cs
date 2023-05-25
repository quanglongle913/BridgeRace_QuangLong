using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayController : MonoBehaviour
{
    [SerializeField] private List<GameObject> listCharacter;
    public List<GameObject> ListCharacter { get => listCharacter; set => listCharacter = value; }
    private void Start()
    {
        OnInit();
    }
    public void OnInit()
    {
        ListCharacter[0].gameObject.GetComponent<Player>().WinAction += PlayerWin;

        for (int i = 1; i < ListCharacter.Count; i++)
        {
            ListCharacter[i].gameObject.GetComponent<BotAI>().WinAction += PlayerLose;
        }
    }
    private void PlayerWin()
    {
        for (int i = 0; i < ListCharacter.Count; i++)
        { 
            if (ListCharacter[i].TryGetComponent<BotAI>(out var botAI1))
            {
                botAI1.StopAll();
            }
        }

    }

    private void PlayerLose()
    {
        for (int i = 0; i < ListCharacter.Count; i++)
        {
            if (ListCharacter[i].TryGetComponent<Player>(out var player1))
            {
                player1.Lose();
            }
            if (ListCharacter[i].TryGetComponent<BotAI>(out var botAI1))
            {
                if (botAI1.IsWin)
                {
                    botAI1.Won();
                }
                else 
                {
                    botAI1.StopAll();
                }
            }
        }
    }
}
