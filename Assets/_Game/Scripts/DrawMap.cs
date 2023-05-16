using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawMap : PoolingSpawner
{
    [Tooltip("Pool Parent Object")]
    [SerializeField] protected GameObject PoolParent;
    [SerializeField] protected int Row;
    [SerializeField] protected int Column;

    [SerializeField] private float Size_x;
    [SerializeField] private float Size_z;
    [SerializeField] protected float offset;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        for (int i = 0; i < Row; i++)
        {
            for (int j = 0; j < Column; j++)
            {
                int index = Row * j + i;
                //row =12 column =10 ///TEST LOGIC
                //i=0 =>z=5 i=1=>z=4 => Z=5-i
                //j=0 x=-6,j=1 x=-5,j=2 x=-4,j=3 x=-3, x=5 j=10
                Vector3 birckPosition = new Vector3((j - (Row / 2)) + offset *j, 0.05f, ((Column / 2) - i)- offset*i);
                drawRectangle(birckPosition);
            }
        }
    }
    //Vẽ Scene vị trí viên gạch
    private void drawRectangle(Vector3 point)
    {
        //Top Left
        Vector3 topL = new Vector3(point.x - Size_x, point.y, point.z + Size_z);
        //Top Right
        Vector3 topR = new Vector3(point.x + Size_x, point.y, point.z + Size_z);
        //Bot Right
        Vector3 botR = new Vector3(point.x + Size_x, point.y, point.z - Size_z);
        //Bot Left
        Vector3 botL = new Vector3(point.x - Size_x, point.y, point.z - Size_z);

        Gizmos.DrawLine(topL, topR);
        Gizmos.DrawLine(topR, botR);
        Gizmos.DrawLine(botR, botL);
        Gizmos.DrawLine(botL, topL);
    }
}