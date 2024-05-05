using UnityEngine;

public class Arrow : MonoBehaviour
{


    void Update()
    {
        transform.position.Set(transform.position.x, Mathf.Sin(Time.time), transform.position.z);

    }
}
