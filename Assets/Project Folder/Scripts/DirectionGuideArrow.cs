using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

public class DirectionGuideArrow : MonoBehaviour
{
    [Tooltip("distance from player")]
    public float radius = 1f;
    public float heightOffset = 1f;
    public float movementDuration = 2f;
    public float InstructionBoardHeightOffset = 0.5f;

    private Vector3 disableYVec = new Vector3(1, 0, 1);

    public Transform _target;
    private Transform _playerHead;
    [ShowInInspector]
    private bool _shouldUpdatePosition = false;
    private InstructionsBoard _followTheArrowInstruction;
    private MeshRenderer _meshRenderer;

    // Start is called before the first frame update
    private void Start()
    {
        _playerHead = TXRPlayer.Instance.PlayerHead;
        _followTheArrowInstruction = SceneReferencer.Instance.followTheArrow;
        _meshRenderer = GetComponent<MeshRenderer>();
        _shouldUpdatePosition = false;

    }

    // Update is called once per frame
    private void Update()
    {
        //UpdatePositionTowardsTarget();
    }

    public void Show() { gameObject.SetActive(true); }
    public void Hide() { gameObject.SetActive(false); }
    public void SetTarget(Transform newTarget)
    {
        _target = newTarget;
    }

    private Vector3 calculateFirstPosition()
    {
        Start();
        Debug.Log("DEBUG: calculateFirstPosition");
        _shouldUpdatePosition = false;
        Vector3 directionToTarget = (_target.position - _playerHead.position).normalized;
        Vector3 firstPosition = _playerHead.position + (Vector3.Scale(_playerHead.forward, disableYVec).normalized * radius);
        firstPosition.y = _playerHead.position.y + heightOffset; // Set y position with heightOffset
        Debug.Log("DEBUG: calculateFirstPosition 1");

        // Determine the direction to face
        Vector3 cross = Vector3.Cross(_playerHead.forward, directionToTarget);
        if (cross.y > 0)
        {
            // Face left
            transform.rotation = Quaternion.LookRotation(Vector3.Cross(Vector3.up, _playerHead.forward));
        }
        else
        {
            // Face right
            transform.rotation = Quaternion.LookRotation(Vector3.Cross(_playerHead.forward, Vector3.up));
        }
        Debug.Log("DEBUG: calculateFirstPosition 2");
        return firstPosition;
    }

    private Vector3 calculatePositionTowardsTarget()
    {
        Vector3 targetPosition = _playerHead.position + radius * (_target.position - _playerHead.position).normalized;
        targetPosition.y = _playerHead.position.y + heightOffset; // Set y position with heightOffset
        return targetPosition;
    }

    private void MoveOnCircle(Vector3 pointA, Vector3 pointB, Vector3 center, float duration)
    {
        // Calculate the angle between point A and point B relative to the center
        Vector3 dirA = (pointA - center).normalized;
        Vector3 dirB = (pointB - center).normalized;
        float angleA = Mathf.Atan2(dirA.z, dirA.x);
        float angleB = Mathf.Atan2(dirB.z, dirB.x);

        // Ensure the shortest path is taken
        if (Mathf.Abs(angleB - angleA) > Mathf.PI)
        {
            if (angleB > angleA)
            {
                angleA += 2 * Mathf.PI;
            }
            else
            {
                angleB += 2 * Mathf.PI;
            }
        }

        // Calculate waypoints along the circular path
        int numWaypoints = 50; // Number of waypoints
        Vector3[] waypoints = new Vector3[numWaypoints];
        for (int i = 0; i < numWaypoints; i++)
        {
            float t = (float)i / (numWaypoints - 1);
            float angle = Mathf.Lerp(angleA, angleB, t);
            waypoints[i] = center + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
            waypoints[i].y = center.y + heightOffset; // Keep the y value constant with heightOffset
        }

        // Move the object along the waypoints and rotate smoothly towards the target
        transform.DOPath(waypoints, duration, PathType.CatmullRom).SetEase(Ease.InOutCubic);
        transform.DOLookAt(_target.position, duration, AxisConstraint.Y).SetEase(Ease.InOutCubic);

    }



    private void UpdatePositionTowardsTarget()
    {
        if (!_shouldUpdatePosition)
        {
            return;
        }
        transform.position = calculatePositionTowardsTarget();
        transform.LookAt(_target);

    }

    public async UniTask ShowAndSetTarget(Transform newTarget, bool showInstruction)
    {
        Show();
        SetTarget(newTarget);
        transform.position = calculateFirstPosition();
        if (showInstruction)
        {
            SetInstructionsPositionAndShow();
        }

        await UniTask.Delay(TimeSpan.FromSeconds(1.5f));
        MoveOnCircle(transform.position, calculatePositionTowardsTarget(), _playerHead.position, movementDuration);
        _shouldUpdatePosition = true;
    }

    private void SetInstructionsPositionAndShow()
    {
        _followTheArrowInstruction.transform.position = transform.position + new Vector3(0, InstructionBoardHeightOffset, 0);
        _followTheArrowInstruction.transform.rotation = Quaternion.LookRotation(_playerHead.position - _followTheArrowInstruction.transform.position, Vector3.up) * Quaternion.Euler(0, 180, 0);
        _followTheArrowInstruction.ShowUntilAudioEnds().Forget();
    }


    #region Debug
    [Button]
    public async UniTask TestAsync()
    {
        Show();
        transform.position = calculateFirstPosition();
        SetInstructionsPositionAndShow();
        await UniTask.Delay(TimeSpan.FromSeconds(1));
        MoveOnCircle(transform.position, calculatePositionTowardsTarget(), _playerHead.position, movementDuration);
    }
    #endregion
}
