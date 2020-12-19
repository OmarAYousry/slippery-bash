using UnityEngine;

/// <summary>
/// [AUTHOR] Akbar Suriaganda
/// This script handles the transitions between the events: Idle, Titanic, Storm and Snow
/// </summary>
public class EventStateController: BlendState
{
    public enum EventState { Idle, Titanic, Storm, Snow }

    [SerializeField] private BlendState sky = null;
    [SerializeField] private BlendState wave = null;
    [SerializeField] private BlendState particles = null;
    [SerializeField] private EventStateProfile[] profiles = null;

    [Header("Parameters")]
    /// <summary>
    /// With which event state will the game start?
    /// </summary>
    [Tooltip("With which event state will the game start?")]
    public EventState initialState = EventState.Idle;

    //private EventState currentState;


    //---------------------------------------------------------------------------------------------//
    protected override void Start()
    {
        State = (int)initialState;

        EventStateProfile profile = profiles[State];

        base.Start();
    }

    /// <summary>
    /// Make sure there are as many profiles as states.
    /// </summary>
    private void OnValidate()
    {
        if(profiles.Length != 4)
        {
            EventStateProfile[] old = new EventStateProfile[profiles.Length];
            for(int i = 0; i < old.Length; i++)
            {
                old[i] = profiles[i];
            }
            profiles = new EventStateProfile[4];
            if(old.Length > 0)
            {
                for(int i = 0; i < profiles.Length; i++)
                {
                    profiles[i] = old[Mathf.Min(i, old.Length - 1)];
                }
            }
        }
    }


    //---------------------------------------------------------------------------------------------//
    #region Interface
    public override int StateAmount()
    {
        return 4;
    }
    #endregion


    //---------------------------------------------------------------------------------------------//
    #region Internal
    protected override void ApplyTransition(int toState, float transition)
    {
        ApplyState(toState);
    }
    protected override void ApplyState(int toState)
    {
        //currentState = (EventState)toState;

        EventStateProfile profile = profiles[toState];

        sky.duration = profile.skyProperties.transitionDuration;
        sky.State = profile.skyProperties.state;

        wave.duration = profile.waveProperties.transitionDuration;
        wave.State = profile.waveProperties.state;

        particles.duration = profile.particlesProperties.transitionDuration;
        particles.State = profile.particlesProperties.state;
    }

    protected override void OverrideStateProperties(int index)
    {
        Debug.Log("[EventStateController] Overriding is not necessary since changes is already made on the file.");
    }
    #endregion
}
