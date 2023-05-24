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
    public Vector3 stairTP;

    public bool IsBrickTarget { get => isBrickTarget; set => isBrickTarget = value; }
    public override void Awake()
    {
        base.Awake();
        stairTP = EndTarget.transform.position;
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
        return BrickCount >= MaxBrickInCharacter;
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
        //List<GameObject> newListBrickObject = sortListBuyDistance(listBrickInStageCharacterColor)
        listBrickInStageCharacterColor.Clear();
        for (int i=0;  i<_stage.ListBrickInStage.Count;i++)
        {
            if (colorType == _stage.ListBrickInStage[i].GetComponent<Brick>().ColorType)
            {
                listBrickInStageCharacterColor.Add(_stage.ListBrickInStage[i]);
            }
        }
        List<GameObject> newListBrickObject = sortListBuyDistance(listBrickInStageCharacterColor);
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
        MoveTowards(agent, stairTP);
    }
    public void Win()
    {
        if (IsDes(EndTarget))
        {
            //Debug.Log("Dance");
            ChangeAnim("Dance");
            Quaternion target = Quaternion.Euler(0, 180, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * 10);
            ClearBrick();
        }
        else 
        {
            ChangeAnim("Run");
            //Debug.Log("Run");
            Vector3 newPos = new Vector3(EndTarget.transform.position.x, EndTarget.transform.position.y + 0.4f, EndTarget.transform.position.z);
            MoveTowards(agent, newPos);
        }
       
    }
    IEnumerator MoveCoroutine(string animName, float time, Vector3 a_Target)
    {
        yield return new WaitForSeconds(time);
        ChangeAnim(animName);
        MoveTowards(agent, a_Target);
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
