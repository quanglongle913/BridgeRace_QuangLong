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
        if (ListStage != null)
        {
            levelManager.ListStage = ListStage;
        }
        if (Wintarget != null)
        {
            levelManager.Wintarget = Wintarget;
        }
        if (ListStair != null)
        {
            levelManager.ListStair = ListStair;
        }
    }
}
