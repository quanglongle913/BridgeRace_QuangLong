using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BotAI : Character
{
    [SerializeField] private NavMeshAgent agent;

    private bool isBrickTarget = false;
    private IState currentState;

    public bool IsBrickTarget { get => isBrickTarget; set => isBrickTarget = value; }
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        //agent.updateRotation = false;
        ListBrickObject = new List<GameObject>();
    }

    public override void OnInit()
    {
        base.OnInit();
        ChangeState(new IdleState());
    }
    public void Update()
    {
        if (currentState != null)
        {
            currentState.OnExecute(this);
        }
    }
    public void ChangeState(IState newState)
    {
        if (currentState!=null)
        {
            currentState.OnExit(this);
        }
        currentState = newState;
        if (currentState != null)
        {
            currentState.OnEnter(this);
        }
    }
    public bool isEnoughBrick()
    {
        return BrickCount >= index;
    }
    public void Moving()
    {
        if (TargetPoint != null) 
        {
            StartCoroutine(MoveCoroutine("Run", 0.2f, TargetPoint));
        }
    }
    public Vector3 getTarget()
    {
        List<GameObject> newListBrickObject = sortListBuyDistance(ListBrickObject);
        Vector3 BrickTarget = TargetPoint;
        for (int i = 0; i < getListBrickObjectCount(newListBrickObject); i++)
        {
            if (!isBrickTarget)
            {
                if (isActiveObj(newListBrickObject[i]))
                {
                    isBrickTarget = true;
                    BrickTarget = getBrickObjectFromList(i, newListBrickObject).transform.position;
                }
            }
        }
        return BrickTarget;
    }
    private GameObject getBrickObjectFromList(int index, List<GameObject> listBrickObject)
    {
        return listBrickObject[index];
    }
    private bool isActiveObj(GameObject gameObj)
    {
        return gameObj.activeSelf;
    }
    private int getListBrickObjectCount(List<GameObject> listObj)
    {
        return listObj.Count;
    }
    public bool isDestination()
    {
        return IsDes();
    }
    public void setTarget(GameObject _target)
    {
        TargetPoint = _target.transform.position;
    }
    public void StopMoving()
    {
        isBrickTarget = true;
        ChangeAnim("Idle");
    }
    public void Attack() 
    { 

    }
    IEnumerator MoveCoroutine(string animName, float time, Vector3 a_Target)
    {
        yield return new WaitForSeconds(time);

        ChangeAnim(animName);
        MoveTowards(agent, a_Target);
        
        //RotateTowards(this.gameObject, ObjTarget.transform);
    }
    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.black;
        if (!isBrickTarget)
        {
            Gizmos.DrawLine(TargetPoint, transform.position);
        }

    }
}
