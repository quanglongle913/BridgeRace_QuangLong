using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Character : PoolingSpawner
{
    [SerializeField] GameObject skinnedMeshRenderer;
    [SerializeField] ColorData colorData;
    [SerializeField] protected ColorType colorType;
    [SerializeField] private CharacterAction characterAction;
    [SerializeField] private Animator anim;
    

    //[SerializeField] protected GameObject brickTargetObj;
    [SerializeField] private float rotationSpeed = 1000f;
    [SerializeField] protected float cooldownWindow = 5.0f;

    [Tooltip("Pool Stack Parent  Object")]
    [SerializeField] private GameObject BrickStackParent;
    [SerializeField] private ObjectPool Brick;
    //Index là số gạch Nhân Vật có thể mang tối đa
    [SerializeField] protected int maxBrickInCharacter;

    private GameObject brickParent;
    //danh sách gạch cùng màu với nhân vật ở trên sân
    protected List<GameObject> ListBrickInStageCharacterColor;
    //danh sách gạch đc tạo sẵn ở lưng nhân vật
    protected List<GameObject> ListBrickInCharacter;

    private string currentAnimName;
    public float meleeRange = 0.1f;
    private int brickCount;
    private int stageLevel = 0;
    private Vector3 targetPoint;

    public ColorType ColorType => colorType;

    protected int BrickCount { get => brickCount; set => brickCount = value; }
    public Vector3 TargetPoint { get => targetPoint; set => targetPoint = value; }


    private void Awake()
    {
        ListBrickInStageCharacterColor = new List<GameObject>();
        ListBrickInCharacter = new List<GameObject>();
    }
    private void Start()
    {
        OnInit();
    }
    public virtual void OnInit()
    {
        ChangeColor(skinnedMeshRenderer, colorType);

        //Create Pooling Object in BrickStackParent of Player
        StartCoroutine(OnCreateBrickStackPoolingObj(0.2f, maxBrickInCharacter, Brick, colorType, BrickStackParent));
        brickCount = 0;
        if (characterAction != null)
        {
            characterAction.AddBrick += AddBrick;
            characterAction.Stage += Stage;
        }
        
    }

    //ham huy
    public virtual void OnDespawn()
    {
        if (characterAction != null)
        {
            characterAction.AddBrick -= AddBrick;
            characterAction.Stage -= Stage;
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
    protected IEnumerator OnCreateBrickStackPoolingObj(float time, int _maxBrick, ObjectPool a_brick,ColorType a_colortype , GameObject a_root)
    {
        yield return new WaitForSeconds(time);
        for (int i = 0; i < _maxBrick; i++)
        {
            //Spawn Brick
            PooledObject brickObject = Spawner(a_brick, a_root, a_colortype);
            brickObject.transform.localPosition = new Vector3(0, i, 0);
            brickObject.transform.localScale = new Vector3(1, 0.96f, 1);
            brickObject.gameObject.SetActive(false);
            //Add to List
            ListBrickInCharacter.Add(brickObject.gameObject);
        }
    }
    

    protected void ActiveBrickForSeconds(float time, GameObject gameObject)
    {
        //DONE Không được dùng Parent.transform.GetChild(i)
        if (BrickCount < maxBrickInCharacter)
        {
            StartCoroutine(ActiveBrickCoroutine(time, gameObject));
            BrickCount++;

            //Hiển thị gạch trên lưng nhân vật tương ứng
            for (int i = 0; i < BrickCount; i++)
            {
                if (!ListBrickInCharacter[i].activeSelf)
                {
                    ListBrickInCharacter[i].SetActive(true);
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
        Brick _birck = birckObj.GetComponent<Brick>();
        //kiem tra lai ten cua Brick co tag ="Brick" (Brick tren san)
        if (_birck.ColorType == colorType && _birck.StageLevel==stageLevel)
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
    private void Stage(Stage stage)
    {
        Debug.Log("Stage Enter");
        stageLevel = stage.StageLevel;
        brickParent = stage.BrickParent;
        int _poolSize = stage.Row * stage.Column;
        StartCoroutine(InitSpawnObjectWithColor(0.5f, colorType, stageLevel, _poolSize, stage.Brick, brickParent, stage.ListPoolBrickPos));
        //StartCoroutine(OnInitCoroutine(0.5f, brickParent));
    }
    //Tạo gạch trên sân tương ứng với màu của Character
    protected IEnumerator InitSpawnObjectWithColor(float time, ColorType colorType, int stageLevel, int poolSize, ObjectPool Brick, GameObject PoolParent,List<Vector3> ListPoolBrickPos)
    {
        yield return new WaitForSeconds(time);
        int num_Count = getListVector3Count(ListPoolBrickPos);
        if (num_Count > 0)
        {
            for (int j = 0; j < poolSize / 4; j++)
            {
                //Tạo và Thêm đối tượng vào danh sách Gạch với màu tương ứng Cho Nhân Vật ở trên sân
                ListBrickInStageCharacterColor.Add(SpawnObjectWithColor(colorType, stageLevel, Brick, PoolParent, ListPoolBrickPos).gameObject);
            }
        }
    }
    public void ChangeColor(GameObject a_obj, ColorType colorType)
    {
        this.colorType = colorType;
        a_obj.GetComponent<SkinnedMeshRenderer>().material = colorData.GetMat(colorType);
    }

}
