using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Stage : DrawMap
{

    [SerializeField] private int stageLevel = 0;
    [SerializeField] private ObjectPool brick;
    private List<int> listColor;

    public UnityAction<Character, LevelManager> CreateBrick;
    

    public List<int> ListColor { get => listColor; set => listColor = value; }
    public int StageLevel { get => stageLevel; set => stageLevel = value; }
    public ObjectPool Brick { get => brick; set => brick = value; }
    private void Awake()
    {
        listColor = new List<int>();
    }
    public List<Vector3> CreatePoolBrickPosMap()
    {
        List<Vector3> listPoolBrickPos = new List<Vector3>();
        for (int i = 0; i < Row; i++)
        {
            for (int j = 0; j < Column; j++)
            {
                int index = Row * j + i;
                //row =12 column =10 ///TEST LOGIC
                //i=0 =>z=5 i=1=>z=4 => Z=5-i
                //j=0 x=-6,j=1 x=-5,j=2 x=-4,j=3 x=-3, x=5 j=10
                Vector3 birckPosition = new Vector3((j - (Row / 2)) + offset * j + BrickParent.transform.position.x, 0.05f + BrickParent.transform.position.y, ((Column / 2) - i) - offset * i + BrickParent.transform.position.z);
                listPoolBrickPos.Add(birckPosition);
            }
        }
        return listPoolBrickPos;
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Character>(out var character))
        {
            //Character vào sàn thêm viên gạch có màu tương ứng với character 
            //Debug.Log("Stage Collider");
            character.Stage = gameObject.GetComponent<Stage>();
            CreateBrick(character, gameObject.transform.parent.GetComponent<Level>().LevelManager);
        }
    }
}
