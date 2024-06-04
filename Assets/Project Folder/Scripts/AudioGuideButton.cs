using Cysharp.Threading.Tasks;
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

    private TaskCompletionSource<bool> _guideSkipped;

    private void Start()
    {
        _audioGuideSource = GetComponent<AudioSource>();
        _txrButtonTouch = GetComponent<TXRButton>();
        _piece = GetComponentInParent<Piece>();
        _audioGuideSource.clip = _piece.audioGuideClip;

        _txrButtonTouch.Pressed.AddListener(Play);

    }

    public void Play()
    {
        // If the audio guide is not playing, play it
        if (!_isPlaying)
        {
            _audioGuideSource.Play();
            _isPlaying = true;
            TXRDataManager.Instance.ReportAudioGuideTiming(_piece.name, AudioGuideState.Started.ToString());
            _piece.arrow.gameObject.SetActive(false);
        }
    }


    public async UniTask WaitForAudioGuideToFinish()
    {
        //wait for player to hit play
        await new WaitUntil(() => _isPlaying == true);

        // wait for audio guide to finish
        await new WaitUntil(() => _audioGuideSource.isPlaying == false);

        TXRDataManager.Instance.ReportAudioGuideTiming(_piece.name, AudioGuideState.Finished.ToString());



    }

}

