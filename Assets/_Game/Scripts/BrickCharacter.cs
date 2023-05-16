using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickCharacter: MonoBehaviour
{
    [HideInInspector] [SerializeField] new Renderer renderer;
    [SerializeField] ColorData colorData;
    [SerializeField] protected ColorType colorType;
    private int stageLevel=0;
    public ColorType ColorType => colorType;

    public int StageLevel { get => stageLevel; set => stageLevel = value; }

    public void ChangeColor(ColorType colorType)
    {
        renderer = GetComponent<MeshRenderer>();
        this.colorType = colorType;
        renderer.material = colorData.GetMat(colorType);
    }
}
