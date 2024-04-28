using UnityEngine;
using UnityEngine.Events;





public class AudioGuideButton : MonoBehaviour
{
    private Piece _piece;
    private AudioSource _audioGuideSource;
    private TXRButtonTouch _txrButtonTouch;
    private bool _isPlaying = false;

    public UnityEvent guideSkipped;

    private void Start()
    {
        _audioGuideSource = GetComponent<AudioSource>();
        _txrButtonTouch = GetComponent<TXRButtonTouch>();
        _piece = GetComponentInParent<Piece>();
        _audioGuideSource.clip = _piece.audioGuideClip;

        _txrButtonTouch.Pressed.AddListener(PlaySkip);
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



}
