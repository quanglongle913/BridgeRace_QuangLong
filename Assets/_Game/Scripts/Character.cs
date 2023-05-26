using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Character : PooledObject
{


    [SerializeField] private Animator anim;
    [SerializeField] protected LayerMask groundLayer;
    [SerializeField] protected float rotationSpeed = 1000f;
    [SerializeField] private float cooldownWindow = 5.0f;
    [SerializeField] private GameObject endTarget;

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
    private List<GameObject> listBrickInCharacter;
    public UnityAction<Character> CreateBrick;
    private Stage stage;
    private string currentAnimName;
    public float meleeRange = 0.01f;
    private int brickCount;

    private int stageLevel = 0;
    private Vector3 targetPoint;
 
    public ColorType ColorType => colorType;
    public ObjectPool Brick => brick;
    public GameObject BrickStackParent => brickStackParent;
    private bool isWin = false;

    public GameObject EndTarget { get => endTarget; set => endTarget = value; }
    public List<GameObject> ListBrickInCharacter { get => listBrickInCharacter; set => listBrickInCharacter = value; }
    public bool IsWin { get => isWin; set => isWin = value; }
    public int MaxBrickInCharacter { get => maxBrickInCharacter; set => maxBrickInCharacter = value; }
    public int BrickCount { get => brickCount; set => brickCount = value; }
    public Vector3 TargetPoint { get => targetPoint; set => targetPoint = value; }
    public int StageLevel { get => stageLevel; set => stageLevel = value; }
    public ColorData ColorData { get => colorData; set => colorData = value; }
    public float CooldownWindow { get => cooldownWindow; set => cooldownWindow = value; }
    public Stage Stage { get => stage; set => stage = value; }

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
        listBrickInCharacter.Clear();
        listBrickInStageCharacterColor.Clear();
        if (Stage != null)
            Stage.ListBrickInStage.Clear();
        IsWin = false;
        brickCount = 0;
        StageLevel = 0;
        //Create Pooling Object in BrickStackParent of Player
        CreateBrick(gameObject.GetComponent<Character>());

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
    protected bool IsDes(GameObject gameObject)
    {
        float distance = Vector3.Distance(transform.position, gameObject.transform.position);
        return distance < 2.0f;
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
    protected void RotateTowards(GameObject gameObject, Vector3 direction)
    {
       // transform.rotation = Quaternion.LookRotation(direction);
        Quaternion lookRotation = Quaternion.LookRotation(direction, Vector3.up);
        gameObject.transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    // khi nhân vật ăn gạch thì Ẩn và hiện gạch sau 1 khoảng TIME 
    protected IEnumerator ActiveBrickCoroutine(float time, GameObject _brick)
    {
        //TODO trả về pool
        Vector3 pos = _brick.transform.position;
        stage.ListBrickInStage.Remove(_brick.GetComponent<PooledObject>().gameObject);
        _brick.GetComponent<PooledObject>().Release();
        stage.ListPoolBrickPos.Add(pos);
        yield return new WaitForSeconds(time);
        //Hiện gạch sau time S
        //Debug.Log(""+ _SpawnerBrickStage.ListColor.Count);
        if (checkPosInList(pos,stage.ListPoolBrickPos))
        {
            //random màu trong danh sách màu ở Stage
            ColorType _colorType;
            if (stage)
            {
                int randomIndex = Random.Range(0, stage.GetComponent<SpawnerBrickStage>().ListColor.Count);
                 _colorType = (ColorType)stage.GetComponent<SpawnerBrickStage>().ListColor[randomIndex];
            }
            else
            {
                 _colorType = (ColorType) 0;
            }
            //TODO Kiểm tra vị trí đó có gạch hay chưa? if Có:=>ko tạo else -> tạo gạch
            PooledObject brickObject = Spawner(stage.Brick, stage.BrickParent);
            Brick newBrickInStage = brickObject.GetComponent<Brick>();
            newBrickInStage.ChangeColor(_colorType);
            newBrickInStage.StageLevel = stageLevel;
            newBrickInStage.transform.position = pos;
            newBrickInStage.gameObject.SetActive(true);
            stage.ListBrickInStage.Add(brickObject.gameObject);
        }
    }
    private bool checkPosInList(Vector3 pos, List<Vector3> a_List)
    {
        for (int i = 0; i < a_List.Count; i++)
        {
            if (pos == a_List[i])
            {
                return true;
            }
        }
        return false;
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
                    //Debug.Log("Stack Brick in:" + listBrickInCharacter[i].name);
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
    public void ClearBrick()
    {
        for (int i=0;i<BrickCount;i++)
        {
            
            listBrickInCharacter[i].SetActive(false);
        }
        BrickCount = 0;
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
        if (other.gameObject.TryGetComponent<Stage>(out var stage))
        {
            //Debug.Log(other.gameObject.name);
            this.stage = stage;
            StageLevel = stage.StageLevel;
            if (StageLevel == 2 && gameObject.GetComponent<BotAI>())
            {
                MaxBrickInCharacter = 20;
            }
        }
    }
}
