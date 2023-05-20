using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Character : MonoBehaviour
{

    [SerializeField] private CharacterTrigger characterTrigger;
    [SerializeField] private Animator anim;
    [SerializeField] protected LayerMask groundLayer;
    [SerializeField] private float rotationSpeed = 1000f;
    [SerializeField] protected float cooldownWindow = 5.0f;

    [Header("Pool Stack Parent  Object: ")]
    [SerializeField] private GameObject brickStackParent;
    [SerializeField] private ObjectPool brick;
    //Index là số gạch Nhân Vật có thể mang tối đa
    [SerializeField] private int maxBrickInCharacter;
    [Header("Player Color: ")]
    [SerializeField] GameObject skinnedMeshRenderer;
    [SerializeField] ColorData colorData;
    [SerializeField] protected ColorType colorType;

    private GameObject brickParent;
    //danh sách gạch cùng màu với nhân vật ở trên sân
    public List<GameObject> listBrickInStageCharacterColor;
    //danh sách gạch đc tạo sẵn ở lưng nhân vật
    public List<GameObject> listBrickInCharacter;
    public UnityAction<Character> CreateBrick;

    private string currentAnimName;
    public float meleeRange = 0.1f;
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
        //StartCoroutine(OnCreateBrickStackPoolingObj(0.2f, maxBrickInCharacter, Brick, colorType, BrickStackParent));
        brickCount = 0;
        if (characterTrigger != null)
        {
            characterTrigger.AddBrick += AddBrick;
            //characterTrigger.Stage += Stage;
        }
        
    }

    //ham huy
    public virtual void OnDespawn()
    {
        if (characterTrigger != null)
        {
            characterTrigger.AddBrick -= AddBrick;
            //characterTrigger.Stage -= Stage;
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
    // khi nhân vật ăn gạch thì Ẩn và hiện gạch sau 1 khoảng TIME 
    protected IEnumerator ActiveBrickCoroutine(float time, GameObject gameObject)
    {

        //gameObject.GetComponent<PooledObject>().Release();
        gameObject.SetActive(false);
        yield return new WaitForSeconds(time);
        //Hiện gạch sau time S
        int randomIndex = Random.Range(1, 5);
        gameObject.GetComponent<Brick>().ChangeColor((int)colorData.);
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
    
    public void ChangeColor(GameObject a_obj, ColorType colorType)
    {
        this.colorType = colorType;
        a_obj.GetComponent<SkinnedMeshRenderer>().material = colorData.GetMat(colorType);
    }

}
