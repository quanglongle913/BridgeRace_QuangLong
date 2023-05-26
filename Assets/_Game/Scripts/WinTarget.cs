using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinTarget : MonoBehaviour
{
    [SerializeField] private List<GameObject> listStair;

    public List<GameObject> ListStair { get => listStair; set => listStair = value; }
}
