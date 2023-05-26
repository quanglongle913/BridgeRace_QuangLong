using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player : Character
{
    [Header("Player Moving:")]
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private FloatingJoystick floatingJoystick;
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float moveSpeedStair = 3.0f;
    [Header ("Player Step Clinmb:")]
    [SerializeField] GameObject stepRayLower;

    public UnityAction WinAction;

    private float MoveSpeed;
    private float horizontal;
    private float vertical;
    float stepOffset = 0.36f;
    public override void Awake()
    {
        base.Awake();

    }
    public override void OnInit()
    {
        base.OnInit();

        MoveSpeed = moveSpeed;
    }
    public void FixedUpdate()
    {
        horizontal = floatingJoystick.Horizontal;
        vertical = floatingJoystick.Vertical;
        if (IsWin)
        {
            Won();
        }
        else
        {
            if (CheckGrounded(0.5f))
            {
                if (Mathf.Abs(horizontal) >= 0.03 || Mathf.Abs(vertical) >= 0.03)
                {
                    Moving(horizontal, vertical);
                }
                else if (horizontal == 0 || vertical == 0)
                {
                    ChangeAnim("Idle");
                }
                //Chuyển nhân vật về sát mặt đất
                if (!CheckGrounded(0.05f))
                {
                    StickToGround(0.02f);
                }
            }
            else if (CheckGrounded(1.5f) && transform.position.y > 0)
            {
                StickToGround(stepOffset);
            }

        }
    }
    public void Lose()
    {
        ClearBrick();
        floatingJoystick.gameObject.SetActive(false);
        Debug.Log("You Losed");
    }
    private void Won()
    {
        ClearBrick();
        transform.position = EndTarget.transform.position;
        Quaternion target = Quaternion.Euler(0, 180, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * rotationSpeed);
        ChangeAnim("Dance");
        floatingJoystick.gameObject.SetActive(false);
        WinAction();
    }
    private void Moving(float _horizontal, float _vertical)
    {
        //StepOffset =0.3f
        Vector3 _Direction = new Vector3(_horizontal * moveSpeed * Time.fixedDeltaTime, _rigidbody.velocity.y, _vertical * moveSpeed * Time.fixedDeltaTime);
        TargetPoint = new Vector3(_rigidbody.position.x + _Direction.x, _rigidbody.position.y, _rigidbody.position.z + _Direction.z);
        RotateTowards(this.gameObject, _Direction);
        if (!isWall(LayerMask.GetMask(Constant.LAYER_WALL_FLOOR)) && !isWall(LayerMask.GetMask(Constant.LAYER_WALL_STAIR_BRICK)))
        {
            float _hitRange = 0.2f;
            //Đi lên cầu thang 
            RaycastHit hit;
            if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(Vector3.forward), out hit, _hitRange, LayerMask.GetMask(Constant.LAYER_STAIR_BRICK)))
            {
                TargetPoint = new Vector3(hit.collider.transform.position.x, hit.collider.transform.position.y + stepOffset, hit.collider.transform.position.z - _hitRange*2);
                MoveSpeed = moveSpeedStair;
            }
            else
            {
                MoveSpeed = moveSpeed;
                
            }
            transform.position = TargetPoint;
            ChangeAnim("Run");
        }
    }
    private bool isWall(LayerMask _layerMask)
    {
        RaycastHit hit;
        bool isWall = false;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Constant.RAYCAST_HIT_RANGE_WALL, _layerMask))
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
    private bool CheckGrounded(float _rangeCast)
    {
        Debug.DrawLine(transform.position, transform.position + Vector3.down * _rangeCast, Color.red);
        RaycastHit hit;
        return Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, _rangeCast, groundLayer);
    }
    private void StickToGround(float offset_down)
    {
        _rigidbody.transform.position = new Vector3(_rigidbody.transform.position.x, transform.position.y - offset_down, _rigidbody.transform.position.z);
    }
}
