using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySFXOneshot : MonoBehaviour
{
    private AudioSource source;
    public float pitchMin = 0.9f, pitchMax = 1.1f;
    IEnumerator Start()
    {
        source = GetComponent<AudioSource>();

        if (source != null && source.clip != null)
        {
            source.pitch = Random.Range(pitchMin, pitchMax);
            source.Play();
            yield return new WaitForSeconds(source.clip.length);
        }

        Destroy(gameObject);

    }
}
