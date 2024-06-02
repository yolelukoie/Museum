using UnityEngine;

[CreateAssetMenu(fileName = "ButtonVisualsConfigurations", menuName = "ScriptableObjects/ButtonVisualsConfigurations", order = 1)]
public class ButtonVisualsConfigurations : ScriptableObject
{
    [Header("Active Animation")]
    [Tooltip("Gradient of the backface when active")]
    public float backfaceGradientRadiusActive;
    [Tooltip("Thickness of the stroke when active")]
    public float strokeThicknessActive;
    [Tooltip("Z position of the backfade when active")]
    public float backfaceZPositionActive;
    [Tooltip("Duration of the active state animation")]
    public float activeDuration;

    [Header("Press Animation")]
    [Tooltip("Duration of the press animation")]
    public float pressDuration;
    [Tooltip("Thickness of the stroke on press")]
    public float strokeThicknessPress;
    [Tooltip("Z position of the backfade on press")]
    public float backfadeZPositionPress;

    [Header("Hide Animation")]
    [Tooltip("Color of the backface when hidden")]
    public Color backfaceColorHide;
    [Tooltip("Duration of the hide animation")]
    public float hideDuration;

    [Header("Hover Animation")]
    [Tooltip("Duration of the hover animation")]
    public float hoverDuration;
    [Tooltip("The color of the gradient appears on the button when hovering on it")]
    public Color backfaceColorGradientHover;
    [Tooltip("Gradient radius of the backface on hover")]
    public float backfaceGradientRadiusHover;
    [Tooltip("Thickness of the stroke on hover")]
    public float strokeThicknessHover;
    [Tooltip("Z position of the backfade on hover")]
    public float backfadeZPositionHover;

    [Header("Defaults")]
    public Color activeColor;
    public Color pressColor;
    public Color disableColor;

    /*
     * possibly new parameters
     * color - active, pressed, hidden, disabled
     * hover - gradient color, gradient radius, stroke z, backface z
     * 
     * 
     */
}
