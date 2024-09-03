using UnityEngine;
using UnityEngine.AI;

public class GoToTarget : MonoBehaviour
{
    public Transform player;
    public Transform target;
    public float distanceFromPlayer = 2.0f;
    private NavMeshPath path;
    private int currentPathIndex = 0;

    void Start()
    {
        path = new NavMeshPath();
    }

    void Update()
    {
        // Calculate the path to the target
        NavMesh.CalculatePath(player.position, target.position, NavMesh.AllAreas, path);

        // Move the arrow along the path
        if (path.corners.Length > 1)
        {
            Vector3 nextPoint = path.corners[currentPathIndex];
            Vector3 directionToNextPoint = nextPoint - player.position;

            // Check if the arrow is close enough to the next point
            if (directionToNextPoint.magnitude < distanceFromPlayer)
            {
                currentPathIndex++;
                if (currentPathIndex >= path.corners.Length)
                {
                    currentPathIndex = path.corners.Length - 1;
                }
            }

            // Move the arrow to the next point
            Vector3 arrowPosition = player.position + directionToNextPoint.normalized * distanceFromPlayer;
            transform.position = arrowPosition;
        }
    }
}