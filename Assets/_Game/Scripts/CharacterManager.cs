using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterManager : MonoBehaviour
{
    public UnityAction<GameObject> AddBrick;
    public UnityAction<GameObject> RemoveBrick;
    public UnityAction<GameObject> ClearBrick;
    public UnityAction<GameObject> Stage;

    private void OnTriggerEnter(Collider other)
    { //check va cham Character voi viên gạch
        if (other.gameObject.tag == "Brick")
        {
            //Debug.Log(other.gameObject.name);
            AddBrick(other.gameObject);
        }
       
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Floor")
        {
            //Debug.Log(other.gameObject.name);
            Stage(other.gameObject);
        }
    }
}
