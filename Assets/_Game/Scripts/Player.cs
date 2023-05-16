using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{

    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private FixedJoystick _fixedJoystick;
    [SerializeField] private float moveSpeed;

    private void Awake()
    {
        ListBrickObject = new List<GameObject>();
    }
    public override void OnInit()
    {
        base.OnInit();

        _rigidbody = _rigidbody.GetComponent<Rigidbody>();
    }
    public void Update()
    {
        _rigidbody.velocity = new Vector3(_fixedJoystick.Horizontal * moveSpeed, _rigidbody.velocity.y, _fixedJoystick.Vertical * moveSpeed);
        if (_fixedJoystick.Horizontal != 0 || _fixedJoystick.Vertical != 0)
        { 
            RotateTowards(this.gameObject, _rigidbody.velocity);
            ChangeAnim("Run");
        }
        else if (_fixedJoystick.Horizontal == 0 || _fixedJoystick.Vertical == 0)
        {
            ChangeAnim("Idle");
        }

    }
}
