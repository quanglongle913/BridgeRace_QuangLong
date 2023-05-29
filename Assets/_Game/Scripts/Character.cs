using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using DG.Tweening;
public class Character : PooledObject
{
    [SerializeField] private Animator anim;
    [SerializeField] protected float rotationSpeed = 1000f;
    [SerializeField] private float cooldownWindow = 5.0f;
    [SerializeField] private GameObject endTarget;
    [SerializeField] private LevelManager levelManager;

    [Header("Pool Stack Parent  Object: ")]
    [SerializeField] private GameObject brickStackParent;
    [SerializeField] private ObjectPool brick;
    //số gạch Nhân Vật có thể mang tối đa
    [SerializeField] private int maxBrickInCharacter;
    [Header("Player Color: ")]
    [SerializeField] GameObject skinnedMeshRenderer;
    [SerializeField] private ColorData colorData;
    [SerializeField] protected ColorType colorType;

    [Header("ParticleSystem: ")]
    [SerializeField] private ParticleSystem particleSystem;

    //danh sách gạch cùng màu với nhân vật ở trên sân
    protected List<Brick> listBrickInStageCharacterColor;
    //danh sách gạch đc tạo sẵn ở lưng nhân vật
    public List<GameObject> listBrickInCharacter;
    public UnityAction CreateBrick;
    private Stage stage;
    private string currentAnimName;
    public float meleeRange = 0.01f;
    public int brickCount;

    public int stageLevel = 0;
    private Vector3 targetPoint;
 
    public ColorType ColorType => colorType;
    public ObjectPool Brick => brick;
    public GameObject BrickStackParent => brickStackParent;
    public bool isWin = false;

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
    public LevelManager LevelManager { get => levelManager; set => levelManager = value; }
    public ParticleSystem ParticleSystem { get => particleSystem; set => particleSystem = value; }

    public virtual void Awake()
    {
        listBrickInStageCharacterColor = new List<Brick>();
        listBrickInCharacter = new List<GameObject>();
    }
    public virtual void OnInit()
    {
        //Do Not Set StageLevel = 0;
        ChangeColor(skinnedMeshRenderer, colorType);
        IsWin = false;
        brickCount = 0;
        ClearBrick();
        //Create Pooling Object in BrickStackParent of Player
        CreateBrick();
    }
    /*public virtual void Update()
    {
        OnInit();
    }*/
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

    protected List<Brick> sortListBuyDistance(List<Brick> listObj)
    {
        //sap xep theo khoang cach gan nhat voi BotAI
        Brick brickObject;
        for (int i = 0; i < listObj.Count - 1; i++)
        {
            for (int j = 0; j < listObj.Count; j++)
            {
                if (Vector3.Distance(transform.position, listObj[i].gameObject.transform.position) < Vector3.Distance(transform.position, listObj[j].gameObject.transform.position))
                {
                    brickObject = listObj[i];
                    listObj[i] = listObj[j];
                    listObj[j] = brickObject;
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
    protected IEnumerator ActiveBrickCoroutine(float time, Brick _brick)
    {
        Vector3 pos = _brick.transform.position;
        LevelManager.ListBrickInStage[stageLevel-1].Remove(_brick);
        _brick.GetComponent<PooledObject>().Release();
        //Thêm vị trí vào danh sách trống
        levelManager.ListBrickPosInStage[stageLevel - 1].Add(pos);
        yield return new WaitForSeconds(time);
        //TODO Kiểm tra vị trí đó có trong danh sách trống hay không
        if (checkPosInList(pos, levelManager.ListBrickPosInStage[stageLevel - 1]))
        {            
            int randomIndex = Random.Range(0, stage.ListColor.Count);
            ColorType _colorType = (ColorType)stage.ListColor[randomIndex];
            PooledObject brickObject = Spawner(stage.Brick, stage.BrickParent);
            Brick newBrickInStage = brickObject.GetComponent<Brick>();
            newBrickInStage.ChangeColor(_colorType);
            newBrickInStage.StageLevel = stageLevel;
            newBrickInStage.transform.position = pos;
            newBrickInStage.gameObject.SetActive(true);
            levelManager.ListBrickPosInStage[stageLevel - 1].Remove(pos);
            LevelManager.ListBrickInStage[newBrickInStage.StageLevel - 1].Add(newBrickInStage);
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
    public Ease easeAnim;
    private void AddBrick(Brick _brick)
    {
        if (BrickCount < maxBrickInCharacter)
        {
            BrickCount++;
            int index = BrickCount;
            PooledObject brickObject = Spawner(Brick, BrickStackParent);
            brickObject.GetComponent<BrickCharacter>().ChangeColor(ColorType);
            brickObject.transform.localPosition = new Vector3(0, index + 2, 0);
            brickObject.transform.localScale = new Vector3(1, 0.96f, 1);
            brickObject.gameObject.SetActive(true);
            brickObject.transform.DOMoveY(ListBrickInCharacter[index - 1].transform.position.y, 0.8f)
                //.SetEase(Ease.InElastic)
                .SetEase(easeAnim)
                .SetLoops(0, LoopType.Yoyo)
                .OnComplete(() =>
                {
                    //TODO
                    if (index <= BrickCount) 
                    {
                        ListBrickInCharacter[index - 1].SetActive(true);
                    }
                    brickObject.Release();
                });
            StartCoroutine(ActiveBrickCoroutine(cooldownWindow, _brick));
        }
        else
        {
            //Debug.Log("FULL Stack Brick in:" + transform.gameObject.name);
        }
    }
    private void AddBrickNone(Brick _brick)
    {
        if (BrickCount < maxBrickInCharacter)
        {
            BrickCount++;
            ListBrickInCharacter[BrickCount - 1].SetActive(true);
            LevelManager.ListBrickInStage[stageLevel - 1].Remove(_brick);
            _brick.GetComponent<PooledObject>().Release();
        }
        else
        {
            //Debug.Log("FULL Stack Brick in:" + transform.gameObject.name);
        }
    }
    public List<Vector3> CreatePoolBrickPosMap(Vector3 a_root, int bỉckCount)
    {
        List<Vector3> listPoolBrickPos = new List<Vector3>();
        int Row = Mathf.CeilToInt(Mathf.Sqrt(brickCount));
        //Debug.Log("Brick Count: "+brickCount+" || ROW:"+Row);
        int Column = Row;
        float offset = 1.0f;
        for (int i = 0; i < Row; i++)
        {
            for (int j = 0; j < Column; j++)
            {
                Vector3 birckPosition = new Vector3((j - (Row / 2)) + offset * j + a_root.x, 0.05f + a_root.y, ((Column / 2) - i) - offset * i + a_root.z);
                listPoolBrickPos.Add(birckPosition);
            }
        }
        return listPoolBrickPos;
    }
    public void RandomBrick(Character a_obj)
    {
        List<Vector3> listPoolBrickPos = new List<Vector3>();
        listPoolBrickPos = CreatePoolBrickPosMap(gameObject.transform.position, BrickCount);

        for (int i = 0; i < a_obj.BrickCount; i++)
        {
            int randomIndex = Random.Range(0, listPoolBrickPos.Count);
            Vector3 a_vector3 = listPoolBrickPos[randomIndex];
            PooledObject brickObject = Spawner(stage.Brick, stage.BrickParent);
            brickObject.transform.position = a_vector3;
            Brick brick = brickObject.GetComponent<Brick>();
            brick.ChangeColor(ColorType.None);
            brick.StageLevel = stageLevel;
            listPoolBrickPos.Remove(a_vector3);
            levelManager.ListBrickInStage[stageLevel - 1].Add(brick);
            StartCoroutine(RandomBrickCoroutine(cooldownWindow, brick));
        }
        ClearBrick();
    }
    protected IEnumerator RandomBrickCoroutine(float time, Brick _brick)
    {
        //UNDONE
        yield return new WaitForSeconds(time);
        LevelManager.ListBrickInStage[stageLevel - 1].Remove(_brick);
        if (_brick!=null)
        {
            _brick.GetComponent<PooledObject>().Release();
        }
       
    }
    public void RemoveBrick()
    {
        ListBrickInCharacter[BrickCount - 1].SetActive(false);
        BrickCount--;
    }
    public void ClearBrick()
    {
        for (int i=0;i< ListBrickInCharacter.Count; i++)
        {
            ListBrickInCharacter[i].SetActive(false);
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
            Brick brick = other.gameObject.GetComponent<Brick>();
            if (brick.StageLevel == stageLevel && brick.ColorType == colorType)
            {
                AddBrick(brick);
            }
            else if (brick.StageLevel == stageLevel && brick.ColorType == ColorType.None)
            {
                AddBrickNone(brick);
            }
        }
        if (other.gameObject.GetComponent<Player>())
        {
            Player player = other.gameObject.GetComponent<Player>();

            if (BrickCount < player.BrickCount)
            {
                //Debug.Log("Stun BotAI.......");
                gameObject.GetComponent<BotAI>().ChangeState(new StunState());
            }
            else if (BrickCount > player.BrickCount)
            {
                //Debug.Log("Stun Player.......");
                player.Stuned();  
            }
        }
    }
}
