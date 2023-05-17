using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolingSpawner : MonoBehaviour
{
    //Spawner Floor Brick
    public void Spawner(ColorType colorType, int stageLevel, ObjectPool a_obj, GameObject a_root, Vector3 a_vector3)
    {
        //Debug.Log(ListPoolBrick.Count +" : Index"+ index);
        GameObject brickObject = a_obj.GetPooledObject().gameObject;
        brickObject.GetComponent<Brick>().ChangeColor(colorType);
        //StageLevel is Floor level
        brickObject.GetComponent<Brick>().StageLevel = stageLevel;
        brickObject.transform.SetParent(a_root.transform);
        brickObject.transform.position = a_vector3;
    }
    //Spawner Character Brick
    public void Spawner(int index, ObjectPool a_obj, GameObject a_root, ColorType colorType)
    {
        for (int i = 0; i < index; i++)
        {
            GameObject brickObject = a_obj.GetPooledObject().gameObject;
            brickObject.GetComponent<BrickCharacter>().ChangeColor(colorType);
            brickObject.transform.SetParent(a_root.transform);
            brickObject.SetActive(true);
        }
        SetPoolBrickPosCharacter(a_root);
    }
    private void SetPoolBrickPosCharacter(GameObject a_root)
    {
        if (a_root.gameObject.transform.childCount > 0)
        {
            for (int i = 0; i < a_root.gameObject.transform.childCount; i++)
            {
                a_root.gameObject.transform.GetChild(i).gameObject.transform.localPosition = new Vector3(0, i, 0);
                a_root.gameObject.transform.GetChild(i).gameObject.transform.localScale = new Vector3(1, 0.96f, 1);
                a_root.gameObject.transform.GetChild(i).gameObject.SetActive(false);
            }

        }
        else
            return;
    }
    
    protected void SpawnObjectWithColor(ColorType colorType,int stageLevel, int poolSize, ObjectPool a_obj, GameObject a_root, List<Vector3> a_listVector3)
    {

        //Debug.Log("Index: " + j);
        if (getListPoolBrickPosCount(a_listVector3) > 0)
        {
            for (int j = 0; j < poolSize / 4; j++) // j= 0->3 if count =4
            {
                int randomIndex = Random.Range(0, getListPoolBrickPosCount(a_listVector3));
                Debug.Log("randomIndex: " + randomIndex);
                Debug.Log("getListPoolBrickPosCount: " + getListPoolBrickPosCount(a_listVector3));
                Vector3 a_vector3 = getBrickPos(randomIndex, a_listVector3);
                //tăng 1 đơn vị tương ứng với màu 1 = Yellow vì 0 là None
                Spawner(colorType, stageLevel, a_obj, a_root, a_vector3);
                a_listVector3.Remove(a_vector3);
            }
            //chia 4 du thi ...tạo gạch màu xám
            /*int num = poolSize % 4;
            if (num != 0 && getListPoolBrickPosCount(a_listVector3) <= 3)
            {
                for (int i = 0; i < getListPoolBrickPosCount(a_listVector3); i++)
                {
                    Spawner(0, stageLevel, a_obj, a_root, getBrickPos(i, a_listVector3));
                    a_listVector3.Remove(getBrickPos(i, a_listVector3));
                }
            }*/
        }

    }
    private Vector3 getBrickPos(int index, List<Vector3> a_listVector3)
    {
        //Debug.Log(index);
        return a_listVector3[index];
    }
    private int getListPoolBrickPosCount(List<Vector3> a_listVector3)
    {
        return a_listVector3.Count;
    }
}
