using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolingSpawner : MonoBehaviour
{

    //Spawn Floor Brick
    public PooledObject Spawner(ColorType colorType, int stageLevel, ObjectPool a_obj, GameObject a_root, Vector3 a_vector3)
    {
        PooledObject brickObject = a_obj.GetPooledObject();
        brickObject.GetComponent<Brick>().ChangeColor(colorType);
        brickObject.GetComponent<Brick>().StageLevel = stageLevel;
        brickObject.transform.SetParent(a_root.transform);
        brickObject.transform.position = a_vector3;
        return brickObject;
    }
    //Spawn Character Brick
    public PooledObject Spawner(ObjectPool a_obj, GameObject a_root, ColorType colorType)
    {
        PooledObject brickObject = a_obj.GetPooledObject();
        brickObject.GetComponent<BrickCharacter>().ChangeColor(colorType);
        brickObject.transform.SetParent(a_root.transform);
        brickObject.gameObject.SetActive(true);
        return brickObject;
    }
    //Tạo Gạch màu và random vị trí trên sân
    protected PooledObject SpawnObjectWithColor(ColorType colorType, int stageLevel, ObjectPool a_obj, GameObject a_root, List<Vector3> a_listVector3)
    {
        int randomIndex = Random.Range(0, getListVector3Count(a_listVector3));
        Vector3 a_vector3 = getListVector3(randomIndex, a_listVector3);
        PooledObject brickObject = Spawner(colorType, stageLevel, a_obj, a_root, a_vector3);
        a_listVector3.Remove(a_vector3);
        return brickObject;
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
