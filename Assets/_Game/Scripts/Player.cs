using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    [Header("Player Moving:")]
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] private FloatingJoystick floatingJoystick;
    [SerializeField] private float moveSpeed = 0.5f;

    [Header ("Player Step Clinmb:")]
    [SerializeField] GameObject stepRayUpper;
    [SerializeField] GameObject stepRayLower;
    [SerializeField] float stepHeight = 0.3f;
    [SerializeField] float stepSmooth = 2f;

    private bool isGrounded = false;
    private float horizontal;
    private float vertical;
    public override void Awake()
    {
        base.Awake();
        stepRayUpper.transform.position = new Vector3(stepRayUpper.transform.position.x, stepHeight, stepRayUpper.transform.position.z);
    }
    public override void OnInit()
    {
        base.OnInit();
    }
    public void FixedUpdate()
    {
        horizontal = floatingJoystick.Horizontal;
        vertical = floatingJoystick.Vertical;
        //Debug.Log("horizontal:" + horizontal+ "------vertical:" + vertical);
        isGrounded = CheckGrounded();
        if (isGrounded)
        {
            //Debug.Log("isGrounded");
            if (Mathf.Abs(horizontal) >= 0.03 || Mathf.Abs(vertical) >= 0.03)
            {
                playerMove(horizontal, vertical);
            }
            else if (horizontal == 0 || vertical == 0)
            {
                ChangeAnim("Idle");
            }
        }
        //stepClimb();

    }
    private void playerMove(float _horizontal,float _vertical)
    {
        Vector3 _Direction = new Vector3(_horizontal * moveSpeed, rigidbody.velocity.y, _vertical * moveSpeed);
        Vector3 _Target = new Vector3(rigidbody.position.x + _Direction.x, rigidbody.position.y, rigidbody.position.z + _Direction.z);
        RotateTowards(this.gameObject, _Direction);
        if (!isWall())
        {
           
            _Target.y = rigidbody.position.y;
            //rigidbody.velocity = _Direction;
            transform.position = _Target;
            ChangeAnim("Run");
        }
    }
   
    private bool isWall()
    {
        RaycastHit hit;
        bool isWall = false;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Constant.RAYCAST_HIT_RANGE, LayerMask.GetMask(Constant.LAYER_WALL_FLOOR)))
        {
            isWall = true;
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
    private bool CheckGrounded()
    {
        Debug.DrawLine(transform.position, transform.position + Vector3.down * 1.1f, Color.red);
        RaycastHit hit;
        return Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 1.1f, groundLayer);
    }
}
