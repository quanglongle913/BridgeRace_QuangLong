using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpawnerBrick : PooledObject
{

    //chuyuen sang PooledObject va sua laij sinh gachj
    //Spawn Floor Brick
    [SerializeField] private Stage stage;
    [SerializeField] private Character character;
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
        if (character != null)
        {
            character.CreateBrick += CreateBrickCharacter;
        }

    }

    //ham huy
    public void OnDespawn()
    {
        if (stage != null)
        {
            stage.CreateBrick -= CreateBrick;
        }
        if (character != null)
        {
            character.CreateBrick -= CreateBrickCharacter;
        }
    }

    //Create Pooling Object in BrickStackParent of Player
    private void CreateBrickCharacter(Character _character)
    {
            StartCoroutine(OnCreateBrickStackPoolingObj(0.2f, _character)); 
    }
    protected IEnumerator OnCreateBrickStackPoolingObj(float time, Character _character)
    {
        yield return new WaitForSeconds(time);
        for (int i = 0; i < _character.MaxBrickInCharacter; i++)
        {
            //Spawn Brick
            PooledObject brickObject = Spawner(_character.Brick, _character.BrickStackParent);
            brickObject.GetComponent<BrickCharacter>().ChangeColor(_character.ColorType);
            brickObject.transform.localPosition = new Vector3(0, i, 0);
            brickObject.transform.localScale = new Vector3(1, 0.96f, 1);
            brickObject.gameObject.SetActive(false);

            //Add to List
            _character.listBrickInCharacter.Add(brickObject.gameObject);
        }
    }
    //Tạo Gạch màu và random vị trí trên sân
    private void CreateBrick(Stage _stage,Character _character)
    {
        if (_character.StageLevel != _stage.StageLevel)
        {
            //Debug.Log("Stage Enter");
            _character.StageLevel = stage.StageLevel;
            //brickParent = stage.BrickParent;
            int _poolSize = stage.Row * stage.Column;
            StartCoroutine(InitSpawnObjectWithColor(0.5f, _character.ColorType, stage.StageLevel, _poolSize, stage.Brick, stage.BrickParent, stage.ListPoolBrickPos, _character.listBrickInStageCharacterColor));
        }
    }
    //Tạo gạch trên sân tương ứng với màu của Character
    protected IEnumerator InitSpawnObjectWithColor(float time, ColorType colorType, int stageLevel, int poolSize, ObjectPool a_brick_obj, GameObject a_root, List<Vector3> a_listVector3, List<GameObject> ListBrickInStageCharacterColor)
    {
        yield return new WaitForSeconds(time);
        int num_Count = getListVector3Count(a_listVector3);
        if (num_Count > 0)
        {
            for (int j = 0; j < poolSize / 4; j++)
            {
                //Tạo và Thêm đối tượng vào danh sách Gạch với màu tương ứng Cho Nhân Vật ở trên sân
                int randomIndex = Random.Range(0, getListVector3Count(a_listVector3));
                Vector3 a_vector3 = getListVector3(randomIndex, a_listVector3);
                PooledObject brickObject = Spawner(a_brick_obj, a_root);
                brickObject.transform.position = a_vector3;
                brickObject.GetComponent<Brick>().ChangeColor(colorType);
                brickObject.GetComponent<Brick>().StageLevel = stageLevel;
                a_listVector3.Remove(a_vector3);

                ListBrickInStageCharacterColor.Add(brickObject.gameObject);
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
