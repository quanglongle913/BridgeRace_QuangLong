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

    public void OnTriggerEnter(Collider other)
    { //check va cham Character voi viên gạch

        if (other.gameObject.GetComponent<Brick>())
        {
            //Debug.Log(other.gameObject.name);
            AddBrick(other.gameObject);
        }
        if (other.gameObject.GetComponent<BotAI>())
        {
            //Debug.Log(other.gameObject.name);
            //(other.gameObject);
        }
        if (other.gameObject.GetComponent<Stage>())
        {
            //Character vào sàn thêm viên gạch có màu tương ứng với character 
            //Debug.Log("Stage Collider");
            Stage(other.gameObject);
        }
    }
    public void OnTriggerExit(Collider other)
    { 
        if (other.gameObject.GetComponent<Stage>())
        {
            //Character ra khỏi sàn -> Xóa hết viên gạch có màu tương ứng với character
        }
    }
}
