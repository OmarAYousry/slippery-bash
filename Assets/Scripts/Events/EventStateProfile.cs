using UnityEngine;

[CreateAssetMenu(fileName = "EventState Profile", menuName = "Profiles/Event State")]
public class EventStateProfile : ScriptableObject
{
    [System.Serializable]
    public class BlendStateProperties
    {
        public int state = 0;
        public float transitionDuration = 1;
    }

    public BlendStateProperties skyProperties;
    public BlendStateProperties waveProperties;
    public BlendStateProperties particlesProperties;
    public float overrideTime = -1;
}
