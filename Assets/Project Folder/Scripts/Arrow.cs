using UnityEngine;

public class Arrow : MonoBehaviour
{

    private float _movementScalar = 0.002f;
    private void Update()
    {
        this.transform.Translate(0, Mathf.Sin(Time.time) * _movementScalar, 0);
    }
}
