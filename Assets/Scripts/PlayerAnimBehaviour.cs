using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimBehaviour : MonoBehaviour
{
    public Animator anim;

    public int HitState { get; private set; }

    public void SetHitState(int value)
    {
        HitState = value;
    }

    public void SetAnimSpeed(int speed)
    {
        anim.speed = speed;
    }
}
