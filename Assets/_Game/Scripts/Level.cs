using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private List<Stage> listStage;
    [SerializeField] private GameObject wintarget;
    [SerializeField] private List<GameObject> listStair;

    public List<GameObject> ListStair { get => listStair; set => listStair = value; }
    public LevelManager LevelManager { get => levelManager; set => levelManager = value; }
    public List<Stage> ListStage { get => listStage; set => listStage = value; }
    public GameObject Wintarget { get => wintarget; set => wintarget = value; }
    private void Start()
    {
        OnInit();
    }
    public void OnInit()
    {
        if (GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>())
        {
            levelManager = LevelManager.instance;
            levelManager.ListStage = ListStage;
            levelManager.Wintarget = Wintarget;
            levelManager.ListStair = ListStair;
            levelManager.GameState = GameState.Init;
        }
    }
}
