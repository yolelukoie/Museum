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
    private AudioTimeLeft _audioTimeLeft;

    public UnityEvent guideSkipped;

    private DirectionGuideArrow _directionArrow;
    private TaskCompletionSource<bool> _guideSkipped;

    private void Awake()
    {
        _audioGuideSource = GetComponent<AudioSource>();
        _txrButtonTouch = GetComponent<TXRButton>();
    }

    private void Start()
    {
        _piece = GetComponentInParent<Piece>();
        _audioTimeLeft = GetComponentInChildren<AudioTimeLeft>();
        _audioTimeLeft.gameObject.SetActive(false);
        _audioGuideSource.clip = _piece.audioGuideClip;
        _directionArrow = SceneReferencer.Instance.DirectionArrow;
        _txrButtonTouch.Pressed.AddListener(Play);

        Debug.Log("AudioGuideButton.cs: " + _piece.name + " state after start: \n"
            + "_piece = " + _piece.ToString() + "\n"
            + "_audioGuideSource = " + _audioGuideSource.ToString() + "\n"
            + "_audioTimeLeft = " + _audioTimeLeft.ToString() + "\n"
            + "_isPlaying = " + _isPlaying.ToString() + "\n"
            + "_audioGuideSource.clip = " + _audioGuideSource.clip.ToString() + "\n"
            + "_piece.audioGuideClip = " + _piece.audioGuideClip.ToString() + "\n"
            + "_audioGuideSource.clip.length = " + _audioGuideSource.clip.length.ToString() + "\n"
            + "_piece.audioGuideClip.length = " + _piece.audioGuideClip.length.ToString() + "\n"
            );

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
            _audioTimeLeft.gameObject.SetActive(true);
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
        print("AudioGuideButton.cs: Debug WaitForAudioGuideToFinish, " + _piece.name);

        //wait for player to hit play
        await new WaitUntil(() => _audioGuideSource.isPlaying == true);

        print("AudioGuideButton.cs: Debug WaitForAudioGuideToFinish: " + _audioGuideSource.clip.name + " is playing");
        // wait for audio guide to finish

        await new WaitUntil(() => _audioGuideSource.time >= _audioGuideSource.clip.length);
        print("AudioGuideButton.cs: Debug WaitForAudioGuideToFinish: " + _audioGuideSource.clip.name + " finished");

        _audioTimeLeft.gameObject.SetActive(false);
        print("AudioGuideButton.cs: after _audioTimeLeft.gameObject.SetActive(false)");
        TXRDataManager.Instance.ReportAudioGuideTiming(_piece.name, AudioGuideState.Finished.ToString());

    }

    public async UniTask waitForPress()
    {
        //wait for player to hit play
        await new WaitUntil(() => _audioGuideSource.isPlaying == true);

    }


}

