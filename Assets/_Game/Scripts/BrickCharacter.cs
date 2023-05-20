using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickCharacter: MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] ColorData colorData;
    [SerializeField] protected ColorType colorType;
    private int stageLevel=0;
    public ColorType ColorType => colorType;

    public int StageLevel { get => stageLevel; set => stageLevel = value; }

    public void ChangeColor(ColorType colorType)
    {
        meshRenderer = GetComponent<MeshRenderer>();
        this.colorType = colorType;
        meshRenderer.material = colorData.GetMat(colorType);
    }
}
