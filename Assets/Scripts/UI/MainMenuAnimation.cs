﻿using UnityEngine;

/// <summary>
/// [AUTHOR] Akbar Suriaganda
/// [DATE] 2021.01.15
/// 
/// The menu moves out of the screen when starting the game and moves in again after finishing.
/// </summary>
public class MainMenuAnimation : MonoBehaviour
{
    [SerializeField] private RectTransform mainUI = null;
    [SerializeField] private RectTransform startUI = null;
    [SerializeField] private RectTransform playerUI = null;

    /// <summary>
    /// The duration of the full animation in seconds.
    /// </summary>
    [Tooltip("The duration of the full animation in seconds.")]
    public float duration = 1;
    /// <summary>
    /// Animation progress behavior over time.
    /// </summary>
    [Tooltip("Animation progress behavior over time.")]
    public AnimationCurve curve = new AnimationCurve(new Keyframe[2] { new Keyframe(0, 0), new Keyframe(1, 1) });


    private float animProgress = 0;    // 0 = menu enabled
    private float curveProgress = 0;
    private bool moveIn = true;


    //---------------------------------------------------------------------------------------------//
    private void Update()
    {
        if(animProgress > 0 && moveIn
            || animProgress < 1 && !moveIn)
        {
            animProgress += Time.deltaTime / duration * (moveIn ? -1 : 1);
            animProgress = Mathf.Clamp01(animProgress);

            curveProgress = curve.Evaluate(animProgress);

            mainUI.anchoredPosition = Vector2.left * Screen.width * curveProgress;
            startUI.anchoredPosition = Vector2.right * Screen.width * curveProgress;
            playerUI.anchoredPosition = Vector2.up * Screen.height * curveProgress;
        }
        else
        {
            enabled = false;
        }
    }


    //---------------------------------------------------------------------------------------------//
    /// <summary>
    /// Trigger the animation.
    /// Enable the canvas if necessary.
    /// </summary>
    /// <param name="moveIn">Is the menu appearing or leaving? true: appearing</param>
    public void Animate(bool moveIn)
    {
        if(!gameObject.activeInHierarchy)
            gameObject.SetActive(true);

        this.moveIn = moveIn;
        enabled = true;
    }
}
