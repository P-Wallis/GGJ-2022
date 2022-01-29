using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceCube : MonoBehaviour
{
    private int x, y;
    private Map map;

    public float durability;
    private float currentDurability;

    public Transform cubeModel;
    public float minimumScalePercent;

    public void Init(Map map, int x, int y)
    {
        this.map = map;
        this.x = x;
        this.y = y;

        currentDurability = durability;
        cubeModel.localScale = Vector3.one;
    }

    public void Burn()
    {
        currentDurability -= Time.deltaTime;

        cubeModel.localScale = Vector3.one * Mathf.Lerp(minimumScalePercent, 1, currentDurability/durability);

        if (currentDurability <= 0)
        {
            map.RemoveCubeAtLocation(x, y);
        }
    }

}
