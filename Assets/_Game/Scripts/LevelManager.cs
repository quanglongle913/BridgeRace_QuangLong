using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    [SerializeField] private List<GameObject> listCharacter;
    [SerializeField] private GameObject mainCamera;
    [SerializeField] GameObject wintarget;
    [SerializeField] private List<string> listLevelScene;

    public UnityAction PLayerWinAction;
    private List<Stage> listStage;
    private List<List<Brick>> listBrickInStage;
    private List<List<Vector3>> listBrickPosInStage;
    public int inGameLevel;
    private List<GameObject> listStair;
    private GameState gameState;
    public List<GameObject> ListStair { get => listStair; set => listStair = value; }
    public List<GameObject> ListCharacter { get => listCharacter; set => listCharacter = value; }
    public int InGameLevel { get => inGameLevel; set => inGameLevel = value; }
    public List<List<Brick>> ListBrickInStage { get => listBrickInStage; set => listBrickInStage = value; }
    public List<Stage> ListStage { get => listStage; set => listStage = value; }
    public GameObject Wintarget { get => wintarget; set => wintarget = value; }
    public List<List<Vector3>> ListBrickPosInStage { get => listBrickPosInStage; set => listBrickPosInStage = value; }
    public GameState GameState { get => gameState; set => gameState = value; }

   
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        ListBrickInStage = new List<List<Brick>>();
        listBrickPosInStage = new List<List<Vector3>>();
        gameState = GameState.EndGame;
        //Debug.Log("Awake");
    }
    private void Start()
    {
        //OnInit();
        InGameLevel = PlayerPrefs.GetInt(Constant.LEVEL, 0);
        SceneManager.LoadScene(""+listLevelScene[InGameLevel], LoadSceneMode.Additive);
    }
    public void OnInit()
    {
        ListBrickInStage.Clear();
        listBrickPosInStage.Clear();
        mainCamera.GetComponent<CameraFollow>().OnInit();
        for (int i = 0; i < listStage.Count; i++)
        {
            List<Brick> temp = new List<Brick>();
            List<Vector3> temp2 = listStage[i].CreatePoolBrickPosMap();
            ListBrickInStage.Add(temp);
            listBrickPosInStage.Add(temp2);
        }

        for (int i = 0; i < ListCharacter.Count; i++)
        {
            if (ListCharacter[i].TryGetComponent<Player>(out var player))
            {
                player.WinAction += PlayerWin;
                player.OnInit();
                player.EndTarget = wintarget;
            }
            if (ListCharacter[i].TryGetComponent<BotAI>(out var botAI))
            {
                botAI.WinAction += PlayerLose;
                botAI.OnInit();
                botAI.EndTarget = wintarget;
                botAI.StairTP = ListStair[i - 1].transform.position;
                botAI.ChangeState(new IdleState());
            }
        }
        bool isDebug = true;
        UIManager.instance.isNextButton(isDebug);
        UIManager.instance.isReplayButton(isDebug);

    }
    public void Update()
    {
        if (gameState == GameState.Init)
        {
            OnInit();
            gameState = GameState.Ingame;
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
        gameState = GameState.EndGame;
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
                if (!botAI1.IsWin)
                {
                    botAI1.StopAll();
                }
            }
        }
        UIManager.instance.isNextButton(true);
        UIManager.instance.isReplayButton(true);
        gameState = GameState.EndGame;
    }
    public void nextLevel()
    {
        //Level = inGameLevel +1, check Level <5 -> level +++ else ko doi
        if (inGameLevel < 1)
        {
            inGameLevel++;
            PlayerPrefs.SetInt(Constant.LEVEL, inGameLevel);
            PlayerPrefs.Save();
        }
        initSceneWithGameLevel();
    }
    public void replay()
    {
        initSceneWithGameLevel();
    }
    private void initSceneWithGameLevel() 
    {
        GameObject[] obj = GameObject.FindGameObjectsWithTag(Constant.TAG_LEVEL);
        for (int i = 0; i < obj.Length; i++)
        {
            DestroyImmediate(obj[i], true);
        }
        ListStage.Clear();
        listStair.Clear();
        for (int i = 0; i < ListCharacter.Count; i++)
        {
            Character character = ListCharacter[i].GetComponent<Character>();
            Vector3 newPos = new Vector3(-5 + 4 * i, 0, -7);
            character.transform.position = newPos;
            character.StageLevel = 0;
        }
        InGameLevel = PlayerPrefs.GetInt(Constant.LEVEL, 0);
        SceneManager.LoadScene("" + listLevelScene[InGameLevel], LoadSceneMode.Additive);
    }
}
