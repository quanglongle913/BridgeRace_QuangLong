using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class BotAI : Character
{
    [Header("NavMeshAgent: ")]
    [SerializeField] private NavMeshAgent agent;

    private bool isBrickTarget = false;
    private IState currentState;
    private Vector3 stairTP;
    public UnityAction WinAction;
    public bool IsBrickTarget { get => isBrickTarget; set => isBrickTarget = value; }
    public NavMeshAgent Agent { get => agent; set => agent = value; }
    public Vector3 StairTP { get => stairTP; set => stairTP = value; }


    public override void Awake()
    {
        base.Awake();
        //stairTP = EndTarget.transform.position;
    }
    //Called in LevelManager
  /*  private void Start()
    {
        OnInit();
    }*/
    public override void OnInit()
    {
        base.OnInit();
        ChangeState(new IdleState());
        //StartCoroutine(OnInitBotAI(2f));
    }
    public void Update()
    {
        //base.Update();
        if (currentState != null && LevelManager.GameState == GameState.Ingame)
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
    public Vector3 getTarget(int index)
    {
        //List<GameObject> newListBrickObject = sortListBuyDistance(listBrickInStageCharacterColor)
        //UNDONE
        listBrickInStageCharacterColor.Clear();
        //Debug.Log("stageLevel:" + stageLevel + "--Brick Count:" +LevelManager.ListBrickInStage[index].Count);
        if (LevelManager.ListBrickInStage[index].Count != 0)
        {
            for (int i = 0; i < LevelManager.ListBrickInStage[index].Count; i++)
            {
                if (colorType == LevelManager.ListBrickInStage[index][i].GetComponent<Brick>().ColorType)
                {
                    listBrickInStageCharacterColor.Add(LevelManager.ListBrickInStage[index][i]);
                } else if (colorType==ColorType.None)
                {
                    listBrickInStageCharacterColor.Add(LevelManager.ListBrickInStage[index][i]);
                }
            }
        }
        
        List<Brick> newListBrickObject = SortListBuyDistance(listBrickInStageCharacterColor);
        Vector3 BrickTarget = TargetPoint;
        for (int i = 0; i < getListBrickObjectCount(newListBrickObject); i++)
        {
            if (!isBrickTarget)
            {
                if (newListBrickObject[i].gameObject.activeSelf)
                {
                    isBrickTarget = true;
                    BrickTarget = newListBrickObject[i].transform.position;
                }
            }
        }
        return BrickTarget;
    }
    private int getListBrickObjectCount(List<Brick> listObj)
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
    public void StopAll()
    {
        ClearBrick();
        ChangeState(new LoseState());
        ChangeAnim("Idle");
    }
    public void Stun()
    {
        ////TODO BrickCount =0 and random Brick to Stage
        RandomBrick(gameObject.GetComponent<Character>());
        isBrickTarget = true;
        ChangeAnim("Falling");
    }
    public void Attack()
    {
        agent.SetDestination(stairTP);
    }
    public void Won()
    {
        if (IsDes(EndTarget))
        {
            ClearBrick();
            WinAction();
            ChangeState(new WonState());
            ChangeAnim("Dance");
            IsWin = false;
        }
        else 
        {
            ChangeAnim("Run");
            //Debug.Log("Run");
            Vector3 newPos = new Vector3(EndTarget.transform.position.x, EndTarget.transform.position.y + 0.4f, EndTarget.transform.position.z);
            agent.SetDestination(newPos);
        }
       
    }
    IEnumerator MoveCoroutine(string animName, float time, Vector3 a_Target)
    {
        yield return new WaitForSeconds(time);
        ChangeAnim(animName);
        agent.SetDestination(a_Target);

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
    protected IEnumerator OnInitBotAI(float time)
    {
        yield return new WaitForSeconds(time);
        ChangeState(new IdleState());
    }
}
