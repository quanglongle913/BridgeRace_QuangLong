using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    [Header("Player Moving:")]
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private FloatingJoystick floatingJoystick;
    [SerializeField] private float moveSpeed = 5.0f;

    [Header ("Player Step Clinmb:")]
    [SerializeField] GameObject stepRayLower;
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
      
    }
    public void FixedUpdate()
    {
        horizontal = floatingJoystick.Horizontal;
        vertical = floatingJoystick.Vertical;

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
                TargetPoint = new Vector3(hit.collider.transform.position.x, hit.collider.transform.position.y + stepOffset, hit.collider.transform.position.z- _hitRange);
            }
            /*RaycastHit hitLower45;
            if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(1.5f, 0, 1), out hitLower45, _hitRange, LayerMask.GetMask(Constant.LAYER_STAIR_BRICK)))
            {
                Debug.Log("hitLower45");
                TargetPoint = new Vector3(hitLower45.collider.transform.position.x, hitLower45.collider.transform.position.y + stepOffset, hitLower45.collider.transform.position.z-_hitRange);
            }

            RaycastHit hitLowerMinus45;
            if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(-1.5f, 0, 1), out hitLowerMinus45, _hitRange, LayerMask.GetMask(Constant.LAYER_STAIR_BRICK)))
            {
                Debug.Log("hitLowerMinus45");
                TargetPoint = new Vector3(hitLowerMinus45.collider.transform.position.x, hitLowerMinus45.collider.transform.position.y + stepOffset, hitLowerMinus45.collider.transform.position.z-_hitRange);
            }*/
            transform.position = new Vector3(TargetPoint.x, TargetPoint.y, TargetPoint.z);
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
