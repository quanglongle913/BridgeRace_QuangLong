using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    [SerializeField] private List<GameObject> listCharacter;
    [SerializeField] private List<GameObject> listLevel;
    [SerializeField] private GameObject mainCamera;
    public UnityAction PLayerWinAction;

    private int inGameLevel;
    public List<GameObject> ListCharacter { get => listCharacter; set => listCharacter = value; }
    public int InGameLevel { get => inGameLevel; set => inGameLevel = value; }
    public List<GameObject> ListLevel { get => listLevel; set => listLevel = value; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        //Debug.Log("Awake");
    }
    private void Start()
    {
        OnInit();
    }
    public void OnInit()
    {
        mainCamera.GetComponent<CameraFollow>().OnInit();
        InGameLevel = PlayerPrefs.GetInt(Constant.LEVEL, 0);
        Instantiate(ListLevel[InGameLevel], new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
        //Debug.Log("List Lvl"+InGameLevel);
        WinTarget wintarget = ListLevel[InGameLevel].GetComponentInChildren<WinTarget>();
        ListCharacter[0].gameObject.GetComponent<Character>().OnInit();
        StartCoroutine(OnInitCharacter(0.3f, new Vector3(-5, 0, -7), ListCharacter[0].gameObject, wintarget.gameObject, wintarget.ListStair[0]));
        
        ListCharacter[0].gameObject.GetComponent<Player>().WinAction += PlayerWin;
        for (int i = 1; i < ListCharacter.Count; i++)
        {
            ListCharacter[i].gameObject.GetComponent<BotAI>().WinAction += PlayerLose;
            ListCharacter[i].gameObject.GetComponent<Character>().OnInit();
            StartCoroutine(OnInitCharacter(0.3f, new Vector3(-5 + 4 * i, 0, -7), ListCharacter[i].gameObject, wintarget.gameObject,wintarget.ListStair[i-1]));
        }
        bool isDebug = true;
        UIManager.instance.isNextButton(isDebug);
        UIManager.instance.isReplayButton(isDebug);
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
        PLayerWinAction();
        UIManager.instance.isNextButton(true);
        UIManager.instance.isReplayButton(true);
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
        UIManager.instance.isNextButton(true);
        UIManager.instance.isReplayButton(true);
    }
    public void nextLevel()
    {
        //Level = inGameLevel +1, check Level <5 -> level +++ else ko doi
        if (inGameLevel < 2)
        {
            //Debug.Log("inGameLevel");
            inGameLevel++;
            PlayerPrefs.SetInt(Constant.LEVEL, inGameLevel);
            PlayerPrefs.Save();
        }
        GameObject[] obj = GameObject.FindGameObjectsWithTag(Constant.TAG_LEVEL);
        for (int i = 0; i < obj.Length; i++)
        {
            DestroyImmediate(obj[i], true);
        }
        OnInit();
    }
    public void replay()
    {
        GameObject[] obj = GameObject.FindGameObjectsWithTag(Constant.TAG_LEVEL);
        for (int i = 0; i < obj.Length; i++)
        {
            DestroyImmediate(obj[i], true);
        }
        OnInit();
    }
    protected IEnumerator OnInitCharacter(float time,Vector3 vector3,GameObject gameobject,GameObject EndTarget, GameObject StairTP)
    {
        gameobject.transform.position = new Vector3(0,5,0);

        yield return new WaitForSeconds(time);
        gameobject.transform.position = vector3;
        if (gameobject.TryGetComponent<BotAI>(out var botAI))
        {
            botAI.StageLevel = 0;
            botAI.EndTarget = EndTarget;
            botAI.StairTP = StairTP.transform.position;
            botAI.ChangeState(new IdleState());
        }
        if (gameobject.TryGetComponent<Player>(out var player))
        {
            player.StageLevel = 0;
            player.EndTarget = EndTarget;
        }
    }
}
