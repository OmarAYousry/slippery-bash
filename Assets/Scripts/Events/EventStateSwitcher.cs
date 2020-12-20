using UnityEngine;

/// <summary>
/// [AUTHOR] Akbar Suriaganda
/// This script handles the transitions between the events and their behaviour: Idle, Titanic, Storm and Snow
/// </summary>
public class EventStateSwitcher: BlendState
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

    [Header("Behaviors")]
    [SerializeField] private EventBehavior titanic = null;
    [SerializeField] private EventBehavior storm = null;


    private bool skyReady;
    private bool waveReady;
    private bool particlesReady;


    //---------------------------------------------------------------------------------------------//
    protected override void Start()
    {
        startState = (int)initialState;

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

    public override bool IsAnimating
    {
        get
        {
            return base.IsAnimating || sky.IsAnimating || wave.IsAnimating || particles.IsAnimating;
        }
    }

    /// <summary>
    /// The event's behavior depends on the duration;
    /// </summary>
    public float EventDuration { get; set; }
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
        skyReady = false;
        sky.OnAnimationEnd += EffectReady;

        wave.duration = profile.waveProperties.transitionDuration;
        wave.State = profile.waveProperties.state;
        waveReady = false;
        wave.OnAnimationEnd += EffectReady;

        particles.duration = profile.particlesProperties.transitionDuration;
        particles.State = profile.particlesProperties.state;
        particlesReady = false;
        particles.OnAnimationEnd += EffectReady;

        if(!sky.IsAnimating && !wave.IsAnimating && !particles.IsAnimating)
        {
            TriggerEventBehaviour();
            OnAnimationEnd?.Invoke(this);
        }
    }

    protected override void DisableState(int previousState) {
        switch((EventState)previousState)
        {
            case EventState.Idle:
                break;
            case EventState.Titanic:
                titanic.ResetBehavior();
                break;
            case EventState.Storm:
                storm.ResetBehavior();
                break;
            case EventState.Snow:
                break;
        }
    }

    protected override void OverrideStateProperties(int index)
    {
        Debug.Log("[EventStateController] Overriding is not necessary since changes is already made on the file.");
    }

    private void EffectReady(BlendState effect)
    {
        if(effect == sky)
        {
            sky.OnAnimationEnd -= EffectReady;
            skyReady = true;
        }
        else if(effect == wave)
        {
            wave.OnAnimationEnd -= EffectReady;
            waveReady = true;
        }
        else if(effect == particles)
        {
            particles.OnAnimationEnd -= EffectReady;
            particlesReady = true;
        }

        if(skyReady && waveReady && particlesReady)
        {
            TriggerEventBehaviour();
            OnAnimationEnd?.Invoke(this);
        }
    }

    public void TriggerEventBehaviour()
    {
        switch((EventState)State)
        {
            case EventState.Idle:
                break;
            case EventState.Titanic:
                titanic.StartBehavior(EventDuration);
                break;
            case EventState.Storm:
                storm.StartBehavior(EventDuration);
                break;
            case EventState.Snow:
                break;
        }
    }
    #endregion
}
