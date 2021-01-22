using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using static EventStateSwitcher;

public enum EventBackgroundType
{
    NONE, STORM, SNOW, SHIP
}

public enum PlayerEffectType
{
    JUMP, PUNCH, HIT
}

public class AudioController : MonoBehaviour
{
    private static AudioController instance = null;

    [SerializeField]
    private AudioSource defaultBGMAudioSource = null;

    [SerializeField]
    private AudioSource EventsAudioSource = null;

    #region background-music-clips

    [SerializeField]
    private AudioClip defaultBackgroundMusic;

    [SerializeField]
    private AudioClip titanicEventMusic;

    [SerializeField]
    private AudioClip stormEventMusic;

    [SerializeField]
    private AudioClip snowEventMusic;

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

    //public static void PlaySoundEffect(PlayerEffectType playerSoundEffectType)
    //{
    //    PlayBackgroundMusic(playerSoundEffectType, instance.default);
    //}

    public static void PlaySoundEffect(PlayerEffectType playerSoundEffectType, AudioSource audioSrc)
    {
        audioSrc.Stop();

        switch (playerSoundEffectType)
        {
            case PlayerEffectType.JUMP:
                audioSrc.PlayOneShot(instance.playerJumpClip);
                break;
            case PlayerEffectType.PUNCH:
                audioSrc.PlayOneShot(instance.playerPunchClip);
                break;
            case PlayerEffectType.HIT:
                audioSrc.PlayOneShot(instance.playerGetHitClip);
                break;
            default:
                break;
        }
    }

    #endregion

}
