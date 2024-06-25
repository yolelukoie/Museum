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
            _isPlaying = true;
            _directionArrow.Hide();
            TXRDataManager.Instance.ReportAudioGuideTiming(_piece.name, AudioGuideState.Started.ToString());
            _piece.arrow.gameObject.SetActive(false);
        }
    }

    // Skip the audio guide, for debug only!!!
    //[Button("Skip")]
    private void Skip()
    {
        _isSkipped = true;
        _audioGuideSource.Stop();
        guideSkipped.Invoke();
        _guideSkipped.SetResult(true);

    }


    public async UniTask WaitForAudioGuideToFinish()
    {
        //wait for player to hit play
        await new WaitUntil(() => _isPlaying == true);

        // wait for audio guide to finish
        await new WaitUntil(() => ((_audioGuideSource.isPlaying == false)));

        TXRDataManager.Instance.ReportAudioGuideTiming(_piece.name, AudioGuideState.Finished.ToString());

    }

}

