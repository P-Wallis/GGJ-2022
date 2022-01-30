using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public static Map _;
    private void Awake()
    {
        if (_ == null)
            _ = this;
    }

    public GameObject iceCubePrefab;
    public GameObject hardyIceCubePrefab;
    public GameObject iceBurstPrefab;
    public GameObject iceBurstInversePrefab;
    public GameObject bombPrefab;
    public GameObject[] gemPrefabs;

    public GameObject crunchSFXPrefab;

    public float cubeSize = 1;
    public int mapSize;
    float halfMap;
    [Range(0,1)] public float fillPercent = 0.5f;
    [Range(0, 1)] public float hardyPercent = 0.5f;

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

    public void CreateCubeAtWorldPosition(Vector3 pos)
    {
        int x = Mathf.RoundToInt((pos.x / cubeSize) + halfMap);
        int y = Mathf.RoundToInt((pos.z / cubeSize) + halfMap);

        if(x >= 0 && x <mapSize && y>=0 && y<mapSize)
        {
            if(iceCubes[x,y] == null)
            {
                Vector3 position = WorldFromArrayPos(x, y);
                GameObject burstGO = Instantiate(iceBurstInversePrefab, position, Quaternion.identity, transform);
                Instantiate(crunchSFXPrefab, position, Quaternion.identity); // play sound
                Destroy(burstGO, 1);
                CreateCubeAtLocation(x, y, false, 0);
            }
            else
            {
                iceCubes[x, y].Freeze();
            }
        }
    }

    void CreateCubeAtLocation(int x, int y, bool hardy = false, float durabilityPercent = 1)
    {
        Vector3 pos = WorldFromArrayPos(x, y);

        GameObject go = Instantiate(hardy ? hardyIceCubePrefab : iceCubePrefab, pos, Quaternion.identity, this.transform);
        iceCubes[x, y] = go.GetComponent<IceCube>();
        iceCubes[x, y].Init(this, x, y, durabilityPercent);
    }

    private Vector3 WorldFromArrayPos(int x, int y)
    {
        return new Vector3((x - halfMap) * cubeSize, iceCubePrefab.transform.position.y, (y - halfMap) * cubeSize);
    }

    public void RemoveCubeAtLocation(int x, int y, bool burst = true)
    {
        if (iceCubes[x,y] != null)
        {
            if (burst)
            {
                GameObject burstGO = Instantiate(iceBurstPrefab, WorldFromArrayPos(x,y), Quaternion.identity);
                Destroy(burstGO, 1);
            }

            if (Random.value < iceCubes[x, y].bombPercent)
            {
                GameObject bombGO = Instantiate(bombPrefab, iceCubes[x, y].transform.position, Quaternion.identity);
                Bomb bomb = bombGO.GetComponent<Bomb>();
                if(bomb!=null)
                {
                    bomb.LightTheFuse();
                }
            }else if (Random.value < iceCubes[x, y].gemPercent)
            {
                GameObject gemGO = Instantiate(gemPrefabs[Random.Range(0,gemPrefabs.Length)], iceCubes[x, y].transform.position, Quaternion.identity);
                gemGO.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
            }

            Destroy(iceCubes[x, y].gameObject);
            iceCubes[x, y] = null;
        }
    }
}
