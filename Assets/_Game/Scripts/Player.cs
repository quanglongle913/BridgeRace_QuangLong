﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    [Header("Player Moving:")]
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private FloatingJoystick floatingJoystick;
    [SerializeField] private float moveSpeed = 5.0f;

    [Header("Player Step Clinmb:")]
    [SerializeField] GameObject stepRayLower;
    //[SerializeField] GameObject stepRayUpper;
    //[SerializeField] float stepHeight = 0.3f;
    //[SerializeField] float stepSmooth = 2f;

    private float horizontal;
    private float vertical;
    private float posY;
    public float PosY { get => posY; set => posY = value; }
    public override void Awake()
    {
        base.Awake();
        posY = 0;
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
            ////Chuyển nhân vật về sát mặt đất
            //if (!CheckGrounded(0.1f))
            //{
            //    //Debug.Log("Low1 ");
            //    _rigidbody.transform.position = new Vector3(_rigidbody.transform.position.x, _rigidbody.transform.position.y - 0.045f, _rigidbody.transform.position.z);
            //}
        }
        else ChangeAnim("Idle");

        //else if (CheckGrounded(1.5f))
        //{
        //    //Debug.Log("High ");
        //    _rigidbody.transform.position = new Vector3(_rigidbody.transform.position.x, _rigidbody.transform.position.y - 0.5f, _rigidbody.transform.position.z); ;
        //}

    }
    private void Moving(float _horizontal, float _vertical)
    {
        if (CheckGrounded(0.4f))
        {
            //StepOffset =0.3f
            Vector3 _Direction = new Vector3(_horizontal * moveSpeed, _rigidbody.velocity.y, _vertical * moveSpeed);
            TargetPoint = new Vector3(_rigidbody.position.x + _Direction.x, _rigidbody.position.y + posY, _rigidbody.position.z + _Direction.z);
            RotateTowards(this.gameObject, _Direction);
            if (!isWall(Constant.RAYCAST_HIT_RANGE_WALL, LayerMask.GetMask(Constant.LAYER_WALL_FLOOR)))
            {
                float stepOffet = 0.4f;
                float _hitRange = 0.1f;
                //Đi lên cầu thang 
                //RaycastHit hit;
                //if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(Vector3.forward), out hit, _hitRange, LayerMask.GetMask(Constant.LAYER_STAIR_BRICK)))
                //{
                //    //Debug.Log("Did Hit");
                //    TargetPoint = hit.collider.transform.position;
                //    TargetPoint = new Vector3(TargetPoint.x, TargetPoint.y + stepOffet, TargetPoint.z);
                //    moveTarget(TargetPoint);

                //}
                //RaycastHit hitLower45;
                //if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(1.5f, 0, 1), out hitLower45, _hitRange, LayerMask.GetMask(Constant.LAYER_STAIR_BRICK)))
                //{
                //    //Debug.Log("Did hitLower45");

                //    TargetPoint = hitLower45.collider.transform.position;
                //    TargetPoint = new Vector3(TargetPoint.x, TargetPoint.y + stepOffet, TargetPoint.z);
                //    moveTarget(TargetPoint);
                //}

                //RaycastHit hitLowerMinus45;
                //if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(-1.5f, 0, 1), out hitLowerMinus45, _hitRange, LayerMask.GetMask(Constant.LAYER_STAIR_BRICK)))
                //{
                //    // Debug.Log("Did hitLowerMinus45");
                //    TargetPoint = hitLowerMinus45.collider.transform.position;
                //    TargetPoint = new Vector3(TargetPoint.x, TargetPoint.y + stepOffet, TargetPoint.z);
                //    moveTarget(TargetPoint);
                //}

                //else
                //{
                // di chuyển bình thường
                _rigidbody.transform.position = new Vector3(TargetPoint.x, TargetPoint.y, TargetPoint.z);
                //_rigidbody.transform.position = Vector3.MoveTowards(_rigidbody.transform.position, TargetPoint, moveSpeed * Time.fixedDeltaTime);
                //}
                ChangeAnim("Run");
            }
        }
        else ChangeAnim("Idle");
    }
    public void moveTarget(float posY)
    {
        PosY = posY;
        //_rigidbody.transform.position = new Vector3(_rigidbody.transform.position.x, _TargetPoint.y, _rigidbody.transform.position.z);
        //_rigidbody.transform.position = Vector3.MoveTowards(_rigidbody.transform.position, _TargetPoint, moveSpeed * Time.fixedDeltaTime);
        //EDIT
        //Debug.Log("_TargetPoint.y:= " + _TargetPoint.y);
        //_rigidbody.transform.position = new Vector3(TargetPoint.x, TargetPoint.y, TargetPoint.z);
    }
    private bool isWall(float _hitRange, LayerMask _layerMask)
    {
        RaycastHit hit;
        bool isWall = false;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, _hitRange, _layerMask))
        {
            isWall = true;
            //if (_layerMask == LayerMask.GetMask(Constant.LAYER_WALL_STAIR_BRICK))
            //{
            //    if (BrickCount > 0)
            //    {
            //        hit.collider.gameObject.SetActive(false);
            //    }
            //}
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
        //return Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, _rangeCast, groundLayer);
        //EDit...
        return true;
    }
}
