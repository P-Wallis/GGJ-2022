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

    [Range(0, 1)] public float bombPercent = 0.1f;
    [Range(0, 1)] public float gemPercent = 0.1f;

    public void Init(Map map, int x, int y, float durabilityPercent = 1)
    {
        this.map = map;
        this.x = x;
        this.y = y;

        SetDurability(durability * durabilityPercent);
    }

    public void Burn()
    {
        SetDurability(currentDurability - Time.deltaTime);


        if (currentDurability <= 0)
        {
            map.RemoveCubeAtLocation(x, y);
        }
    }

    public void Freeze()
    {
        SetDurability(currentDurability + Time.deltaTime);
    }

    void SetDurability(float newDurability)
    {
        currentDurability = Mathf.Clamp(newDurability, 0, durability);
        cubeModel.localScale = Vector3.one * Mathf.Lerp(minimumScalePercent, 1, currentDurability / durability);
    }

}
