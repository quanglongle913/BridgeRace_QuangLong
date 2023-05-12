using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectColor : MonoBehaviour
{
    [SerializeField] Renderer renderer;
    [SerializeField] ColorData colorData;
    ColorType colorType;

    public ColorType ColorType => colorType;

    public void ChangeColor(ColorType colorType)
    {
        this.colorType = colorType;
        renderer.material = colorData.GetMat(colorType);
    }
}
