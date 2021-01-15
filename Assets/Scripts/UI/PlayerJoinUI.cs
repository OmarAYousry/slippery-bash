using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// [AUTHOR] Akbar Suriaganda
/// [DATE] 2021.01.15
/// 
/// The player join ui changes when the payer joins.
/// </summary>
public class PlayerJoinUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text = null;
    [SerializeField] private Image background = null;

    /// <summary>
    /// The text that is shown when the player has not yet joined.
    /// </summary>
    [Tooltip("The text that is shown when the player has not yet joined.")]
    [TextArea]
    public string idleText = "Press any button to join!";
    /// <summary>
    /// The color of the background when the player has not yet joined.
    /// </summary>
    [Tooltip("The color of the background when the player has not yet joined.")]
    public Color idleBackgroundColor = Color.red;
    /// <summary>
    /// The color of the text when the player has not yet joined.
    /// </summary>
    [Tooltip(" The color of the text when the player has not yet joined.")]
    public Color idleTextColor = Color.white;
    /// <summary>
    /// The text that is shown when the player has joined.
    /// </summary>
    [Tooltip("The text that is shown when the player has joined.")]
    [TextArea]
    public string joinedText = "<b>READY!";
    /// <summary>
    /// The color of the background when the player has joined.
    /// </summary>
    [Tooltip("The color of the background when the player has joined.")]
    public Color joinedBackgroundColor = Color.green;
    /// <summary>
    /// The color of the text when the player has joined.
    /// </summary>
    [Tooltip(" The color of the text when the player has joined.")]
    public Color joinedTextColor = Color.white;


    //---------------------------------------------------------------------------------------------//
    /// <summary>
    /// Change the ui to the repective state.
    /// </summary>
    /// <param name="join">true if the player has joined</param>
    public void ToggleJoin(bool join)
    {
        if(join)
        {
            text.text = joinedText;
            text.color = joinedTextColor;
            background.color = joinedBackgroundColor;
        }
        else
        {
            text.text = idleText;
            text.color = idleTextColor;
            background.color = idleBackgroundColor;
        }
    }
}
