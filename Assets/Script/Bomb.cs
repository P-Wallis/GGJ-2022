using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float fuseLength = 1.5f;
    public float explosionRadius = 5;
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

        if(Player._!=null)
        {
            Vector3 direction = Player._.transform.position - transform.position;
            direction.y = 0;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, direction.normalized, out hit, Mathf.Max(direction.magnitude, explosionRadius)))
            {
                if(hit.rigidbody != null && hit.rigidbody.gameObject.tag == "Player")
                {
                    Player._.Explode(transform.position);
                }
            }
        }

        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }
}
