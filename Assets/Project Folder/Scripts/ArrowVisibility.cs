using UnityEngine;

public class ArrowVisibility : MonoBehaviour
{
    private MeshRenderer _meshRenderer;
    private Material _material;
    private Color _originalColor;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _material = _meshRenderer.material;
        _originalColor = _material.color;
    }

    private void Update()
    {
        AdjustOpacity();
    }

    private void AdjustOpacity()
    {
        Ray ray = new Ray(Camera.main.transform.position, transform.position - Camera.main.transform.position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform != transform)
            {
                // Arrow is behind another object
                Color color = _material.color;
                color.a = 0.5f; // Set to a lower opacity
                _material.color = color;
            }
            else
            {
                // Arrow is not behind another object
                _material.color = _originalColor;
            }
        }
    }
}
