using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BotAI : Character
{
    [SerializeField] private NavMeshAgent agent;

    private GameObject brickTarget;
    private bool isBrickTarget = true;
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
        if (brickTarget != null) 
        {
            StartCoroutine(MoveCoroutine("Run", 0.2f, brickTarget));
        }
    }
    public bool isTarget()
    {
        List<GameObject> newListBrickObject = sortListBuyDistance(ListBrickObject);
        for (int i = 0; i < getListBrickObjectCount(newListBrickObject); i++)
        {
            if (isBrickTarget)
            {
                if (isActiveObj(newListBrickObject[i]))
                {
                    isBrickTarget = false;
                    brickTarget = getBrickObjectFromList(i, newListBrickObject);
                }
            }
        }
        return !isBrickTarget;
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
    public void StopMoving()
    {
        isBrickTarget = true;
        ChangeAnim("Idle");
    }
    public void Attack() 
    { 

    }
    IEnumerator MoveCoroutine(string animName, float time, GameObject ObjTarget)
    {
        yield return new WaitForSeconds(time);

        ChangeAnim(animName);
        MoveTowards(agent, ObjTarget.transform);
        //RotateTowards(this.gameObject, ObjTarget.transform);
    }
    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.black;
        if (!isBrickTarget)
        {
            Gizmos.DrawLine(brickTarget.transform.position, transform.position);
        }

    }
}
