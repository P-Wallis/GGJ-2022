using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float fuseLength = 1.5f;
    public MeshRenderer bombMesh;
    public GameObject explosion;

    private MaterialPropertyBlock mpb;
    private Color bombStartColor;
    public void LightTheFuse()
    {
        mpb = new MaterialPropertyBlock();
        bombStartColor = bombMesh.material.color;
        StartCoroutine(Explode(fuseLength));
    }

    IEnumerator Explode(float time)
    {
        float percent = 0;
        float dt = 1f / time;

        while(percent<1)
        {
            if((percent * 10) % 2 < 1)
            {
                mpb.SetColor("_BaseColor", bombStartColor);
            }
            else
            {
                mpb.SetColor("_BaseColor", Color.red);
            }
            bombMesh.SetPropertyBlock(mpb);

            percent += dt * Time.deltaTime;
            yield return null;
        }

        explosion.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }
}
