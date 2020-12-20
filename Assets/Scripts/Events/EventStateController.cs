using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// [AUTHOR] Akbar Suriaganda
/// This controller specifies how long an event last over time and what events to be triggered.
/// See this script as the head object of the events.
/// </summary>
public class EventStateController: MonoBehaviour
{
    public enum EventStatePhase { Standby, Playing, Pause, Resetting }

    [SerializeField] private EventStateSwitcher switcher = null;

    [Header("Main")]
    /// <summary>
    /// The duration between the events where nothing happens.
    /// </summary>
    [Tooltip("The duration between the events where nothing happens.")]
    public float initialIdleDuration = 10;
    /// <summary>
    /// The duration of an event in the beginning and at the end of the logic.
    /// </summary>
    [Tooltip("The duration of an event in the beginning and at the end of the logic.")]
    public Vector2 maxMinEventDuration = new Vector2(30, 10);
    /// <summary>
    /// The lerp curve of the event duration over time.
    /// </summary>
    [Tooltip("The lerp curve of the event duration over time.")]
    public AnimationCurve eventDurationCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0, 0, Mathf.PI), new Keyframe(1, 1) });
    /// <summary>
    /// The time in seconds until there are no pauses (or at least no changes in the logic)
    /// </summary>
    [Tooltip("The time in seconds until there are no pauses (or at least no changes in the logic)")]
    public float timeUntilCompleteFalloff = 600;

    [Header("Probabilities")]
    public AnimationCurve idleProbability = new AnimationCurve(new Keyframe[] { new Keyframe(1, 1), new Keyframe(1, 1) });
    public AnimationCurve titanicProbability = new AnimationCurve(new Keyframe[] { new Keyframe(1, 1), new Keyframe(1, 1) });
    public AnimationCurve stormProbability = new AnimationCurve(new Keyframe[] { new Keyframe(1, 1), new Keyframe(1, 1) });
    public AnimationCurve snowProbability = new AnimationCurve(new Keyframe[] { new Keyframe(1, 1), new Keyframe(1, 1) });

    private float timer;
    private float eventTimer;
    private float currentEventDuration;

    private float currentIdleProbability;
    private float currentTitanicProbability;
    private float currentStormProbability;
    private float currentSnowProbability;
    private float currentRandom;


    //---------------------------------------------------------------------------------------------//
    #region Interface
    /// <summary>
    /// Start the events logic.
    /// </summary>
    public void Play()
    {
        timer = 0;
        PlayEvent(EventStateSwitcher.EventState.Idle);
    }

    /// <summary>
    /// Stop the logic and go back to idle.
    /// </summary>
    public void Stop()
    {
        PlayEvent(EventStateSwitcher.EventState.Idle);
        Phase = EventStatePhase.Resetting;
    }

    /// <summary>
    /// This event is called after stopping the logic and the event states have been fully switched to idle.
    /// Use this in case you need to wait for reset before restarting.
    /// Alternatively, use Phase to check if this script is on standby.
    /// </summary>
    public UnityAction OnReset;

    /// <summary>
    /// Get the current phase of the event state:
    /// - Standby: safe for a new start
    /// - Playing: the logic is running and events are being played
    /// - Resetting: Stop() has been called and all events are resetting
    /// </summary>
    public EventStatePhase Phase { get; private set; }
    #endregion


    //---------------------------------------------------------------------------------------------//
    #region Internal
    private void Update()
    {
        switch(Phase)
        {
            case EventStatePhase.Standby:
                break;
            case EventStatePhase.Playing:
                Playing();
                break;
            case EventStatePhase.Pause:
                Pause();
                break;
            case EventStatePhase.Resetting:
                Resetting();
                break;
        }
    }

    private void PlayEvent(EventStateSwitcher.EventState eventState)
    {
        eventTimer = 0;
        currentEventDuration = Mathf.Lerp(
            maxMinEventDuration.x,
            maxMinEventDuration.y,
            eventDurationCurve.Evaluate(timer / timeUntilCompleteFalloff));
        switcher.State = (int)eventState;
        switcher.EventDuration = currentEventDuration;

        Phase = EventStatePhase.Playing;
    }

    private void PauseEvent()
    {
        eventTimer = 0;
        currentEventDuration = Mathf.Lerp(initialIdleDuration, 0, eventDurationCurve.Evaluate(timer / timeUntilCompleteFalloff));
        switcher.State = (int)EventStateSwitcher.EventState.Idle;

        Phase = EventStatePhase.Pause;
    }


    private void Playing()
    {
        timer += Time.deltaTime;

        // check first if the event is still in transition
        if(switcher.IsAnimating)
            return;

        // progress the event logic
        eventTimer += Time.deltaTime;

        // pause at the end of an event
        if(eventTimer >= currentEventDuration)
        {
            PauseEvent();
        }
    }

    private void Pause()
    {
        timer += Time.deltaTime;

        // check first if the event is still in transition
        if(switcher.IsAnimating)
            return;

        // progress the pause logic
        eventTimer += Time.deltaTime;

        // search for next event at the end of the pause
        if(eventTimer >= currentEventDuration)
        {
            eventTimer = timer / timeUntilCompleteFalloff;

            // the probablities might not add up to one
            currentIdleProbability = idleProbability.Evaluate(eventTimer);
            currentTitanicProbability = titanicProbability.Evaluate(eventTimer);
            currentStormProbability = stormProbability.Evaluate(eventTimer);
            currentSnowProbability = snowProbability.Evaluate(eventTimer);

            currentRandom = Random.Range(0, currentIdleProbability
                + currentTitanicProbability
                + currentStormProbability
                + currentSnowProbability);

            // switch to the chosen event
            if(currentRandom > currentIdleProbability)
            {
                currentRandom -= currentIdleProbability;
                if(currentRandom > currentTitanicProbability)
                {
                    currentRandom -= currentTitanicProbability;
                    if(currentRandom > currentStormProbability)
                    {
                        PlayEvent(EventStateSwitcher.EventState.Snow);
                    }
                    else
                    {
                        PlayEvent(EventStateSwitcher.EventState.Storm);
                    }
                }
                else
                {
                    PlayEvent(EventStateSwitcher.EventState.Titanic);
                }
            }
            else
            {
                PlayEvent(EventStateSwitcher.EventState.Idle);
            }
        }
    }

    private void Resetting()
    {
        // wait first until the events are resetted
        if(switcher.IsAnimating)
            return;

        // safely go to standby and call the listeners
        Phase = EventStatePhase.Standby;
        OnReset?.Invoke();
    }
    #endregion
}
