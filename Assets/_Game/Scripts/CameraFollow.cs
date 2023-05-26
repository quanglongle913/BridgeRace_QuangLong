using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float xAxis, yAxis, zAxis;
    [SerializeField] private LevelManager levelManager;
    bool isWin;
    private void Start()
    {
        OnInit();
        isWin = false;
    }
    public void OnInit()
    {

        if (levelManager != null)
        {
            levelManager.PLayerWinAction += PLayerWinAction;
        }
    }
    private void PLayerWinAction()
    {
        //Debug.Log("CameraFollow");
        isWin = true;
        Vector3 newPos = new Vector3(player.transform.position.x, player.transform.position.y + 4.0f, player.transform.position.z - 10.0f);
        transform.position = newPos;
        Quaternion target = Quaternion.Euler(0, 0, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * 1000);
    }
    private void FixedUpdate()
    {
        if (!isWin)
        {
            transform.position = new Vector3(player.transform.position.x + xAxis, player.transform.position.y + yAxis, player.transform.position.z + zAxis);
        }
        
    }
}
