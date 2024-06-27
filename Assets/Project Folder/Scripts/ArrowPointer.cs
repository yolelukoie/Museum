using UnityEngine;

public class ArrowPointer : MonoBehaviour
{
    public Transform target; // Reference to the target GameObject
    private Transform _playerHead;

    public float radius;
    public float heightOffset;

    private float initialRadius;

    void Update()
    {
        if (target != null)
        {

            Vector3 direction = (target.position - _playerHead.position).normalized;
            transform.position = _playerHead.position + direction * radius + Vector3.up * heightOffset;
            transform.LookAt(target);


            //// Calculate the direction from the arrow to the target
            //Vector3 direction = target.position - _playerHead.position;

            //// Calculate the rotation required to point in that direction
            //Quaternion rotation = Quaternion.LookRotation(direction);

            //// Apply the rotation to the arrow (only rotate around the Y axis if it's a 2D arrow)
            //transform.rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);
        }
    }

    public void Show() { gameObject.SetActive(true); }
    public void Hide() { gameObject.SetActive(false); }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void ShowAndSetTarget(Transform newTarget)
    {
        SetTarget(newTarget);
        Show();
    }

    private void Start()
    {
        _playerHead = TXRPlayer.Instance.PlayerHead;
        initialRadius = radius;
    }

    private void UpdateRadius()
    {
        Vector3 distanceToTarget = target.position - _playerHead.position;
        if (distanceToTarget.magnitude < radius)
        {
            radius = distanceToTarget.magnitude;
        }
        else
        {
            radius = initialRadius;
        }
    }

}
