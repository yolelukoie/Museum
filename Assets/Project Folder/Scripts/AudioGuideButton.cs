using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;





public class AudioGuideButton : MonoBehaviour
{
    private Piece _piece;
    private AudioSource _audioGuideSource;
    private TXRButtonTouch _txrButtonTouch;
    private bool _isPlaying = false;

    public UnityEvent guideSkipped;

    private TaskCompletionSource<bool> _guideSkipped;

    private void Start()
    {
        _audioGuideSource = GetComponent<AudioSource>();
        _txrButtonTouch = GetComponent<TXRButtonTouch>();
        _piece = GetComponentInParent<Piece>();
        _audioGuideSource.clip = _piece.audioGuideClip;

        _txrButtonTouch.Pressed.AddListener(PlaySkip);
        guideSkipped.AddListener(OnGuideSkipped);
    }

    public void PlaySkip()
    {
        // If the audio guide is not playing, play it
        if (!_isPlaying)
        {
            _audioGuideSource.Play();
            _isPlaying = true;
            return;
        }

        // If the audio guide is playing, skip 
        if (_isPlaying)
        {
            _audioGuideSource.Stop();
            _isPlaying = false;
            guideSkipped.Invoke();

            return;
        }

    }

    private void OnGuideSkipped()
    {
        // Complete the task after guide was skipped
        if (_guideSkipped != null && !_guideSkipped.Task.IsCompleted)
        {
            _guideSkipped.SetResult(true);
        }
    }

    public async UniTask WaitForAudioGuideToFinish()
    {
        //wait for player to hit play
        await new WaitUntil(() => _isPlaying == true);
        // wait for player to hit skip / audioguidefinished
        await new WaitUntil(() => _audioGuideSource.isPlaying == false);





    }



    private Task WaitForPlayerToSkip()
    {
        _guideSkipped = new TaskCompletionSource<bool>();
        return _guideSkipped.Task;

    }


}
