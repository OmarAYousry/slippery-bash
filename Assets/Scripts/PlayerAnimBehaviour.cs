using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimBehaviour: MonoBehaviour
{
    public Animator anim;
    public AudioSource moveAudio;

    private PlayerBehaviour player;

    public int HitState { get; private set; }

    public void SetHitState(int value)
    {
        HitState = value;
    }

    public void SetAnimSpeed(int speed)
    {
        anim.speed = speed;
    }

    public void Step(AnimationEvent animEvent)
    {
        if(!player)
            player = transform.parent.GetComponent<PlayerBehaviour>();

        if(!player.IsOnGround)
            return;

        if(player.isOnIce)
            AudioController.PlaySoundEffect(SoundEffectType.PLAYER_STEP, moveAudio);
        else
            AudioController.PlaySoundEffect(SoundEffectType.PLAYER_SNOWSTEP, moveAudio);
    }

    public void Swim(AnimationEvent animEvent)
    {
        AudioController.PlaySoundEffect(SoundEffectType.PLAYER_SWIM, moveAudio);
    }
}
