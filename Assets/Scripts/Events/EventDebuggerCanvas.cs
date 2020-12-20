using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// [AUTHOR] Akbar Suriaganda
/// This script updates the status seen on the debugging canvas.
/// (Not further documented)
/// </summary>
public class EventDebuggerCanvas : MonoBehaviour
{
    public Button[] events;
    public Text phase;
    public Image timer;
    public Image eventTimer;
    public Image[] probabilities;

    public EventStateController controller;
    public EventStateSwitcher switcher;

    private void Update()
    {
        for(int i = 0; i < events.Length; i++)
        {
            events[i].interactable = switcher.State != i;
            float fill = controller.GameProgress;
            switch(i)
            {
                case 0:
                    fill = controller.idleProbability.Evaluate(fill);
                    break;
                case 1:
                    fill = controller.titanicProbability.Evaluate(fill);
                    break;
                case 2:
                    fill = controller.stormProbability.Evaluate(fill);
                    break;
                case 3:
                    fill = controller.snowProbability.Evaluate(fill);
                    break;
            }
            probabilities[i].fillAmount = fill;

        }

        phase.text = "Phase: " + (switcher.IsAnimating ? "Transitioning..." : controller.Phase.ToString());
        timer.fillAmount = controller.GameProgress;
        eventTimer.fillAmount = controller.EventProgress;
    }
}
