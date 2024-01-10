using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPrefab : MonoBehaviour
{
    public AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        CheckIsPlaying();
    }

    public void CheckIsPlaying()
    {
        if(!audioSource.isPlaying)
        {
            SoundManager.instance.Stop(audioSource.clip.name);
        }
    }
}
