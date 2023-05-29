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
            character.CreateBrick += CreateBrick;
        }
    }
    public void OnDespawn()
    {
        if (character != null)
        {
            character.CreateBrick -= CreateBrick;
        }
    }

    //Create Pooling Object in BrickStackParent of Player
    private void CreateBrick()
    {
            StartCoroutine(OnCreateBrickStackPoolingObj(0.5f)); 
    }
    protected IEnumerator OnCreateBrickStackPoolingObj(float time)
    {
        for (int i = 0; i < character.ListBrickInCharacter.Count; i++)
        {
            character.ListBrickInCharacter[i].SetActive(false);
            character.ListBrickInCharacter[i].GetComponent<PooledObject>().Release();
        }
        character.ListBrickInCharacter.Clear();
       
        yield return new WaitForSeconds(time);
        for (int i = 0; i < 50; i++)
        {
            PooledObject brickObject = Spawner(character.Brick, character.BrickStackParent);
            brickObject.GetComponent<BrickCharacter>().ChangeColor(character.ColorType);
            brickObject.transform.localPosition = new Vector3(0, i*2, 0);
            brickObject.transform.localScale = new Vector3(1, 1.96f, 1);
            brickObject.gameObject.SetActive(false);
            //Add to List
            character.ListBrickInCharacter.Add(brickObject.gameObject);
            //Debug.Log("Character ListBrickInCharacter"+ character.ListBrickInCharacter.Count);
        }
    }
}
