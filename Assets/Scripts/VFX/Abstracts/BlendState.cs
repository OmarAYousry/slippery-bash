using UnityEngine;

/// <summary>
/// [AUTHOR] Akbar Suriaganda
/// This class handles the transition between states.
/// </summary>
public abstract class BlendState: MonoBehaviour
{
    [Header("Transition")]
    /// <summary>
    /// Give the index of the state to start with.
    /// </summary>
    [Tooltip("Give the index of the state to start with.")]
    public int startState = 0;
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

    private int toState;
    private float transition = -1;


    //---------------------------------------------------------------------------------------------//
    private void Start()
    {
        toState = startState;
        ApplyState(toState);
    }


    //---------------------------------------------------------------------------------------------//
    #region Interface
    /// <summary>
    /// The blend factor between dark and bright; 1: bright.
    /// Changing this property will trigger a transition to the requested intensity.
    /// </summary>
    public int State
    {
        get
        {
            return toState;
        }
        set
        {
            int amount = StateAmount();
            int targetIndex = value;
            if(targetIndex >= amount)
            {
                targetIndex = amount - 1;
                Debug.LogWarning(string.Concat(
                    "[",
                    GetType(),
                    "] This object does not handle ",
                    value,
                    " states. The target index is set to ",
                   targetIndex,
                    " instead if necessary."
                    ));
            }
            if(targetIndex != toState)
            {
                toState = targetIndex;
                transition = 0;
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
            return transition >= 0;
        }
    }

    /// <summary>
    /// How many states does this script handle?
    /// </summary>
    public abstract int StateAmount();
    #endregion


    //---------------------------------------------------------------------------------------------//
    #region Internal
    /// <summary>
    /// Animate the transition if requested.
    /// </summary>
    private void Update()
    {
        if(IsAnimating)
        {
            transition += Time.deltaTime / duration;
            float progress = curve.Evaluate(transition);
            ApplyTransition(toState, progress);

            if(transition >= 1)
            {
                transition = -1;
            }
        }
    }

    protected abstract void ApplyTransition(int toState, float transition);

    protected abstract void ApplyState(int toState);
    #endregion


    //---------------------------------------------------------------------------------------------//
    #region Debugging
    /// <summary>
    /// Show debugging options.
    /// </summary>
    public bool debugging { get; set; }
    /// <summary>
    /// This attribute is used for on editor debugging and only applies if debugging is set to true.
    /// Use the State property instead for proper blending.
    /// </summary>
    public int forceState { get; set; }

    /// <summary>
    /// Save the properties of the states and overwrite them on the active assets.
    /// This is only applied during debugging and only to be used from the inspector.
    /// !WARNING IT WILL DISCARD PREVIOUS CHANGES ON THE ASSET!
    /// </summary>
    /// <param name="state">index of the state in the array</param>
    public void OverrideButton(int state)
    {
        if(debugging)
        {
            OverrideStateProperties(state);
        }
    }

    protected abstract void OverrideStateProperties(int index);
    #endregion
}
