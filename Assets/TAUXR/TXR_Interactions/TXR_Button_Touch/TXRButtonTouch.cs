using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

/*TODO
 * - Pass toucher transform in a more elegant way
 * - refactor input processing to another script and get rid of the hovera and press colliders
 */

public class TXRButtonTouch : MonoBehaviour
{
    public ButtonState State => state;
    public bool ShouldPlaySounds = true;

    [SerializeField] private ButtonState state = ButtonState.Interactable;
    private ButtonState lastState;
    [SerializeField] private Transform buttonSurface;

    public Transform activeToucher;
    private List<Transform> touchers = new List<Transform>();

    private float HOVER_DISTANCE_MIN = .00069f;
    private float HOVER_DISTANCE_MAX = .0028f;

    private float distanceToucherFromButtonClamped;

    public Transform ActiveToucher => activeToucher;
    public float DistanceToucherFromButton => distanceToucherFromButtonClamped;

    [SerializeField] private ButtonColliderResponse ResponseHoverEnter;
    public UnityEvent HoverEnter;

    [SerializeField] private ButtonColliderResponse ResponseHoverExit;
    public UnityEvent HoverExit;

    [SerializeField] private ButtonColliderResponse ResponsePress;
    public UnityEvent Pressed;

    [SerializeField] private ButtonColliderResponse ResponseRelease;
    public UnityEvent Released;


    [SerializeField] private AudioSource soundDisabled;
    [SerializeField] private AudioSource soundActive;
    [SerializeField] private AudioSource soundHoverEnter;
    [SerializeField] private AudioSource soundHoverExit;
    [SerializeField] private AudioSource soundPress;
    [SerializeField] private AudioSource soundRelease;
    [SerializeField] private Animator animator;

    private bool isPressed = false;
    private bool isHovered = false;

    public Action<Transform> PressTransform;
    private TaskCompletionSource<bool> _buttonPressedTcs;

    void Start()
    {
        lastState = state;
        SetState(state);
    }

    void Update()
    {
        if (lastState != state)
        {
            SetState(state);
            lastState = state;
        }

        if (state != ButtonState.Interactable) return;


        if (isHovered)
        {
            distanceToucherFromButtonClamped = GetToucherToButtonDistance(activeToucher.position, buttonSurface.position);
        }
    }

    public void SetState(ButtonState state)
    {
        switch (state)
        {
            case ButtonState.Hidden:
                break;
            case ButtonState.Disabled:
                animator.SetBool("IsInteractable", false);
                break;
            case ButtonState.Interactable:
                animator.SetBool("IsInteractable", true);
                break;
        }

        this.state = state;
    }

    // used for external scripts that want to manipulate buttons regardless of touchers.
    public void TriggerButtonEvent(ButtonEvent buttonEvent, ButtonColliderResponse response)
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

    // TODO: Move to interactor
    private float GetToucherToButtonDistance(Vector3 toucherPosition, Vector3 buttonSurfacePosition)
    {
        float distanceToucherFromButtom = (toucherPosition - buttonSurfacePosition).sqrMagnitude;
        float distnaceClamped = 1 - ((distanceToucherFromButtom - HOVER_DISTANCE_MIN) / (HOVER_DISTANCE_MAX - HOVER_DISTANCE_MIN));
        return Mathf.Clamp01(distnaceClamped);
    }
    private void PlaySound(AudioSource sound)
    {
        if (sound == null || !ShouldPlaySounds) return;
        sound.Stop();
        sound.Play();
    }

    // Called from Hover Collider on its public UnityEvent
    public void OnHoverEnter(Transform toucher)
    {
        if (state != ButtonState.Interactable) return;

        var ShouldContinueAfterToucherEnter = HoverEnterToucherProcess(toucher);
        if (!ShouldContinueAfterToucherEnter) return;

        DelegateInteralExtenralResponses(ResponseHoverEnter, OnHoverEnterInternal, HoverEnter);
    }

    private void DelegateInteralExtenralResponses(ButtonColliderResponse response, Action internalAction, UnityEvent externalEvent)
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

    private bool HoverEnterToucherProcess(Transform toucher)
    {
        if (touchers.Count == 0)
        {
            touchers.Add(toucher);
            activeToucher = toucher;
            return true;
        }
        else
        {
            touchers.Add(toucher);
            return false;
        }
    }

    private void OnHoverEnterInternal()
    {
        isHovered = true;
        PlaySound(soundHoverEnter);
        animator.SetBool("IsHover", true);
    }

    // called from button collider

    public void OnHoverExit(Transform toucher)
    {
        if (state != ButtonState.Interactable) return;

        // Catching extreme cases where toucher exit the hover collider without activating the press collider
        if (isPressed)
        {
            OnReleased(toucher);
        }

        var ShouldContinueAfterToucherExit = HoverExitToucherProcessing(toucher);
        if (!ShouldContinueAfterToucherExit) return;

        DelegateInteralExtenralResponses(ResponseHoverExit, OnHoverExitInternal, HoverExit);
    }

    private bool HoverExitToucherProcessing(Transform toucher)
    {
        touchers.Remove(toucher);
        if (toucher != activeToucher) return false;
        else
        {
            if (touchers.Count > 0)
            {
                activeToucher = touchers.Last();
                return false;
            }
            else
            {
                activeToucher = null;
            }
        }
        return true;
    }

    private void OnHoverExitInternal()
    {
        isHovered = false;
        activeToucher = null;
        PlaySound(soundHoverExit);
        animator.SetBool("IsHover", false);
    }

    // called from the UnityEvent on the press collider
    public void OnPressed(Transform toucher)
    {
        if (state != ButtonState.Interactable) return;

        if (toucher != activeToucher) return;

        PressTransform?.Invoke(toucher);
        DelegateInteralExtenralResponses(ResponsePress, OnPressedInternal, Pressed);
    }

    private void OnPressedInternal()
    {
        isPressed = true;
        
        // Complete the task after button pressed
        if (_buttonPressedTcs != null && !_buttonPressedTcs.Task.IsCompleted)
        {
            _buttonPressedTcs.SetResult(true);
        }

        PlaySound(soundPress);
        animator.SetBool("IsPressed", true);
    }

    // called from the UnityEvent on the press collider
    public void OnReleased(Transform toucher)
    {
        if (state != ButtonState.Interactable) return;

        if (toucher != activeToucher) return;

        DelegateInteralExtenralResponses(ResponseRelease, OnReleasedInternal, Released);
    }

    private void OnReleasedInternal()
    {
        isPressed = false;
        PlaySound(soundRelease);
        animator.SetBool("IsPressed", false);
    }

    // added as a quick fix for bug where title on sun nav wouldn't clear active toucher after touch (probably because it moves immediately to keyboard position.
    public void ClearActiveToucher()
    {
        OnHoverExitInternal();
        touchers.Clear();
    }

    private void OnDisable()
    {
        ClearActiveToucher();
    }


    public Task WaitForButtonPress()
    {
        _buttonPressedTcs = new TaskCompletionSource<bool>();
        return _buttonPressedTcs.Task;
    }

}

public enum ButtonColliderResponse { Both, Internal, External, None }
public enum ButtonEvent { HoverEnter, Pressed, Released, HoverExit }
public enum ButtonState { Hidden, Disabled, Interactable }