using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float xAxis, yAxis, zAxis;

    private void FixedUpdate()
    {
        transform.position = new Vector3(player.transform.position.x + xAxis, player.transform.position.y + yAxis, player.transform.position.z + zAxis);
        if (player.GetComponent<Player>().isWin) {
            transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 4.0f, player.transform.position.z -10.0f);
            Quaternion target = Quaternion.Euler(0, 0, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * 10);
        }
    }
}
