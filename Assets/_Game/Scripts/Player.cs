using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{

    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private FloatingJoystick _floatingJoystick;

    [SerializeField] private float moveSpeed = 0.5f;
    private float horizontal;
    private float vertical;
    public override void OnInit()
    {
        base.OnInit();
        

}
    public void Update()
    {
        horizontal = _floatingJoystick.Horizontal;
        vertical = _floatingJoystick.Vertical;
        //Debug.Log("horizontal:" + horizontal+ "------vertical:" + vertical);

        if (Mathf.Abs(horizontal) >= 0.03 || Mathf.Abs(vertical) >= 0.03)
        {
            Move(horizontal, vertical);
        }
        else if (horizontal == 0 || vertical == 0)
        {

            ChangeAnim("Idle");
        }
    }
    private void Move(float _horizontal,float _vertical)
    {
        Vector3 _Direction = new Vector3(_horizontal * moveSpeed, _rigidbody.velocity.y, _vertical * moveSpeed);
        RotateTowards(this.gameObject, _Direction);
        if (!isWall())
        {
            _rigidbody.velocity = _Direction;
            ChangeAnim("Run");
        }
    }
    private bool isWall()
    {
        RaycastHit hit;
        bool isWall= Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Constant.RAYCAST_HIT_RANGE, LayerMask.GetMask(Constant.LAYER_WALL_FLOOR));
        // Does the ray intersect any objects excluding the player layer
        if (isWall)
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.red);
            //Debug.Log("Did Hit");
        }
        else 
        {
            isWall = false;
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
            //Debug.Log("Did not Hit");
        }
        return isWall;
    }
}
