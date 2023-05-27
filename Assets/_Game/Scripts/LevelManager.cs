using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    [SerializeField] private List<GameObject> listCharacter;
    [SerializeField] private GameObject mainCamera;
    [SerializeField] GameObject wintarget;

    public UnityAction PLayerWinAction;

    private List<Stage> listStage;
    private List<List<Brick>> listBrickInStage;
    private List<List<Vector3>> listBrickPosInStage;
    private int inGameLevel;
    private List<GameObject> listStair;

    public List<GameObject> ListStair { get => listStair; set => listStair = value; }
    public List<GameObject> ListCharacter { get => listCharacter; set => listCharacter = value; }
    public int InGameLevel { get => inGameLevel; set => inGameLevel = value; }
    public List<List<Brick>> ListBrickInStage { get => listBrickInStage; set => listBrickInStage = value; }
    public List<Stage> ListStage { get => listStage; set => listStage = value; }
    public GameObject Wintarget { get => wintarget; set => wintarget = value; }
    public List<List<Vector3>> ListBrickPosInStage { get => listBrickPosInStage; set => listBrickPosInStage = value; }
    
    public GameState gameState;
    private bool isInit;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        ListBrickInStage = new List<List<Brick>>();
        listBrickPosInStage = new List<List<Vector3>>();
        isInit = false;
        //Debug.Log("Awake");
    }
    private void Start()
    {
        //OnInit();
        gameState = GameState.Init;
    }
    public void OnInit()
    {
        ListBrickInStage.Clear();
        listBrickPosInStage.Clear();
        mainCamera.GetComponent<CameraFollow>().OnInit();
        InGameLevel = PlayerPrefs.GetInt(Constant.LEVEL, 0);
        //Instantiate(ListLevel[InGameLevel], new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
        for (int i = 0; i < listStage.Count; i++)
        {
            List<Brick> temp = new List<Brick>();
            List<Vector3> temp2 = listStage[i].CreatePoolBrickPosMap(listStage[i].Row, listStage[i].Column, listStage[i].Offset, listStage[i].BrickParent);
            ListBrickInStage.Add(temp);
            listBrickPosInStage.Add(temp2);
        }
        ListCharacter[0].gameObject.GetComponent<Character>().OnInit();
        StartCoroutine(OnInitCharacter(0.3f, new Vector3(-5, 0, -7), ListCharacter[0].gameObject, wintarget, ListStair[0]));
        
        ListCharacter[0].gameObject.GetComponent<Player>().WinAction += PlayerWin;
        for (int i = 1; i < ListCharacter.Count; i++)
        {
            ListCharacter[i].gameObject.GetComponent<BotAI>().WinAction += PlayerLose;
            ListCharacter[i].gameObject.GetComponent<BotAI>().OnInit();
            StartCoroutine(OnInitCharacter(0.3f, new Vector3(-5 + 4 * i, 0, -7), ListCharacter[i].gameObject, wintarget, ListStair[i-1]));
        }
        bool isDebug = true;
        UIManager.instance.isNextButton(isDebug);
        UIManager.instance.isReplayButton(isDebug);
        gameState = GameState.Ingame;
    }
    public void Update()
    {
        if (!isInit && gameState == GameState.Init && listStage.Count>0 && listStair.Count>0)
        {
            OnInit();
            isInit = true;
            //gameState = GameState.Ingame;
        }
        //Reset Character StageLevel =0
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
            inGameLevel++;
            PlayerPrefs.SetInt(Constant.LEVEL, inGameLevel);
            PlayerPrefs.Save();
        }
        for (int i = 0; i < listStage.Count; i++)
        {
            listStage[i].GetComponent<SpawnerBrickStage>().ListColor.Clear();

        }
        listStage.Clear();
        listStair.Clear();

        GameObject[] obj = GameObject.FindGameObjectsWithTag(Constant.TAG_LEVEL);
        for (int i = 0; i < obj.Length; i++)
        {
            DestroyImmediate(obj[i], true);
        }
        SceneManager.LoadScene("Level2", LoadSceneMode.Additive);
        gameState = GameState.EndGame;
        isInit = false;
        
        for (int i = 0; i < ListCharacter.Count; i++)
        {
            ListCharacter[i].gameObject.GetComponent<Character>().StageLevel = 0;
            if (i == ListCharacter.Count)
            {
                Debug.Log("Done");
                gameState = GameState.Init;
            }
        }
       
        OnInit();
    }
    public void replay()
    {
        /*GameObject[] obj = GameObject.FindGameObjectsWithTag(Constant.TAG_LEVEL);
        for (int i = 0; i < obj.Length; i++)
        {
            DestroyImmediate(obj[i], true);
        }*/
        gameState = GameState.EndGame;
        
        OnInit();
    }
    protected IEnumerator OnInitCharacter(float time,Vector3 vector3,GameObject gameobject,GameObject EndTarget, GameObject StairTP)
    {
        //gameobject.transform.position = new Vector3(0,5,0);
        yield return new WaitForSeconds(time);
        gameobject.transform.position = vector3;
        if (gameobject.TryGetComponent<BotAI>(out var botAI))
        {
            botAI.EndTarget = EndTarget;
            botAI.StairTP = StairTP.transform.position;
            botAI.ChangeState(new IdleState());
        }
        if (gameobject.TryGetComponent<Player>(out var player))
        {
            player.EndTarget = EndTarget;
        }
    }
}
