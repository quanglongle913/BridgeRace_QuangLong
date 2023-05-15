using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Character : MonoBehaviour
{
    [SerializeField] Renderer renderer;
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
    protected List<GameObject> listBrickObject;

    private string currentAnimName;
    public float meleeRange = 0.1f;
    protected int brickCount;
    private int stageLevel = 0;
    private void Start()
    {
        OnInit();
    }
    public virtual void OnInit()
    {
        ChangeColor(colorType);
        StartCoroutine(OnInitCoroutine(0.3f));
        if (characterManager != null)
        {
            characterManager.AddBrick += AddBrick;
            characterManager.Stage += Stage;
        }
        //Create Pooling Object in BrickStackParent of Player
        StartCoroutine(OnCreateBrickStackPoolingObj(0.2f));
        brickCount = 0;
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
    protected void MoveTowards(NavMeshAgent agent, Transform target)
    {
        agent.SetDestination(target.position);
    }
    protected List<GameObject> sortListBuyDistance(List<GameObject> listObj)
    {
        //sap xep theo khoang cach gan nhat voi Enemy
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
        Quaternion lookRotation = Quaternion.LookRotation(-direction, Vector3.up);
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
        GenerateNewObject(index);

    }
    private void SetPoolBrickPos()
    {
        if (BrickStackParent.gameObject.transform.childCount > 0)
        {
            for (int i = 0; i < BrickStackParent.gameObject.transform.childCount; i++)
            {
                BrickStackParent.gameObject.transform.GetChild(i).gameObject.transform.localPosition = new Vector3(0, i, 0);
                BrickStackParent.gameObject.transform.GetChild(i).gameObject.transform.localScale = new Vector3(1, 0.96f, 1);
                BrickStackParent.gameObject.transform.GetChild(i).gameObject.SetActive(false);
            }

        }
    }
    // Tạo danh sách Gạch với màu tương ứng Cho Nhân Vật ở trên sân
    protected IEnumerator OnInitCoroutine(float time)
    {

        yield return new WaitForSeconds(time);
        for (int i = 0; i < brickParent.gameObject.transform.childCount; i++)
        {
            if (brickParent.gameObject.transform.GetChild(i).gameObject.GetComponent<Brick>().ColorType == colorType)
            {
                if (listBrickObject != null)
                {
                    listBrickObject.Add(brickParent.gameObject.transform.GetChild(i).gameObject);
                }
            }
        }
    }
    //Create Pooling Object in BrickStackParent of Player
    private void GenerateNewObject(int index)
    {
        for (int i = 0; i < index; i++)
        {
            GameObject brickObject = Brick.GetPooledObject().gameObject;
            //Set Tag of Brick Stack Pool => unDuplicate tags
            brickObject.gameObject.transform.tag = "Brick Stack Object";
            brickObject.GetComponent<Brick>().ChangeColor(colorType);
            if (brickObject == null)
            {
                return;
            }
            brickObject.transform.SetParent(BrickStackParent.transform);
            brickObject.SetActive(true);
        }
        SetPoolBrickPos();
    }
    protected void ActiveBrickForSeconds(float time, GameObject gameObject)
    {
        if (brickCount < index)
        {
            StartCoroutine(ActiveBrickCoroutine(time, gameObject));
            brickCount++;
            //Kiểm tra lại pool gạch trên nhân vật
            if (BrickStackParent.gameObject.transform.childCount == 0)
            {
                Debug.Log("Error create Birck Pooling in Character:" + transform.gameObject.name);
                return;
            }
            else
            {
                for (int i = 0; i < brickCount; i++)
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
        gameObject.SetActive(false);
        yield return new WaitForSeconds(time);
        gameObject.SetActive(true);
    }


    private void AddBrick(GameObject birckObj)
    {
        //kiem tra lai ten cua Brick co tag ="Brick" (Brick tren san)
        if (birckObj.GetComponent<Brick>().ColorType == colorType && birckObj.GetComponent<Brick>().StageLevel==stageLevel)
        {
            //Debug.Log(arg0.gameObject.GetComponent<Brick>().ColorType);
            ActiveBrickForSeconds(cooldownWindow, birckObj.gameObject);

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
        //Debug.Log("stageLevel:"+ stageLevel);
    }
    public ColorType ColorType => colorType;

    public void ChangeColor(ColorType colorType)
    {
        this.colorType = colorType;
        renderer.material = colorData.GetMat(colorType);
    }
}
