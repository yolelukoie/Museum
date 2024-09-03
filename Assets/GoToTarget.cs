using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

public class GoToTarget : MonoBehaviour
{
    public Transform player;
    public Transform target;
    public float distanceFromPlayer = 2.0f;
    private NavMeshPath path;
    private int currentPathIndex = 0;

    public bool isPathCalculated = false;

    private Vector3 nextPoint;
    private Vector3 directionToNextPoint;
    private Vector3 closestPointToPlayer;

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
            Vector3 _;

            (closestPointToPlayer, _) = FindClosestPointOnPathAndEndOfSegment(player.position);
            (_, nextPoint) = FindClosestPointOnPathAndEndOfSegment(transform.position);
            directionToNextPoint = nextPoint - closestPointToPlayer;

            // Check if the arrow is close enough to the next point
            //if (directionToNextPoint.magnitude < distanceFromPlayer)
            //{
            //    currentPathIndex++;
            //    if (currentPathIndex >= path.corners.Length)
            //    {
            //        currentPathIndex = path.corners.Length - 1;
            //    }
            //}

            // Move the arrow to the next point
            Vector3 arrowPosition = closestPointToPlayer + directionToNextPoint.normalized * distanceFromPlayer;
            transform.position = arrowPosition;
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
        NavMesh.CalculatePath(player.position, target.position, NavMesh.AllAreas, path);
        isPathCalculated = true;
    }


    (Vector3, Vector3) FindClosestPointOnPathAndEndOfSegment(Vector3 playerPosition)
    {
        Vector3 closestPoint = path.corners[0];
        float closestDistance = Vector3.Distance(playerPosition, closestPoint);
        Vector3 nextEndSegment = path.corners[0];
        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            Vector3 segmentStart = path.corners[i];
            Vector3 segmentEnd = path.corners[i + 1];
            Vector3 closestPointOnSegment = PointProjection.ProjectPointOnLine(playerPosition, segmentStart, segmentEnd);//GetClosestPointOnSegment(playerPosition, segmentStart, segmentEnd);

            float distance = Vector3.Distance(playerPosition, closestPointOnSegment);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPoint = closestPointOnSegment;
                nextEndSegment = segmentEnd;
            }

        }

        return (closestPoint, nextEndSegment);
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
                remainingDistance = Vector3.Distance(closestPointToPlayer, segmentEnd);

            }

        }
        if (remainingDistance < distanceFromPlayer)
        {

        }


        return (closestPointToPlayer, nextEndSegment);
    }

    Vector3 GetClosestPointOnSegment(Vector3 point, Vector3 segmentStart, Vector3 segmentEnd)
    {
        Vector3 segmentDirection = segmentEnd - segmentStart;
        float segmentLength = segmentDirection.magnitude;
        segmentDirection.Normalize();

        float projection = Vector3.Dot(point - segmentStart, segmentDirection);
        projection = Mathf.Clamp(projection, 0, segmentLength);

        return segmentStart + segmentDirection * projection;
    }

    //Vector3 FindNextCorner(Vector3 pointOnPath)
    //{
    //    for (int i = 0; i < path.corners.Length - 1; i++)
    //    {
    //        Vector3 segmentStart = path.corners[i];
    //        Vector3 segmentEnd = path.corners[i + 1];
    //        if (Vector3.Distance(pointOnPath, segmentEnd) < Vector3.Distance(pointOnPath, segmentStart))
    //        {
    //            return segmentEnd;
    //        }
    //    }
    //    return path.corners[path.corners.Length - 1]; // Return the last corner if no next corner is found

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