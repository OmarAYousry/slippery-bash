using UnityEngine;

/// <summary>
/// [AUTHOR] Akbar Suriaganda
/// This class handles the transition between two states.
/// </summary>
public abstract class BlendBinaryState : MonoBehaviour
{
    [Header("Transition")]
    /// <summary>
    /// At what state should the object start?
    /// </summary>
    [Tooltip("At what state should the object start?")]
    [Range(0,1)] public float startValue = 0;
    /// <summary>
    /// The time in seconds the transition to the requested intensity needs.
    /// </summary>
    [Tooltip("The time in seconds the transition to the requested intensity needs.")]
    public float duration = 5;
    /// <summary>
    /// The curve of the value during the transition over time.
    /// </summary>
    [Tooltip("The curve of the value during the transition over time.")]
    public AnimationCurve curve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(1, 1) });

    private float toValue;
    private float currentValue;
    private int progression;          // -1: go to 0; 0: no animation; 1: go to 1


    //---------------------------------------------------------------------------------------------//
    private void Start()
    {
        Value = overrideValue = toValue = currentValue = startValue;
        progression = 0;

        ApplyInstant(startValue);
    }


    //---------------------------------------------------------------------------------------------//
    #region Interface
    /// <summary>
    /// The blend factor between dark and bright; 1: bright.
    /// Changing this property will trigger a transition to the requested intensity.
    /// </summary>
    public float Value
    {
        get
        {
            return currentValue;
        }
        set
        {
            if(value != toValue)
            {
                toValue = value;
                progression = toValue > currentValue ? 1 : -1;
            }
        }
    }

    /// <summary>
    /// The property telling, if the object is currently animating the transition to a requested intensity.
    /// </summary>
    public bool IsAnimating
    {
        get
        {
            return progression != 0;
        }
    }
    #endregion


    //---------------------------------------------------------------------------------------------//
    #region Internal
    /// <summary>
    /// Animate the transition if requested.
    /// </summary>
    private void Update()
    {
        if(progression != 0)
        {
            if(progression > 0 && currentValue >= toValue
                || progression < 0 && currentValue <= toValue)
            {
                currentValue = toValue;
                progression = 0;
            }
            else
            {
                currentValue += Time.deltaTime / duration * progression;
            }
            float progress = curve.Evaluate(currentValue);
            ApplyProgress(progress);
        }
    }

    protected abstract void ApplyProgress(float progress);

    protected abstract void ApplyInstant(float progress);
    #endregion


    //---------------------------------------------------------------------------------------------//
    #region Debugging
    /// <summary>
    /// Show debugging options.
    /// </summary>
    public bool debugging { get; set; }
    /// <summary>
    /// This attribute is used for on editor debugging and only applies if debugging is set to true.
    /// Use the Value property instead for proper blending.
    /// </summary>
    public float overrideValue { get; set; }

    /// <summary>
    /// Save the properties of the states and overwrite them on the active assets.
    /// This is only applied during debugging and only to be used from the inspector.
    /// !WARNING IT WILL DISCARD PREVIOUS CHANGES ON THE ASSET!
    /// </summary>
    /// <param name="firstState">true if the asset of the first state should be overwritten</param>
    public void OverrideButton(bool firstState)
    {
        if(debugging)
        {
            OverrideStateProperties(firstState);
        }
    }

    protected abstract void OverrideStateProperties(bool firstState);
    #endregion
}
