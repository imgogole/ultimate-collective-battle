using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityHitPoint : MonoBehaviour
{
    public List<AudioClip> PunchSounds;
    public AudioSource audioSource;
    public ParticleSystem pSystem;

    private void Start()
    {
        pSystem.Clear();
        audioSource.Stop();
    }

    public void Hit()
    {
        pSystem.Clear();
        pSystem.Stop();
        audioSource.Stop();

        AudioClip clip = PunchSounds[Random.Range(0, 4)];
        audioSource.PlayOneShot(clip);
        pSystem.Play();
    }
}
