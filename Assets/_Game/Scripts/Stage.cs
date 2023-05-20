using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Stage : DrawMap
{
    [SerializeField] private int stageLevel = 0;

    [SerializeField] private ObjectPool brick;
    private List<GameObject> listBrickInStage;
    private List<Vector3> listPoolBrickPos;
    public UnityAction<Stage, Character> CreateBrick;

    public int StageLevel { get => stageLevel; set => stageLevel = value; }
  
    public ObjectPool Brick { get => brick; set => brick = value; }

    public List<Vector3> ListPoolBrickPos { get => listPoolBrickPos; set => listPoolBrickPos = value; }
    public List<GameObject> ListBrickInStage { get => listBrickInStage; set => listBrickInStage = value; }

    private void Awake()
    {
        ListPoolBrickPos = new List<Vector3>();
        listBrickInStage = new List<GameObject>();
        ListPoolBrickPos = CreatePoolBrickPosMap(Row, Column, Offset, brickParent);
    }
    private List<Vector3> CreatePoolBrickPosMap(int row, int column, float offset, GameObject a_root)
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
                Vector3 birckPosition = new Vector3((j - (Row / 2)) + offset * j + a_root.transform.position.x, 0.05f + a_root.transform.position.y, ((Column / 2) - i) - offset * i + a_root.transform.position.z);
                listPoolBrickPos.Add(birckPosition);
            }
        }
        return listPoolBrickPos;
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<CharacterTrigger>())
        {
            //Character vào sàn thêm viên gạch có màu tương ứng với character 
            //Debug.Log("Stage Collider");
            Stage stageObject = this.gameObject.GetComponent<Stage>();
            Character characterObject = other.GetComponent<Character>();
            CreateBrick(stageObject, characterObject);
        }
    }
}
