using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpawnerBrickStage : PooledObject
{
    [SerializeField] private Stage stage;

    List<int> listColor;

    public List<int> ListColor { get => listColor; set => listColor = value; }

    private void Start()
    {
        OnInit();
    }
    public void OnInit()
    {
        listColor = new List<int>();
        if (stage != null)
        {
            stage.CreateBrick += CreateBrick;
        }

    }

    //ham huy
    public void OnDespawn()
    {
        if (stage != null)
        {
            stage.CreateBrick -= CreateBrick;
        }
       
    }
  
    //Tạo Gạch màu và random vị trí trên sân
    private void CreateBrick(Stage _stage, Character _character)
    {
        if (_character.StageLevel != _stage.StageLevel)
        {
            Debug.Log("" + (int)_character.ColorType);
            //Thêm danh sách màu vào Stage
            listColor.Add((int)_character.ColorType);
           
            //Debug.Log("Stage Enter");
            _character.StageLevel = stage.StageLevel;
            int _poolSize = (stage.Row * stage.Column) / (_character.ColorData.Mats.Length - 1);
            StartCoroutine(InitSpawnObjectWithColor(0.5f, _character.ColorType, stage.StageLevel, _poolSize, stage.Brick, stage.BrickParent, stage.ListPoolBrickPos, _stage.ListBrickInStage));
        }
    }
    //Tạo gạch trên sân tương ứng với màu của Character
    protected IEnumerator InitSpawnObjectWithColor(float time, ColorType colorType, int stageLevel, int poolSize, ObjectPool a_brick_obj, GameObject a_root, List<Vector3> a_listVector3, List<GameObject> ListBrickInStage)
    {
        yield return new WaitForSeconds(time);
        int num_Count = getListVector3Count(a_listVector3);
        if (num_Count > 0)
        {
            for (int j = 0; j < poolSize; j++)
            {
                //Tạo và Thêm đối tượng vào danh sách Gạch với màu tương ứng Cho Nhân Vật ở trên sân
                int randomIndex = Random.Range(0, getListVector3Count(a_listVector3));
                Vector3 a_vector3 = getListVector3(randomIndex, a_listVector3);
                PooledObject brickObject = Spawner(a_brick_obj, a_root);
                brickObject.transform.position = a_vector3;
                brickObject.GetComponent<Brick>().ChangeColor(colorType);
                brickObject.GetComponent<Brick>().StageLevel = stageLevel;
                a_listVector3.Remove(a_vector3);
                //ListBrickInStageCharacterColor.Add(brickObject.gameObject);
                ListBrickInStage.Add(brickObject.gameObject);
            }
        }
    }


    protected Vector3 getListVector3(int index, List<Vector3> a_listVector3)
    {
        return a_listVector3[index];
    }
    protected int getListVector3Count(List<Vector3> a_listVector3)
    {
        return a_listVector3.Count;
    }
}
