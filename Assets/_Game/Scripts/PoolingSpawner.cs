using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolingSpawner : MonoBehaviour
{
    //Spawner Floor Brick
    public void Spawner(int index,int stageLevel, ObjectPool Brick, GameObject PoolParent, Vector3 _vector3)
    {
        //Debug.Log(ListPoolBrick.Count +" : Index"+ index);
        GameObject brickObject = Brick.GetPooledObject().gameObject;
        brickObject.GetComponent<Brick>().ChangeColor((ColorType)index);
        //StageLevel is Floor level
        brickObject.GetComponent<Brick>().StageLevel = stageLevel;
        brickObject.transform.SetParent(PoolParent.transform);
        brickObject.transform.position = _vector3;
    }
    public void Spawner(ColorType colorType, int stageLevel, ObjectPool Brick, GameObject PoolParent, Vector3 _vector3)
    {
        //Debug.Log(ListPoolBrick.Count +" : Index"+ index);
        GameObject brickObject = Brick.GetPooledObject().gameObject;
        brickObject.GetComponent<Brick>().ChangeColor(colorType);
        //StageLevel is Floor level
        brickObject.GetComponent<Brick>().StageLevel = stageLevel;
        brickObject.transform.SetParent(PoolParent.transform);
        brickObject.transform.position = _vector3;
    }
    //Spawner Character Brick
    public void Spawner(int index, ObjectPool Brick, GameObject BrickStackParent, ColorType colorType)
    {
        for (int i = 0; i < index; i++)
        {
            GameObject brickObject = Brick.GetPooledObject().gameObject;
            brickObject.GetComponent<BrickCharacter>().ChangeColor(colorType);
            brickObject.transform.SetParent(BrickStackParent.transform);
            brickObject.SetActive(true);
        }
        SetPoolBrickPosCharacter(BrickStackParent);
    }
    private void SetPoolBrickPosCharacter(GameObject BrickStackParent)
    {
        if (BrickStackParent.gameObject.transform.childCount > 0)
        {
            for (int i = 0; i < BrickStackParent.gameObject.transform.childCount; i++)
            {
                BrickStackParent.gameObject.transform.GetChild(i).gameObject.transform.localPosition = new Vector3(0, i, 0);
                BrickStackParent.gameObject.transform.GetChild(i).gameObject.transform.localScale = new Vector3(1, 0.96f, 1);
                BrickStackParent.gameObject.transform.GetChild(i).gameObject.SetActive(false);
            }

        }
        else
            return;
    }
    
    protected void SpawnObjectWithColor(ColorType colorType,int stageLevel, int poolSize, ObjectPool Brick, GameObject PoolParent, List<Vector3> listPoolBrickPos)
    {
        for (int j = 0; j < poolSize / 4; j++) // j= 0->3 if count =4
        {
            //Debug.Log("Index: " + j);
            int randomIndex = Random.Range(0, getListPoolBrickPosCount(listPoolBrickPos));
            //tăng 1 đơn vị tương ứng với màu 1 = Yellow vì 0 là None
            Spawner(colorType, stageLevel, Brick, PoolParent, getBrickPos(randomIndex, listPoolBrickPos));
            listPoolBrickPos.Remove(getBrickPos(randomIndex, listPoolBrickPos));
            
        }
        //chia 4 du thi ...tạo gạch màu xám
        /*if (getListPoolBrickPosCount(ListPoolBrickPos) <= 3)
        {
            for (int i = 0; i < getListPoolBrickPosCount(ListPoolBrickPos); i++)
            {
                Spawner(0, stageLevel, Brick, PoolParent, getBrickPos(i, ListPoolBrickPos));
                ListPoolBrickPos.Remove(getBrickPos(i, ListPoolBrickPos));
            }
        }*/
    }
    private Vector3 getBrickPos(int index, List<Vector3> listPoolBrickPos)
    {
        return listPoolBrickPos[index];
    }
    private int getListPoolBrickPosCount(List<Vector3> listPoolBrickPos)
    {
        return listPoolBrickPos.Count;
    }
}
