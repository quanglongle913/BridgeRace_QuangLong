using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Character : PoolingSpawner
{
    [SerializeField] GameObject skinnedMeshRenderer;
    [SerializeField] ColorData colorData;
    [SerializeField] protected ColorType colorType;
    [SerializeField] private CharacterManager characterManager;
    [SerializeField] private Animator anim;
    

    //[SerializeField] protected GameObject brickTargetObj;
    [SerializeField] private float rotationSpeed = 1000f;
    [SerializeField] protected float cooldownWindow = 5.0f;

    [Tooltip("Pool Stack Parent  Object")]
    [SerializeField] private GameObject BrickStackParent;
    [SerializeField] private ObjectPool Brick;
    //Index là số gạch Nhân Vật có thể mang tối đa
    [SerializeField] protected int index;

    private GameObject brickParent;
    protected List<GameObject> ListBrickObject;

    private string currentAnimName;
    public float meleeRange = 0.1f;
    private int brickCount;
    private int stageLevel = 0;
    private Vector3 targetPoint;
    private bool isCreatePoolBrickPosMap=false;
    private void Start()
    {
        OnInit();
    }
    public virtual void OnInit()
    {
        ChangeColor(skinnedMeshRenderer, colorType);

        //Create Pooling Object in BrickStackParent of Player
        StartCoroutine(OnCreateBrickStackPoolingObj(0.2f));
        brickCount = 0;
        if (characterManager != null)
        {
            characterManager.AddBrick += AddBrick;
            characterManager.Stage += Stage;
        }
        
    }

    //ham huy
    public virtual void OnDespawn()
    {
        if (characterManager != null)
        {
            characterManager.AddBrick -= AddBrick;
            characterManager.Stage -= Stage;
        }
    }

    protected void ChangeAnim(string animName)
    {

        if (currentAnimName != animName)
        {
            anim.ResetTrigger(animName);
            currentAnimName = animName;
            anim.SetTrigger(currentAnimName);
        }
    }
    protected bool IsInMeleeRangeOf(Transform target)
    {
        float distance = Vector3.Distance(transform.position, target.position);
        return distance < meleeRange;
    }
    protected bool IsDes()
    {
        float distance = Vector3.Distance(transform.position, TargetPoint);
        return distance < meleeRange;
    }
    protected void MoveTowards(NavMeshAgent agent, Vector3 target)
    {
        agent.SetDestination(target);
    }
    protected List<GameObject> sortListBuyDistance(List<GameObject> listObj)
    {
        //sap xep theo khoang cach gan nhat voi BotAI
        GameObject gameObject;
        for (int i = 0; i < listObj.Count - 1; i++)
        {
            for (int j = 0; j < listObj.Count; j++)
            {
                if (Vector3.Distance(transform.position, listObj[i].gameObject.transform.position) < Vector3.Distance(transform.position, listObj[j].gameObject.transform.position))
                {
                    gameObject = listObj[i];
                    listObj[i] = listObj[j];
                    listObj[j] = gameObject;
                }
            }
        }
        return listObj;
    }        
    
    //BotAI
    protected void RotateTowards(GameObject gameObject, Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        //Quaternion lookRotation = Quaternion.LookRotation(direction);
        Quaternion lookRotation = Quaternion.LookRotation(direction, Vector3.up);
        gameObject.transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

    }
    //Player
    protected void RotateTowards(GameObject gameObject, Vector3 direction)
    {
        transform.rotation = Quaternion.LookRotation(direction);
    }
    protected IEnumerator OnCreateBrickStackPoolingObj(float time)
    {
        yield return new WaitForSeconds(time);
        Spawner(index, Brick, BrickStackParent,colorType);
    }
    

    protected void ActiveBrickForSeconds(float time, GameObject gameObject)
    {
        if (BrickCount < index)
        {
            StartCoroutine(ActiveBrickCoroutine(time, gameObject));
            BrickCount++;
            //Kiểm tra lại pool gạch trên nhân vật
            if (BrickStackParent.gameObject.transform.childCount == 0)
            {
                Debug.Log("Error create Birck Pooling in Character:" + transform.gameObject.name);
                return;
            }
            else
            {
                for (int i = 0; i < BrickCount; i++)
                {
                    if (!BrickStackParent.gameObject.transform.GetChild(i).gameObject.activeSelf)
                    {
                        BrickStackParent.gameObject.transform.GetChild(i).gameObject.SetActive(true);
                    }
                }
            }
        }
        else
        {
            //Debug.Log("FULL Stack Brick in:" + transform.gameObject.name);
        }
    }
    // khi nhân vật ăn gạch thì Ẩn và hiện gạch sau 1 khoảng TIME 
    protected IEnumerator ActiveBrickCoroutine(float time, GameObject gameObject)
    {

        //gameObject.GetComponent<PooledObject>().Release();
        gameObject.SetActive(false);
        yield return new WaitForSeconds(time);
        //Hiện gạch sau time S
        gameObject.SetActive(true);
    }


    private void AddBrick(GameObject birckObj)
    {
        //kiem tra lai ten cua Brick co tag ="Brick" (Brick tren san)
        if (birckObj.GetComponent<Brick>().ColorType == colorType && birckObj.GetComponent<Brick>().StageLevel==stageLevel)
        {
            //Debug.Log(birckObj.gameObject.GetComponent<Brick>().ColorType);
            ActiveBrickForSeconds(cooldownWindow, birckObj);
        }
    }

    private void RemoveBrick()
    {

    }
    private void ClearBrick()
    {

    }
    private void Stage(GameObject stage)
    {
        stageLevel = stage.gameObject.GetComponent<Stage>().StageLevel;
        brickParent = stage.gameObject.GetComponent<Stage>().BrickParent;
        int Row = stage.gameObject.GetComponent<Stage>().Row;
        int Column = stage.gameObject.GetComponent<Stage>().Column;
        float offset = stage.gameObject.GetComponent<Stage>().Offset;
        ObjectPool Brick = stage.gameObject.GetComponent<Stage>().Brick;
        List<Vector3> ListPoolBrickPos = stage.gameObject.GetComponent<Stage>().ListPoolBrickPos;
        StartCoroutine(InitSpawnObjectWithColor(0.5f, colorType, stageLevel, Row * Column, Brick, brickParent, ListPoolBrickPos));
        StartCoroutine(OnInitCoroutine(0.5f, brickParent));
    }
    //Tạo gạch trên sân tương ứng với màu của Character
    protected IEnumerator InitSpawnObjectWithColor(float time, ColorType colorType, int stageLevel, int poolSize, ObjectPool Brick, GameObject PoolParent,List<Vector3> ListPoolBrickPos)
    {
        yield return new WaitForSeconds(time);
        //Debug.Log("InitSpawnObjectWithColor");
        SpawnObjectWithColor(colorType, stageLevel, poolSize, Brick, PoolParent, ListPoolBrickPos);
    }
    // Tạo danh sách Gạch với màu tương ứng Cho Nhân Vật ở trên sân
    protected IEnumerator OnInitCoroutine(float time, GameObject Parent)
    {

        yield return new WaitForSeconds(time);
        //Debug.Log("OnInitCoroutine");
        Debug.Log("" + Parent.transform.childCount);
        for (int i = 0; i < Parent.transform.childCount; i++)
        {
            if (Parent.transform.GetChild(i).gameObject.GetComponent<Brick>().ColorType == colorType)
            {
                if (ListBrickObject != null)
                {
                    ListBrickObject.Add(Parent.transform.GetChild(i).gameObject);
                }
            }
        }
    }
    public ColorType ColorType => colorType;

    protected int BrickCount { get => brickCount; set => brickCount = value; }
    public Vector3 TargetPoint { get => targetPoint; set => targetPoint = value; }

    public void ChangeColor(GameObject a_obj, ColorType colorType)
    {
        this.colorType = colorType;
        a_obj.GetComponent<SkinnedMeshRenderer>().material = colorData.GetMat(colorType);
    }
}
