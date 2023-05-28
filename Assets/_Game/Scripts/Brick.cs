using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Brick : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private ColorData colorData;
    [SerializeField] protected ColorType colorType;
    public int stageLevel=0;
    public ColorType ColorType => colorType;

    public int StageLevel { get => stageLevel; set => stageLevel = value; }

    public void ChangeColor(ColorType colorType)
    {
        this.colorType = colorType;
       
        meshRenderer.material = colorData.GetMat(colorType);
    }
    public void ChangeColorColorLight(ColorType colorType)
    {
        this.colorType = colorType;

        //meshRenderer.material = colorData.GetMat(colorType);
        meshRenderer.material.DOColor(colorData.GetMat(colorType).color, 1f);
    }
}
