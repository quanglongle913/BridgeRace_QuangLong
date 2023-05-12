using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] Renderer renderer;
    [SerializeField] ColorData colorData;
    [SerializeField] protected ColorType colorType;

    public ColorType ColorType => colorType;

    public void ChangeColor(ColorType colorType)
    {
        this.colorType = colorType;
        renderer.material = colorData.GetMat(colorType);
    }
}
