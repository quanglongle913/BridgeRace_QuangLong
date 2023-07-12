using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Image btn_Replay;
    [SerializeField] private Image btn_NextLevel;
    public static UIManager instance;
    private void Awake()
    {
        instance = this;
    }
    public void IsNextButton(bool isBool)
    {
        btn_NextLevel.gameObject.SetActive(isBool);
    }
    public void IsReplayButton(bool isBool)
    {
        btn_Replay.gameObject.SetActive(isBool);
    }
}
