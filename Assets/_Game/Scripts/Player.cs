using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{

    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private FixedJoystick _fixedJoystick;
    [SerializeField] private float moveSpeed = 0.5f;

    public override void OnInit()
    {
        base.OnInit();
    }
    public void Update()
    {
       
        if (_fixedJoystick.Horizontal != 0 || _fixedJoystick.Vertical != 0)
        {
            Move();
        }
        else if (_fixedJoystick.Horizontal == 0 || _fixedJoystick.Vertical == 0)
        {
            ChangeAnim("Idle");
        }

    }
    private void Move()
    {
        Vector3 _Direction = new Vector3(_fixedJoystick.Horizontal * moveSpeed, _rigidbody.velocity.y, _fixedJoystick.Vertical * moveSpeed);
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
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
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
