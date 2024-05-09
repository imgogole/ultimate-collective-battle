using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    public static AudioManager Instance => instance;

    public AudioSource _AudioSource;
    //public AudioSource _3DAudioSource;
    public AudioSource _MusicAudioSource;

    public GameObject Audio3D;

    public static AudioSource AudioSourceComponent => instance._AudioSource;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
    }

    public List<AudioClip> Audios = new List<AudioClip>();
    public List<AudioClip> Musics = new List<AudioClip>();

    /// <summary>
    /// Adjust the volume of every sound.
    /// </summary>
    /// <param name="sfxVolume"></param>
    /// <param name="musicVolume"></param>
    public static void AdjustVolume(float sfxVolume, float musicVolume)
    {
        instance._AudioSource.volume = sfxVolume;
        instance._MusicAudioSource.volume = musicVolume;
    }

    /// <summary>
    /// Play a sound.
    /// </summary>
    /// <param name="audio"></param>
    public static void PlaySound(Audio audio)
    {
        AudioClip clip = instance.Audios[(int)audio];
        instance._AudioSource.PlayOneShot(clip);
    }

    /// <summary>
    /// Play a sound in a certain position in the world.
    /// </summary>
    /// <param name="audio"></param>
    public static void PlaySound(Audio audio, Vector3 position, float maxDistance)
    {
        Instance.Create3DSound(audio, position, maxDistance);
    }

    /// <summary>
    /// Play a music
    /// </summary>
    /// <param name="audio"></param>
    public static void PlayMusic(Music audio)
    {
        AudioClip clip = instance.Musics[(int)audio];
        instance._MusicAudioSource.PlayOneShot(clip);
    }

    private void Create3DSound(Audio audio, Vector3 position, float maxDistance)
    {
        GameObject obj3d = Instantiate(Audio3D ,position, Quaternion.identity);

        AudioSource audioSource = obj3d.GetComponent<AudioSource>();

        AudioClip clip = instance.Audios[(int)audio];
        obj3d.transform.position = position;
        audioSource.maxDistance = maxDistance;
        audioSource.PlayOneShot(clip);

        StartCoroutine(Coroutine_DeleteAudio(obj3d, clip.length));
    }

    IEnumerator Coroutine_DeleteAudio(GameObject obj3d, float length)
    {
        yield return new WaitForSeconds(length);
        Destroy(obj3d);
    }
}

public enum Audio
{
    ClickButton,
    ObjectBought,
    Blocked,
    GetKey,
    Countdown,
    Jump,
    OnGround,
    DoorOpened,
    DoorClosed,
    Stairs,
    EnemyDoorOpened,
    AllyDoorOpened,
    Coins,
    Hewew,
    HewewSimple,
    Ray,
    CatchUp,
    OpenSoda,
    Eat,
    DestroyedLeMettayer,
    Stun,
    LouUltimate,
    SamuelScream,
    VirusMaxCount,
    ClashRoyale,
    Brother,
    Wifi,
    Noirrel,
    OneKill,
    DoubleKill,
    TripleKill,
    QuadraKill,
    PentaKill,
    Bait,
    UltimeStole,
    LaughUltimeStole
}

public enum Music
{
    SuddenDeath
}