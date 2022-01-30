using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayRandomAudioSnippet : MonoBehaviour
{
    public float snippetLength;

    private AudioSource source;
    IEnumerator Start()
    {
        source = GetComponent<AudioSource>();

        if (source != null && source.clip != null)
        {
            source.loop = true;
            source.time = Random.Range(0, source.clip.length);
            source.Play();
            yield return new WaitForSeconds(snippetLength);
            source.Stop();

        }

        Destroy(gameObject);

    }
}
