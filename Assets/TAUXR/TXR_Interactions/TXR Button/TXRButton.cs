using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class TXRButton : MonoBehaviour
{
    public ButtonState State => state;
    public bool ShouldPlaySounds = true;

    [SerializeField] private ButtonState state = ButtonState.Interactable;

    public UnityEvent Pressed;
    public UnityEvent Released;
    public UnityEvent HoverEnter;
    public UnityEvent HoverExit;

    [SerializeField] protected ButtonColliderResponse ResponseHoverEnter;
    [SerializeField] protected ButtonColliderResponse ResponseHoverExit;
    [SerializeField] protected ButtonColliderResponse ResponsePress;
    [SerializeField] protected ButtonColliderResponse ResponseRelease;

    protected TXRButtonInput input;
    protected TXRButtonVisuals visuals;

    public Action<Transform> PressTransform;
    public TXRButtonReferences References;

    public Transform ActiveToucher => input.MainToucher;


    // Noa Added:
    private TaskCompletionSource<bool> _buttonPressedTcs;

    [SerializeField]
    private TMPro.TextMeshPro textComponent;

    public void SetText(String text)
    {
        textComponent.text = text;

        print("Set button text to: " + text + " textComponent: " + textComponent);
    }

    public void TextInit()
    {
        textComponent = ComponentChecker.GetComponentInAllChildren<TMPro.TextMeshPro>(transform);
    }

    public Task WaitForButtonPress()
    {
        _buttonPressedTcs = new TaskCompletionSource<bool>();
        return _buttonPressedTcs.Task;
    }

    // end of Noa Added

    protected virtual void Start()
    {
        Init();
        textComponent = ComponentChecker.GetComponentInAllChildren<TMPro.TextMeshPro>(transform);
    }

    protected virtual void Init()
    {
        visuals = References.ButtonVisuals;
        visuals.Init(References);

        input = References.ButtonInput;
        input.Init(References);
        SetState(state);
    }

    public void SetColor(EButtonAnimationState state, Color color, float duration = 0.25f)
    {
        visuals.SetColor(state, color, duration);
    }

    public void SetState(ButtonState state)
    {
        switch (state)
        {
            case ButtonState.Hidden:
                visuals.SetState(EButtonAnimationState.Hide);
                break;
            case ButtonState.Disabled:
                visuals.SetState(EButtonAnimationState.Disable);
                break;
            case ButtonState.Interactable:
                visuals.SetState(EButtonAnimationState.Active);
                break;
            case ButtonState.Frozen:
                break;
        }

        this.state = state;
    }

    // used for external scripts that want to manipulate buttons regardless of touchers.
    public virtual void TriggerButtonEvent(ButtonEvent buttonEvent, ButtonColliderResponse response)
    {
        switch (buttonEvent)
        {
            case ButtonEvent.HoverEnter:
                DelegateInteralExtenralResponses(response, OnHoverEnterInternal, HoverEnter);
                break;

            case ButtonEvent.HoverExit:
                DelegateInteralExtenralResponses(response, OnHoverExitInternal, HoverExit);
                break;

            case ButtonEvent.Pressed:
                DelegateInteralExtenralResponses(response, OnPressedInternal, Pressed);
                break;

            case ButtonEvent.Released:
                DelegateInteralExtenralResponses(response, OnReleasedInternal, Released);
                break;
        }
    }

    // called from input manager
    public virtual void TriggerButtonEventFromInput(ButtonEvent buttonEvent)
    {
        if (State != ButtonState.Interactable) return;

        switch (buttonEvent)
        {
            case ButtonEvent.HoverEnter:
                DelegateInteralExtenralResponses(ResponseHoverEnter, OnHoverEnterInternal, HoverEnter);
                break;

            case ButtonEvent.HoverExit:
                DelegateInteralExtenralResponses(ResponseHoverExit, OnHoverExitInternal, HoverExit);
                break;

            case ButtonEvent.Pressed:
                DelegateInteralExtenralResponses(ResponsePress, OnPressedInternal, Pressed);
                break;

            case ButtonEvent.Released:
                DelegateInteralExtenralResponses(ResponseRelease, OnReleasedInternal, Released);
                break;
        }
    }



    protected void PlaySound(AudioSource sound)
    {
        if (sound == null || !ShouldPlaySounds) return;
        sound.Stop();
        sound.Play();
    }

    protected void DelegateInteralExtenralResponses(ButtonColliderResponse response, Action internalAction, UnityEvent externalEvent)
    {
        switch (response)
        {
            case ButtonColliderResponse.None:
                break;
            case ButtonColliderResponse.Both:
                externalEvent.Invoke();
                internalAction();
                break;
            case ButtonColliderResponse.Internal:
                internalAction();
                break;
            case ButtonColliderResponse.External:
                externalEvent.Invoke();
                break;
        }
    }
    protected virtual void OnHoverEnterInternal()
    {
        visuals.SetState(EButtonAnimationState.Hover);
    }
    protected virtual void OnHoverExitInternal()
    {
        visuals.SetState(EButtonAnimationState.Active);
    }
    protected virtual void OnPressedInternal()
    {
        PlaySound(References.SoundPress);
        visuals.SetState(EButtonAnimationState.Press);
        // Complete the task after button pressed
        if (_buttonPressedTcs != null && !_buttonPressedTcs.Task.IsCompleted)
        {
            _buttonPressedTcs.SetResult(true);
        }
    }
    protected virtual void OnReleasedInternal()
    {
        PlaySound(References.SoundRelease);
        visuals.SetState(EButtonAnimationState.Active);
    }
}



public enum ButtonColliderResponse { Both, Internal, External, None }
public enum ButtonEvent { HoverEnter, Pressed, Released, HoverExit }
public enum ButtonState { Hidden, Disabled, Interactable, Frozen }