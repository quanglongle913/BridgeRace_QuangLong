using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Character : PooledObject
{


    [SerializeField] private Animator anim;
    [SerializeField] protected LayerMask groundLayer;
    [SerializeField] private float rotationSpeed = 1000f;
    [SerializeField] private float cooldownWindow = 5.0f;

    [Header("Pool Stack Parent  Object: ")]
    [SerializeField] private GameObject brickStackParent;
    [SerializeField] private ObjectPool brick;
    //số gạch Nhân Vật có thể mang tối đa
    [SerializeField] private int maxBrickInCharacter;
    [Header("Player Color: ")]
    [SerializeField] GameObject skinnedMeshRenderer;
    [SerializeField] private ColorData colorData;
    [SerializeField] protected ColorType colorType;


    //danh sách gạch cùng màu với nhân vật ở trên sân
    protected List<GameObject> listBrickInStageCharacterColor;
    //danh sách gạch đc tạo sẵn ở lưng nhân vật
    public List<GameObject> listBrickInCharacter;
    public UnityAction<Character> CreateBrick;

    private SpawnerBrickStage _SpawnerBrickStage;
    protected Stage _stage;
    private string currentAnimName;
    public float meleeRange = 0.01f;
    private int brickCount;

    private int stageLevel = 0;
    private Vector3 targetPoint;
 
    public ColorType ColorType => colorType;
    public ObjectPool Brick => brick;
    public GameObject BrickStackParent => brickStackParent;

    public int MaxBrickInCharacter { get => maxBrickInCharacter; set => maxBrickInCharacter = value; }
    public int BrickCount { get => brickCount; set => brickCount = value; }
    public Vector3 TargetPoint { get => targetPoint; set => targetPoint = value; }
    public int StageLevel { get => stageLevel; set => stageLevel = value; }
    public ColorData ColorData { get => colorData; set => colorData = value; }
    public float CooldownWindow { get => cooldownWindow; set => cooldownWindow = value; }

    public virtual void Awake()
    {
        listBrickInStageCharacterColor = new List<GameObject>();
        listBrickInCharacter = new List<GameObject>();
    }
    private void Start()
    {
        OnInit();
    }
    public virtual void OnInit()
    {
        ChangeColor(skinnedMeshRenderer, colorType);

        //Create Pooling Object in BrickStackParent of Player
        CreateBrick(this.gameObject.GetComponent<Character>());
        brickCount = 0;   
    }

    //ham huy
    public virtual void OnDespawn()
    {
        /*if (brickController != null)
        {
            brickController.AddBrick -= AddBrick;
            //characterControl.Stage -= Stage;
        }*/
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
   
    // khi nhân vật ăn gạch thì Ẩn và hiện gạch sau 1 khoảng TIME 
    protected IEnumerator ActiveBrickCoroutine(float time, GameObject _brick)
    {
        _brick.gameObject.SetActive(false);
        yield return new WaitForSeconds(time);
        //Hiện gạch sau time S
        //Debug.Log(""+ _SpawnerBrickStage.ListColor.Count);
        //random màu trong danh sách màu ở Stage
        int randomIndex = Random.Range(0, _SpawnerBrickStage.ListColor.Count);
        ColorType _colorType = (ColorType)_SpawnerBrickStage.ListColor[randomIndex];

        _brick.gameObject.GetComponent<Brick>().ChangeColor(_colorType);
        _brick.gameObject.SetActive(true);
    }
    private void AddBrick(GameObject birckObj)
    {
        Brick _brick = birckObj.GetComponent<Brick>();
        //kiem tra lai ten cua Brick co tag ="Brick" (Brick tren san)
        if (_brick.ColorType == colorType && _brick.StageLevel==stageLevel)
        {
            //Debug.Log(birckObj.gameObject.GetComponent<Brick>().ColorType);
            if (BrickCount < maxBrickInCharacter)
            {
                StartCoroutine(ActiveBrickCoroutine(cooldownWindow, birckObj));
                BrickCount++;

                //Hiển thị gạch trên lưng nhân vật tương ứng
                for (int i = 0; i < BrickCount; i++)
                {
                    if (!listBrickInCharacter[i].activeSelf)
                    {
                        listBrickInCharacter[i].SetActive(true);
                    }
                }
            }
            else
            {
                //Debug.Log("FULL Stack Brick in:" + transform.gameObject.name);
            }
        }
    }

    public void RemoveBrick()
    {
        BrickCount--;
        listBrickInCharacter[BrickCount].SetActive(false);
    }
    private void ClearBrick()
    {
    }
    private void Stage(Stage stageObject)
    {
        //stageObject.GetComponent<SpawnerBrickStage>().ListColor;
        StageLevel = stageObject.StageLevel;
    }
    public void ChangeColor(GameObject a_obj, ColorType colorType)
    {
        this.colorType = colorType;
        a_obj.GetComponent<SkinnedMeshRenderer>().material = colorData.GetMat(colorType);
    }
    public void OnTriggerEnter(Collider other)
    {   
        if (other.gameObject.GetComponent<Brick>())
        {
           
            AddBrick(other.gameObject);
        }
        if (other.gameObject.GetComponent<Stage>())
        {
            //Debug.Log(other.gameObject.name);
            _stage = other.gameObject.GetComponent<Stage>();
            _SpawnerBrickStage = other.gameObject.GetComponent<SpawnerBrickStage>();
            Stage(other.gameObject.GetComponent<Stage>());
        }
    }
}
