using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;

public class ArtPieceProximityDetector : MonoBehaviour
{

    Piece _piece;
    bool _isActive = false;
    bool _isPlayerInside = false;
    public TextMeshProUGUI _debugText;
    private void Awake()
    {
        _piece = GetComponentInParent<Piece>();
        _debugText = SceneReferencer.Instance.debugText;
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (_isActive)
    //    {
    //        if (other.CompareTag("PlayerHead"))
    //        {

    //        }

    //    }
    //}

    private void OnTriggerStay(Collider other)
    {
        if (_isActive)
        {
            if (_isPlayerInside)
            {
                if (TXRPlayer.Instance.FocusedObject == _piece.imageCollider)
                {
                    if (other.CompareTag("PlayerHead"))
                    {
                        TXRDataManager.Instance.LogLineToFile("Player is inside the zone of " + _piece.gameObject.name + ", zone radius in cm: " + transform.localScale.x);
                        Debug.Log("Player is inside the zone of " + _piece.gameObject.name + ", zone radius in cm: " + transform.localScale.x);
                        _isPlayerInside = true;
                        ShowDebugText("Player is inside the zone of " + _piece.gameObject.name, Color.green).Forget();
                    }
                }
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (_isPlayerInside)
        {
            if (other.CompareTag("PlayerHead"))
            {

                TXRDataManager.Instance.LogLineToFile("Player exited the zone of " + _piece.gameObject.name + ", zone radius in cm: " + transform.localScale.x);
                Debug.Log("Player exited the zone of " + _piece.gameObject.name + ", zone radius in cm: " + transform.localScale.x);
                _isPlayerInside = false;
                ShowDebugText("Player exited the zone of " + _piece.gameObject.name, Color.red).Forget();
            }
        }
    }

    public void Activate()
    {
        _isActive = true;
    }
    public void Deactivate()
    {
        _isActive = false;
        _isPlayerInside = false; //is this right?
    }

    async UniTask ShowDebugText(string text, Color color)
    {
        if (_debugText != null)
        {
            _debugText.text = text;
            _debugText.color = color;
            await UniTask.Delay(TimeSpan.FromSeconds(3));
        }


        _debugText.text = "";
    }
}


