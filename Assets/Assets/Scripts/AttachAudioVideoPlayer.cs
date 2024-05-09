using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class AttachAudioVideoPlayer : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    private void Start()
    {
        videoPlayer.SetTargetAudioSource(0, AudioManager.AudioSourceComponent);
    }
}
