using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    [SerializeField] private int stageLevel = 0;
    [SerializeField] private GameObject brickParent; // BrickParent in Stage
    public int StageLevel { get => stageLevel; set => stageLevel = value; }
    public GameObject BrickParent { get => brickParent; set => brickParent = value; }
}
