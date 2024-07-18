using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public enum AudioGuideState { Started, Finished }



public class AudioGuideButton : MonoBehaviour
{
    private Piece _piece;
    private AudioSource _audioGuideSource;
    private TXRButton _txrButtonTouch;
    private bool _isPlaying = false;
    private bool _isSkipped = false;

    public UnityEvent guideSkipped;

    private ArrowPointer _directionArrow;
    private TaskCompletionSource<bool> _guideSkipped;

    private void Start()
    {
        _audioGuideSource = GetComponent<AudioSource>();
        _txrButtonTouch = GetComponent<TXRButton>();
        _piece = GetComponentInParent<Piece>();
        _audioGuideSource.clip = _piece.audioGuideClip;

        _directionArrow = SceneReferencer.Instance.arrowPointer;
        _txrButtonTouch.Pressed.AddListener(Play);

    }

    [Button("Play")]
    public void Play()
    {
        // If the audio guide is not playing, play it
        if (!_isPlaying)
        {
            _audioGuideSource.Play();
            TXRDataManager.Instance.ReportAudioGuideTiming(_piece.name, AudioGuideState.Started.ToString());

            _isPlaying = true;

            _directionArrow.Hide();
            _piece.arrow.Hide();
        }
    }

    // Skip the audio guide, for debug only!!!
    [Button("Skip")]
    private void Skip()
    {
        _isSkipped = true;
        _audioGuideSource.Stop();
        guideSkipped.Invoke();
        _guideSkipped.SetResult(true);

    }


    public async UniTask WaitForAudioGuideToFinish()
    {
        print("Debug WaitForAudioGuideToFinish, " + _piece.name);

        //wait for player to hit play
        await new WaitUntil(() => _audioGuideSource.isPlaying == true);

        print("Debug WaitForAudioGuideToFinish: " + _audioGuideSource.clip.name + " is playing");
        // wait for audio guide to finish

        await new WaitUntil(() => _audioGuideSource.time >= _audioGuideSource.clip.length);
        print("Debug WaitForAudioGuideToFinish: " + _audioGuideSource.clip.name + " finished");

        TXRDataManager.Instance.ReportAudioGuideTiming(_piece.name, AudioGuideState.Finished.ToString());

    }

    public async UniTask waitForPress()
    {
        //wait for player to hit play
        await new WaitUntil(() => _audioGuideSource.isPlaying == true);

    }


}

