using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairBrick : MonoBehaviour
{
    [SerializeField] private GameObject wall;
    public void OnTriggerEnter(Collider other)
    { 
        if (other.gameObject.GetComponent<Character>())
        {
            Character _character = other.gameObject.GetComponent<Character>();
            if (_character.BrickCount > 0 && wall.gameObject.activeSelf)
            {
                wall.gameObject.SetActive(false);
                //Remove Birck Character
                _character.RemoveBrick();
                //_character.BrickCount--;
                //_character.listBrickInCharacter[_character.BrickCount].SetActive(false);

                this.gameObject.GetComponent<Brick>().ChangeColor(_character.ColorType);
                MeshRenderer mesh = this.gameObject.GetComponent<MeshRenderer>();
                mesh.enabled = true;
            }
        }
    }
}
