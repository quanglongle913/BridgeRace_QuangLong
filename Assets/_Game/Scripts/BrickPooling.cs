using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickPooling : DrawMap
{
    [Tooltip("Reference to Object Pool")]
    [SerializeField] private ObjectPool Brick;
    [SerializeField] private int BrickColorIndex;
    [SerializeField] private int stageLevel = 0;
    //so luong vien gach
    private int poolSize;

    private List<Vector3> ListPoolBrickPos;

    private List<GameObject> list = new();

    private void Start()
    {
        OnInit();
    }
    private void OnInit()
    {
        StartCoroutine(OnInitCoroutine(0.1f));
        ListPoolBrickPos = new List<Vector3>();
    }
    IEnumerator OnInitCoroutine(float time)
    {
        yield return new WaitForSeconds(time);
        poolSize = Row * Column;
        GenerateAllObject();
    }
    //Tao vi tri tri cua cac vien -> luu vao LIST
    private void ScreatePoolBrickPos()
    {
        for (int i = 0; i < Row; i++)
        {
            for (int j = 0; j < Column; j++)
            {
                int index = Row * j + i;
                //row =12 column =10 ///TEST LOGIC
                //i=0 =>z=5 i=1=>z=4 => Z=5-i
                //j=0 x=-6,j=1 x=-5,j=2 x=-4,j=3 x=-3, x=5 j=10
                Vector3 item = new Vector3((j - (Row / 2)), 0.05f, ((Column / 2) - i));
                ListPoolBrickPos.Add(item);
            }
        }
    }
    //Tao Gach
    private void GenerateNewObject(int index, Vector3 _vector3)
    {

        //Debug.Log(ListPoolBrick.Count +" : Index"+ index);
        GameObject brickObject =Brick.GetPooledObject().gameObject;
        if (brickObject == null)
        {
            return;
        }
        brickObject.GetComponent<Brick>().ChangeColor((ColorType) index);
        brickObject.GetComponent<Brick>().StageLevel = stageLevel;
        brickObject.transform.SetParent(PoolParent.transform);
        brickObject.transform.position = _vector3;
        brickObject.SetActive(true);

        list.Add(brickObject);

    }

    // Tao Gach va set vi tri trong LIST vi tri-> xoa vi tri trong list da set
    private void GenerateAllObject()
    {
        ScreatePoolBrickPos();
        //Debug.Log("Set brick");
        for (int i = 0; i < BrickColorIndex; i++)
        {
            for (int j = 0; j < poolSize / 4; j++) // j= 0->3 if count =4
            {
                int randomIndex = Random.Range(0, ListPoolBrickPos.Count);
                GenerateNewObject(i, ListPoolBrickPos[randomIndex]);
                ListPoolBrickPos.Remove(ListPoolBrickPos[randomIndex]);
            }
        }
        //chia 4 du thi ...
        if (ListPoolBrickPos.Count > 0)
        {
            for (int i = 0; i < ListPoolBrickPos.Count; i++)
            {
                GenerateNewObject(i, ListPoolBrickPos[i]);
                ListPoolBrickPos.Remove(ListPoolBrickPos[i]);
            }
        }
    }
}
