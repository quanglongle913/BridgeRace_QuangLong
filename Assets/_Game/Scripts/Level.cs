using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] private List<GameObject> listBotAI;
    [SerializeField] private Stage startStage;
    public List<GameObject> ListBotAI { get => listBotAI; set => listBotAI = value; }
    private void Start()
    {
        for (int i = 0; i < ListBotAI.Count; i++)
        {
            ListBotAI[i].gameObject.GetComponent<BotAI>().Stage = startStage;
        }
    }
}
