using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpeechArea : MonoBehaviour
{
    public float InnerRadius = 10f;
    public float OuterRadius = 20f;
    public float Angle = 180f;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        
        DrawArc(transform.position, OuterRadius, Angle); //draw outer arc

        DrawArc(transform.position,  InnerRadius, Angle);  //draw inner arc

        Vector3 startOuter = GetArcPoint(transform.position, OuterRadius, -Angle /2);
        Vector3 startInner = GetArcPoint(transform.position, InnerRadius, -Angle /2);
        Vector3 endOuter = GetArcPoint(transform.position, OuterRadius, Angle /2);
        Vector3 endInner = GetArcPoint(transform.position, InnerRadius, Angle /2);

        Gizmos.DrawLine(startOuter, startInner);
        Gizmos.DrawLine (startOuter, endOuter);
    }

    private void DrawArc(Vector3 center, float radius, float angle)
    {
        int segments = 50; //Nu,ber of segments to approximate the arc
        float step = angle / segments;

        for (int i = 0; i < segments; i++)
        {
            float currentAngle = -angle / 2 + step * i;
            float nextAngle = currentAngle + step;

            Vector3 point1 = GetArcPoint(center, radius, currentAngle);
            Vector3 point2 = GetArcPoint(center, radius, nextAngle);

            Gizmos.DrawLine(point1, point2);
        }
    }

    private Vector3 GetArcPoint(Vector3 center, float radius, float angle)
    {
        float rad = Mathf.Deg2Rad * angle;

        Vector3 localPoint = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad)) * radius;
        Vector3 rotatedPoint = transform.rotation * localPoint; 

        return center + rotatedPoint;
    }

    public Vector3 GetRandomPoint()
    {
        float randomAngle = Random.Range (-Angle / 2, Angle / 2);
        float randomDistance = Random.Range(InnerRadius, OuterRadius);

        //calculate the random point
        Vector3 localPoint = new Vector3(
            Mathf.Cos(Mathf.Deg2Rad * randomAngle),
            0f,
            Mathf.Sin(Mathf.Deg2Rad * randomAngle)
            ) * randomDistance;

        Vector3 rotatedPoint = transform.rotation * localPoint;
        Vector3 randomPoint = transform.position + rotatedPoint;

        //Ensure the point is on the NavMesh
        NavMeshHit hit;
        Vector3 finalPosition = transform.position;

        if (NavMesh.SamplePosition(randomPoint, out hit, 2f, NavMesh.AllAreas))
        {
            finalPosition = hit.position;
        }

        return finalPosition;
    }
}
