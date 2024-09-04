using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

public class GoToTarget : MonoBehaviour
{
    public Transform player;
    public Transform target;
    public float distanceFromPlayer = 3.0f;
    private NavMeshPath path;
    private int currentSegmentEnd = 0;
    public float speed = 1.5f;
    public bool isPathCalculated = false;

    private Vector3 nextPoint;
    private Vector3 directionToNextPoint;
    private Vector3 closestPointToPlayer;
    //private List<Vector3> pathPoints = new List<Vector3>();

    void Start()
    {
        path = new NavMeshPath();
    }

    void Update()
    {
        // Calculate the path to the target


        // Move the arrow along the path
        if (path.corners.Length > 1)
        {

            (closestPointToPlayer, nextPoint) = findPlayerPointOnPathAndArrowPosition();
            if (currentSegmentEnd == path.corners.Length - 1)
            {
                transform.LookAt(target);
            }
            else
            {
                transform.LookAt(path.corners[currentSegmentEnd]);
            }

            transform.position = Vector3.Lerp(transform.position, nextPoint, Time.deltaTime * speed);

        }
    }

    void OnDrawGizmos()
    {
        if (path != null)
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                Gizmos.DrawLine(path.corners[i], path.corners[i + 1]);
                Gizmos.DrawSphere(path.corners[i], 0.05f);
            }

        }
        // Draw the closest point to the player
        if (closestPointToPlayer != Vector3.zero)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(closestPointToPlayer, 0.1f);
        }

        // Draw the next point
        if (nextPoint != Vector3.zero)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(nextPoint, 0.1f);
        }
    }

    [Button("Calculate Path")]
    void CalculatePath()
    {
        path.ClearCorners();

        Vector3 targetPosition = target.position;
        NavMeshHit hit;

        // Sample the closest point on the NavMesh within maxDistance
        if (NavMesh.SamplePosition(targetPosition, out hit, 100, NavMesh.AllAreas))
        {
            // Use hit.position as the target position
            Debug.Log("Closest point on NavMesh: " + hit.position);
        }
        else
        {
            Debug.LogWarning("Target is too far from the NavMesh.");
        }
        NavMesh.CalculatePath(player.position, hit.position, NavMesh.AllAreas, path);
        isPathCalculated = true;
    }


    (Vector3, Vector3) findPlayerPointOnPathAndArrowPosition()
    {
        Vector3 closestPointToPlayer = path.corners[0];
        float closestDistance = Vector3.Distance(player.position, closestPointToPlayer);
        Vector3 nextEndSegment = path.corners[0];
        int startSegmentIndex = 0;
        int endSegmentIndex = 0;
        float remainingDistance = distanceFromPlayer;


        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            Vector3 segmentStart = path.corners[i];
            Vector3 segmentEnd = path.corners[i + 1];
            Vector3 closestPointOnSegment = PointProjection.ProjectPointOnLine(player.position, segmentStart, segmentEnd);//GetClosestPointOnSegment(playerPosition, segmentStart, segmentEnd);

            float distance = Vector3.Distance(player.position, closestPointOnSegment);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPointToPlayer = closestPointOnSegment;
                startSegmentIndex = i;
                endSegmentIndex = i + 1;
            }

        }
        // Calculate the arrow position which is distanceFromPlayer units away from the closest point
        Vector3 currentPoint = closestPointToPlayer;
        Vector3 arrowPosition = path.corners[path.corners.Length - 1];
        for (int i = startSegmentIndex; i < path.corners.Length - 1; i++)
        {

            Vector3 segmentStart = path.corners[i];
            Vector3 segmentEnd = path.corners[i + 1];
            Vector3 direction = (segmentEnd - segmentStart).normalized;
            float segmentLength = Vector3.Distance(segmentStart, segmentEnd);
            float distanceToEnd = Vector3.Distance(currentPoint, segmentEnd);

            if (remainingDistance <= distanceToEnd)
            {
                arrowPosition = currentPoint + direction * remainingDistance;
                currentSegmentEnd = i + 1;
                break;
            }
            else
            {
                remainingDistance -= distanceToEnd;
                currentPoint = segmentEnd;
                currentSegmentEnd = i + 1;
            }
        }
        return (closestPointToPlayer, arrowPosition);

    }



}
public class PointProjection
{
    // Function to project a point onto a line segment
    public static Vector3 ProjectPointOnLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
    {
        // Calculate the direction vector of the line segment
        Vector3 lineDirection = lineEnd - lineStart;
        float lineLength = lineDirection.magnitude;
        lineDirection.Normalize();

        // Calculate the vector from the start point to the point to be projected
        Vector3 pointToStart = point - lineStart;

        // Project the point onto the line using the dot product
        float projectionLength = Vector3.Dot(pointToStart, lineDirection);
        projectionLength = Mathf.Clamp(projectionLength, 0, lineLength); // Clamp to ensure the projection is within the segment

        // Calculate the projected point
        Vector3 projectedPoint = lineStart + lineDirection * projectionLength;

        return projectedPoint;
    }
}