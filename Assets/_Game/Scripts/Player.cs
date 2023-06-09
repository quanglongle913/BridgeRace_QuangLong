﻿using System.Collections;
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
    public bool isStun;
    private float vertical;
    float stepOffset = 0.36f;
    public override void Awake()
    {
        base.Awake();
    }
    //Called in LevelManager
    /*  private void Start()
      {
          OnInit();
      }*/
    public override void OnInit()
    {
        base.OnInit();
        isStun = false;
        MoveSpeed = moveSpeed;
        if (isStun)
        {
            gameObject.GetComponent<Player>().ParticleSystem.Play();
        }
        else if (!isStun)
        {
            gameObject.GetComponent<Player>().ParticleSystem.Stop();
        }
    }
  /*  public override void Update()
    {
        base.Update();
    }*/
    public void FixedUpdate()
    {
        if (floatingJoystick.GetComponent<FloatingJoystick>().enabled)
        {
            horizontal = floatingJoystick.Horizontal;
            vertical = floatingJoystick.Vertical;
        }
        if (IsWin)
        {
            Won();
            IsWin = false;
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
                    if (!isStun)
                    { 
                        ChangeAnim("Idle");
                    }
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
       /* if (isStun)
        {
            gameObject.GetComponent<Player>().ParticleSystem.Play();
        }
        else if(!isStun)
        {
            gameObject.GetComponent<Player>().ParticleSystem.Stop();
        }*/
    }
    public void Lose()
    {
        ClearBrick();
        floatingJoystick.OnReset();
        Debug.Log("You Losed");
    }
    private void Won()
    {
        ClearBrick();
        transform.position = EndTarget.transform.position;
        Debug.Log("Win");
        transform.position = new Vector3(transform.position.x,transform.position.y +0.2f,transform.position.z);
        Quaternion target = Quaternion.Euler(0, 180, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * rotationSpeed);
        ChangeAnim("Dance");
        floatingJoystick.OnReset();
        WinAction();
       
    }
    private void Moving(float _horizontal, float _vertical)
    {
        if (LevelManager.GameState == GameState.Ingame && !isStun)
        {
            Vector3 _Direction = new Vector3(_horizontal * MoveSpeed * Time.fixedDeltaTime, _rigidbody.velocity.y, _vertical * MoveSpeed * Time.fixedDeltaTime);
            TargetPoint = new Vector3(_rigidbody.position.x + _Direction.x, _rigidbody.position.y, _rigidbody.position.z + _Direction.z);
            RotateTowards(this.gameObject, _Direction);
            if (!isWall(LayerMask.GetMask(Constant.LAYER_WALL_FLOOR)) && !isWall(LayerMask.GetMask(Constant.LAYER_WALL_STAIR_BRICK)))
            {
                float _hitRange = 0.2f;
                //Đi lên cầu thang 
                RaycastHit hit;
                if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(Vector3.forward), out hit, _hitRange, LayerMask.GetMask(Constant.LAYER_STAIR_BRICK)))
                {
                    MoveSpeed = moveSpeedStair;
                    //TargetPoint = new Vector3(hit.collider.transform.position.x, hit.collider.transform.position.y + stepOffset, hit.collider.transform.position.z - _hitRange);
                    TargetPoint = new Vector3(hit.collider.transform.position.x, hit.collider.transform.position.y + stepOffset, hit.point.z);
                }
                else
                {
                    MoveSpeed = moveSpeed;

                }
                transform.position = TargetPoint;
                ChangeAnim("Run");
            }
        }
       
    }
    public void StopMoving()
    {
        ChangeAnim("Idle");
    }
    public void Stuned()
    {
        //TODO BrickCount =0 and random Brick to Stage
        RandomBrick(gameObject.GetComponent<Character>());
        ChangeAnim("Falling");
        StartCoroutine(StunCoroutine(Random.Range(2.0f, 3.5f)));
    }
    private IEnumerator StunCoroutine(float time)
    {
        isStun = true;
        gameObject.GetComponent<Player>().ParticleSystem.Play();
        yield return new WaitForSeconds(time);
        isStun = false;
        gameObject.GetComponent<Player>().ParticleSystem.Stop();
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
        return Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, _rangeCast, LayerMask.GetMask(Constant.LAYER_GROUND));
    }
    private void StickToGround(float offset_down)
    {
        _rigidbody.transform.position = new Vector3(_rigidbody.transform.position.x, transform.position.y - offset_down, _rigidbody.transform.position.z);
    }
}
