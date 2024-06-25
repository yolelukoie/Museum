using UnityEngine;

public class ArrowPointer : MonoBehaviour
{
    public Transform target; // Reference to the target GameObject

    void Update()
    {
        if (target != null)
        {
            // Calculate the direction from the arrow to the target
            Vector3 direction = target.position - transform.position;

            // Calculate the rotation required to point in that direction
            Quaternion rotation = Quaternion.LookRotation(direction);

            // Apply the rotation to the arrow (only rotate around the Y axis if it's a 2D arrow)
            transform.rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);
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

}
