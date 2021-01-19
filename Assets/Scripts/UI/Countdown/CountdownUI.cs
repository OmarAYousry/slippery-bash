using System;
using UnityEngine;

/// <summary>
/// [AUTHOR] Akbar Suriaganda
/// [DATE] 2021.01.18
/// 
/// This script plays the countdown animation and calls the function defined in the listener.
/// </summary>
public class CountdownUI: MonoBehaviour
{
    /// <summary>
    /// Reference to the countdown instance.
    /// </summary>
    public static CountdownUI Instance { get; private set; }

    [SerializeField] private Animator anim = null;


    //---------------------------------------------------------------------------------------------//
    private void Awake()
    {
        if(Instance)
        {
            Destroy(Instance.gameObject);
        }

        Instance = this;
    }


    //---------------------------------------------------------------------------------------------//
    /// <summary>
    /// What should the countdown call after it's finished?
    /// </summary>
    public Action OnCountdownFinished;

    /// <summary>
    /// Enable the canvas, play the animation and call the functions in the listener.
    /// </summary>
    public void PlayCountdown()
    {
        gameObject.SetActive(true);
        anim.enabled = true;
        anim.Play("Countdown", 0);
    }

    /// <summary>
    /// To be called by the animation when the end of the countdown is reached.
    /// </summary>
    public void CountdownEnd()
    {
        OnCountdownFinished?.Invoke();
    }

    /// <summary>
    /// To be called by the animation when the end of the end is reached.
    /// Turns off the canvas.
    /// </summary>
    public void AnimationEnd()
    {
        gameObject.SetActive(false);
    }
}
