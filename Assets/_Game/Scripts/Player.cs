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
            ChangeAnim("Run");
        }
        else if (_fixedJoystick.Horizontal == 0 || _fixedJoystick.Vertical == 0)
        {
            ChangeAnim("Idle");
        }

    }
    private void Move()
    {
        Vector3 _Direction = new Vector3(_fixedJoystick.Horizontal * moveSpeed, _rigidbody.velocity.y, _fixedJoystick.Vertical * moveSpeed);
        _rigidbody.velocity = _Direction;
        RotateTowards(this.gameObject, _Direction);
    }
   
}
