using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpawnerBrickCharacter : PooledObject
{
    [SerializeField] private Character character;
    private void Start()
    {
        OnInit();
    }
    public void OnInit()
    {
        if (character != null)
        {
            character.CreateBrick += CreateBrickCharacter;
        }
    }
    public void OnDespawn()
    {
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
        for (int i = 0; i < 50; i++)
        {
            PooledObject brickObject = Spawner(_character.Brick, _character.BrickStackParent);
            brickObject.GetComponent<BrickCharacter>().ChangeColor(_character.ColorType);
            brickObject.transform.localPosition = new Vector3(0, i, 0);
            brickObject.transform.localScale = new Vector3(1, 0.96f, 1);
            brickObject.gameObject.SetActive(false);

            //Add to List
            _character.listBrickInCharacter.Add(brickObject.gameObject);
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
