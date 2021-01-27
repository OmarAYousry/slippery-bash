using System.Collections;
using UnityEngine;
using static EventStateSwitcher;

public enum SoundEffectType
{
    PLAYER_JUMP, PLAYER_PUNCH, PLAYER_HIT, LIGHTNING_STRIKE, TITANIC_CRASH
}

public class AudioController : MonoBehaviour
{
    private static AudioController instance = null;

    [SerializeField]
    private AudioSource defaultBGMAudioSource = null;

    [SerializeField]
    private AudioSource EventsAudioSource = null;

    [SerializeField]
    private AudioSource defaultSfxSource = null;

    #region background-music-clips

    [SerializeField]
    private AudioClip defaultBackgroundMusic;

    [SerializeField]
    private AudioClip titanicEventMusic;

    [SerializeField]
    private AudioClip stormEventMusic;

    [SerializeField]
    private AudioClip snowEventMusic;

    [SerializeField]
    private AudioClip lightningStrikeSfx;    
    
    [SerializeField]
    private AudioClip titanicCrashSfx;

    #endregion

    #region player-sound-effect-clips

    [SerializeField]
    private AudioClip playerJumpClip;

    [SerializeField]
    private AudioClip playerPunchClip;

    [SerializeField]
    private AudioClip playerGetHitClip;

    #endregion

    #region mono-behaviours

    void Awake()
    {
        instance = this;    
    }

    void Start()
    {
        if (!instance.defaultBGMAudioSource.isPlaying)
        {
            instance.defaultBGMAudioSource.Play();
        }
    }

    #endregion

    #region background-music-functions

    public static void PlayEventMusic(EventState eventType)
    {
        PlayEventMusic(eventType, instance.EventsAudioSource);
    }

    public static void PlayEventMusic(EventState eventType, AudioSource eventAudioSrc)
    {
        eventAudioSrc.Stop();
        
        switch (eventType)
        {
            case EventState.Idle:
                eventAudioSrc.Stop();
                break;
            case EventState.Storm:
                eventAudioSrc.PlayOneShot(instance.stormEventMusic);
                break;
            case EventState.Snow:
                eventAudioSrc.PlayOneShot(instance.snowEventMusic);
                break;
            case EventState.Titanic:
                eventAudioSrc.PlayOneShot(instance.titanicEventMusic);
                break;
            default:
                throw new System.Exception($"UNHANDLED BACKGROUND MUSIC TYPE {eventType}");
        }
    }

    #endregion

    #region sound-effect-functions

    public static void PlaySoundEffect(SoundEffectType soundEffectType)
    {
        PlaySoundEffect(soundEffectType, instance.defaultSfxSource);
    }

    public static void PlaySoundEffect(SoundEffectType soundEffectType, AudioSource audioSrc, bool handleAudioSourceDestruction = false)
    {
        audioSrc.Stop();

        switch (soundEffectType)
        {
            case SoundEffectType.PLAYER_JUMP:
                audioSrc.PlayOneShot(instance.playerJumpClip);
                break;
            case SoundEffectType.PLAYER_PUNCH:
                audioSrc.PlayOneShot(instance.playerPunchClip);
                break;
            case SoundEffectType.PLAYER_HIT:
                audioSrc.PlayOneShot(instance.playerGetHitClip);
                break;
            case SoundEffectType.LIGHTNING_STRIKE:
                audioSrc.PlayOneShot(instance.lightningStrikeSfx);
                break;
            case SoundEffectType.TITANIC_CRASH:
                audioSrc.PlayOneShot(instance.titanicCrashSfx);
                break;
            default:
                break;
        }

        if (handleAudioSourceDestruction)
        {
            instance.StartCoroutine(instance.destroyAudioSourceWhenDone(audioSrc));
        }
    }

    #endregion

    private IEnumerator destroyAudioSourceWhenDone(AudioSource sourceToBeDestroyed)
    {
        // disable looping to ensure isPlaying turns false (eventually)
        sourceToBeDestroyed.loop = false;

        // remove audio source from parent to avoid parents' destruction affecting it
        sourceToBeDestroyed.transform.parent = transform;

        while (sourceToBeDestroyed && sourceToBeDestroyed.isPlaying)
        {
            // wait for 1 second at a time while
            // the audio source is still playing something
            yield return new WaitForSecondsRealtime(1.0f);
        }

        // destroy the audio source after it's done playing
        if(sourceToBeDestroyed)
            Destroy(sourceToBeDestroyed.gameObject);
    }

}
