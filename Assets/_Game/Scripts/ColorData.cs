using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ColorData", menuName = "ScriptableObjects/ColorData", order = 1)]
public class ColorData : ScriptableObject
{
    [SerializeField] private Material[] mats;

    public Material[] Mats { get => mats; set => mats = value; }

    public Material GetMat(ColorType colorType)
    {
        return mats[(int)colorType];
    }
}
