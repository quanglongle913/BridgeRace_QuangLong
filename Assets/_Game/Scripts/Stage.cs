using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    [SerializeField] private int stageLevel = 0;
    [SerializeField] private GameObject brickParent; // BrickParent in Stage
    [SerializeField] private ObjectPool brick;
    [SerializeField] private int row;
    [SerializeField] private int column;
    [SerializeField] private float offset;
    private List<Vector3> listPoolBrickPos;

    public int StageLevel { get => stageLevel; set => stageLevel = value; }
    public GameObject BrickParent { get => brickParent; set => brickParent = value; }
    public ObjectPool Brick { get => brick; set => brick = value; }
    public int Row { get => row; set => row = value; }
    public int Column { get => column; set => column = value; }
    public float Offset { get => offset; set => offset = value; }
    public List<Vector3> ListPoolBrickPos { get => listPoolBrickPos; set => listPoolBrickPos = value; }
    private void Awake()
    {
        ListPoolBrickPos = new List<Vector3>();
        ListPoolBrickPos = CreatePoolBrickPosMap(Row, Column, Offset);
    }
    private List<Vector3> CreatePoolBrickPosMap(int row, int column, float offset)
    {
        List<Vector3> listPoolBrickPos = new List<Vector3>();
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                int index = row * j + i;
                //row =12 column =10 ///TEST LOGIC
                //i=0 =>z=5 i=1=>z=4 => Z=5-i
                //j=0 x=-6,j=1 x=-5,j=2 x=-4,j=3 x=-3, x=5 j=10
                Vector3 birckPosition = new Vector3((j - (row / 2)) + offset * j, 0.05f, ((column / 2) - i) - offset * i);
                listPoolBrickPos.Add(birckPosition);
            }
        }
        return listPoolBrickPos;
    }
}
