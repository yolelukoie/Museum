using UnityEngine;

public class ArtPieceProximityDetector : MonoBehaviour
{

    Piece _piece;
    bool _isActive = false;
    bool _isPlayerInside = false;
    private void Awake()
    {
        _piece = GetComponentInParent<Piece>();
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
}


