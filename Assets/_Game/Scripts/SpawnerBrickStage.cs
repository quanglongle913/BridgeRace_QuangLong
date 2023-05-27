using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpawnerBrickStage : PooledObject
{
    [SerializeField] private Stage stage;
    private void Start()
    {
        OnInit();
    }
    public void OnInit()
    {
  
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
    private void CreateBrick(Character character, LevelManager levelManager)
    {
        if (character.StageLevel != stage.StageLevel)
        {
            character.StageLevel = stage.StageLevel;
            int _poolSize = (stage.Row * stage.Column) / (character.ColorData.Mats.Length - 1);
            //Debug.Log("" + (int)character.ColorType);
            //Thêm danh sách màu vào Stage
            //Debug.Log("Stage Enter");
            if (!isCheckColorInStage(stage.ListColor, character.ColorType))
            {
                stage.ListColor.Add((int)character.ColorType);
                StartCoroutine(InitSpawnObjectWithColor(0.3f, character.ColorType, _poolSize, stage, levelManager));
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
                //Debug.Log("True" + (ColorType)a_ListObject[i]);
                return true;
            }
        }
        return false;
    }
    //Tạo gạch trên sân tương ứng với màu của Character
    protected IEnumerator InitSpawnObjectWithColor(float time, ColorType colorType, int poolSize, Stage stage, LevelManager levelManager)
    {
        yield return new WaitForSeconds(time);
        int stageLevel= stage.StageLevel;
        int index = stageLevel - 1;
        int num_Count = levelManager.ListBrickPosInStage[index].Count;
        if (num_Count > 0)
        {
            for (int j = 0; j < poolSize; j++)
            {
                //Tạo và Thêm đối tượng vào danh sách Gạch với màu tương ứng Cho Nhân Vật ở trên sân
                int randomIndex = Random.Range(0, levelManager.ListBrickPosInStage[index].Count);
                Vector3 a_vector3 = levelManager.ListBrickPosInStage[index][randomIndex];
                PooledObject brickObject = Spawner(stage.Brick, stage.BrickParent);
                brickObject.transform.position = a_vector3;
                Brick brick = brickObject.GetComponent<Brick>();
                brick.ChangeColor(colorType);
                brick.StageLevel = stageLevel;
                levelManager.ListBrickPosInStage[index].Remove(a_vector3);
                //ListBrickInStageCharacterColor.Add(brickObject.gameObject);
                //stage.ListBrickInStage.Add(brickObject.gameObject);
                //Debug.Log(""+ brick.StageLevel);
                levelManager.ListBrickInStage[index].Add(brick);
            }
        }
    }
}
