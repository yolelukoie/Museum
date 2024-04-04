using UnityEngine;
using DG.Tweening;
using TMPro;

public enum EButtonAnimationState { Show, Hide, Active, Disable, Hover, Press }
public class TXRButtonVisuals : MonoBehaviour
{
    [SerializeField] private Shapes.Rectangle _backface;
    [SerializeField] private Shapes.Rectangle _stroke;
    [SerializeField] private TextMeshPro _text;

    [Header("Active Animation")]
    [SerializeField] private Color _backfaceColorActive;
    [SerializeField] private float _backfaceGradientActive = 0f;
    [SerializeField] private float _strokeThicknessActive = .0005f;
    [SerializeField] private float _backfadeZPositionActive = .0012f;
    [SerializeField] private float _activeDuration;

    [Header("Disabled")]
    [SerializeField] private Color _backfaceColorDisabled;

    [Header("Hide Animation")]
    [SerializeField] private Color _backfaceColorHide;
    [SerializeField] private float _hideDuration;

    [Header("Hover Animation")]
    [SerializeField] private float _hoverDuration;
    [SerializeField] private Color _backfaceColorGradientHover;
    [SerializeField] private float _backfaceGradientHover = .025f;
    [SerializeField] private float _strokeThicknessHover = .001f;
    [SerializeField] private float _backfadeZPositionHover = .0049f;

    [Header("Press Animation")]
    [SerializeField] private float _pressDuration = .025f;
    [SerializeField] private Color _backfaceColorPress;
    [SerializeField] private float _strokeThicknessPress = .001f;
    [SerializeField] private float _backfaceZPositionPress = -.008f;

    Sequence _animationSequence;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Active();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Hide();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Hover();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Press();
        }
    }

    public void Active()
    {
        print("Active");
        _animationSequence.Kill();
        InitSequence(_backfaceColorActive, _backfaceColorActive, _backfaceGradientActive, _backfadeZPositionActive, _strokeThicknessActive, _activeDuration);
        _animationSequence.Restart();
    }

    public void Show()
    {
        Active();
    }

    public void Hide()
    {
        print("HIDE");
        _animationSequence.Kill();
        InitSequence(_backfaceColorHide, _backfaceColorHide, _backfaceGradientActive, _backfadeZPositionActive, 0, _hideDuration);
        _animationSequence.Restart();
    }

    public void Hover()
    {
        print("HOVER");
        _animationSequence.Kill();
        InitSequence(_backfaceColorGradientHover, _backfaceColorActive, _backfaceGradientHover, _backfadeZPositionHover, _strokeThicknessHover, _hoverDuration);
        _animationSequence.Restart();
    }

    public void Press()
    {
        print("Press");
        _animationSequence.Kill();
        InitSequence(_backfaceColorPress, _backfaceColorPress, _backfaceGradientActive, _backfaceZPositionPress, _strokeThicknessPress, _pressDuration);
        _animationSequence.Restart();
    }

    public void Disabled()
    {
        _animationSequence.Kill();
        InitSequence(_backfaceColorDisabled, _backfaceColorDisabled, _backfaceGradientActive, 0, _strokeThicknessActive, _activeDuration);
        _animationSequence.Restart();
    }

    private void InitSequence(Color backfaceFillStart, Color backfaceFillEnd, float backfaceGradientRadius, float backfaceZOffset, float strokeThickness, float duration)
    {
        _animationSequence = DOTween.Sequence();
        _animationSequence.Append(DOVirtual.Color(_backface.FillColorStart, backfaceFillStart, duration, t => { _backface.FillColorStart = t; }));
        _animationSequence.Join(DOVirtual.Color(_backface.FillColorEnd, backfaceFillEnd, duration, t => { _backface.FillColorEnd = t; }));
        _animationSequence.Join(DOVirtual.Float(_backface.FillRadialRadius, backfaceGradientRadius, duration, t => { _backface.FillRadialRadius = t; }));

        Vector3 backfaceLocalPosition = _backface.transform.localPosition;
        backfaceLocalPosition.z = backfaceZOffset;
        _animationSequence.Join(_backface.transform.DOLocalMove(backfaceLocalPosition, duration));

        _animationSequence.Join(DOVirtual.Float(_stroke.Thickness, strokeThickness, duration, t => { _stroke.Thickness = t; }));

        _animationSequence.SetAutoKill(false);
        _animationSequence.Pause();
    }
}
