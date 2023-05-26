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
    private void CreateBrick(Stage stage, Character character)
    {
        if (character.StageLevel != stage.StageLevel)
        {
            character.StageLevel = this.stage.StageLevel;
            int _poolSize = (this.stage.Row * this.stage.Column) / (character.ColorData.Mats.Length - 1);
            //Debug.Log("" + (int)character.ColorType);
            //Thêm danh sách màu vào Stage
            //Debug.Log("Stage Enter");
            if (!isCheckColorInStage(ListColor, character.ColorType))
            {
                listColor.Add((int)character.ColorType);
                StartCoroutine(InitSpawnObjectWithColor(0.3f, character.ColorType, _poolSize, stage));
            }
        }
    }
    //kiểm tra gạch đã tạo trên sân chưa
    private bool isCheckColorInStage(List<int> a_ListObject, ColorType a_ColorType)
    {
        for (int i = 0; i < a_ListObject.Count; i++)
        {
            if ((ColorType)a_ListObject[i] == a_ColorType)
            {
                Debug.Log("True"+ (ColorType)a_ListObject[i]);
                return true;
            }
        }
        return false;
    }
    //Tạo gạch trên sân tương ứng với màu của Character
    protected IEnumerator InitSpawnObjectWithColor(float time, ColorType colorType, int poolSize, Stage stage)
    {
        yield return new WaitForSeconds(time);
        int num_Count = stage.ListPoolBrickPos.Count;
        if (num_Count > 0)
        {
            for (int j = 0; j < poolSize; j++)
            {
                //Tạo và Thêm đối tượng vào danh sách Gạch với màu tương ứng Cho Nhân Vật ở trên sân
                int randomIndex = Random.Range(0, stage.ListPoolBrickPos.Count);
                Vector3 a_vector3 = stage.ListPoolBrickPos[randomIndex];
                PooledObject brickObject = Spawner(stage.Brick, stage.BrickParent);
                brickObject.transform.position = a_vector3;
                brickObject.GetComponent<Brick>().ChangeColor(colorType);
                brickObject.GetComponent<Brick>().StageLevel = stage.StageLevel;
                stage.ListPoolBrickPos.Remove(a_vector3);
                //ListBrickInStageCharacterColor.Add(brickObject.gameObject);
                stage.ListBrickInStage.Add(brickObject.gameObject);
            }
        }
    }
}
