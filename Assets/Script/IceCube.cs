using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceCube : MonoBehaviour
{
    private int x, y;
    private Map map;

    public void Init(Map map, int x, int y)
    {
        this.map = map;
        this.x = x;
        this.y = y;
    }
}
