using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public GameObject iceCubePrefab;
    public GameObject hardyIceCubePrefab;
    public GameObject iceBurstPrefab;
    public GameObject bombPrefab;
    public GameObject gemPrefab;

    public float cubeSize = 1;
    public int mapSize;
    float halfMap;
    [Range(0,1)] public float fillPercent = 0.5f;
    [Range(0, 1)] public float hardyPercent = 0.5f;
    [Range(0, 1)] public float bombPercent = 0.1f;
    [Range(0, 1)] public float gemPercent = 0.1f;

    private IceCube[,] iceCubes;

    private void Start()
    {
        iceCubes = new IceCube[mapSize, mapSize];
        halfMap = mapSize / 2f;
        for(int x=0; x<mapSize; x++)
        {
            for(int y=0; y<mapSize; y++)
            {
                if(Mathf.Abs(x-halfMap)<1 && Mathf.Abs(y - halfMap)<1)
                {
                    continue;
                }

                if (Random.value < fillPercent)
                {
                    CreateCubeAtLocation(x, y, Random.value < hardyPercent);
                }
            }
        }
    }

    void CreateCubeAtLocation(int x, int y, bool hardy = false)
    {
        Vector3 pos = new Vector3((x - halfMap) * cubeSize, iceCubePrefab.transform.position.y, (y - halfMap) * cubeSize);
        GameObject go = Instantiate(hardy ? hardyIceCubePrefab : iceCubePrefab, pos, Quaternion.identity, this.transform);
        iceCubes[x, y] = go.GetComponent<IceCube>();
        iceCubes[x, y].Init(this, x, y);
    }

    public void RemoveCubeAtLocation(int x, int y, bool burst = true)
    {
        if (iceCubes[x,y] != null)
        {
            if (burst)
            {
                GameObject burstGO = Instantiate(iceBurstPrefab, iceCubes[x, y].transform.position, Quaternion.identity);
                Destroy(burstGO, 1);
            }

            if (Random.value < bombPercent)
            {
                GameObject bombGO = Instantiate(bombPrefab, iceCubes[x, y].transform.position, Quaternion.identity);
                Bomb bomb = bombGO.GetComponent<Bomb>();
                if(bomb!=null)
                {
                    bomb.LightTheFuse();
                }
            }

            Destroy(iceCubes[x, y].gameObject);
            iceCubes[x, y] = null;
        }
    }
}
