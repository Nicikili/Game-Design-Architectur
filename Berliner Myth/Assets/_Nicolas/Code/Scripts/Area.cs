using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Area : MonoBehaviour
{
    public float Radius = 20f;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, Radius);  
    }

    public Vector3 GetRandomPoint()
    {

        float distance = Mathf.Sqrt(Random.Range(0f, 1f)) * Radius;
        Vector2 randomCircle = Random.insideUnitCircle.normalized * distance;

        Vector3 randomPoint = transform.position + new Vector3(randomCircle.x, 0f, randomCircle.y);

        //Vector3 randomDirection = Random.insideUnitSphere * Radius;
        //randomDirection.y = 0f;

        //Vector3 randomPoint = transform.position + randomDirection;

        NavMeshHit hit;
        Vector3 finalPosition = transform.position;

        if (NavMesh.SamplePosition(randomPoint, out hit, 10f, 1))
        {
            finalPosition = hit.position;
        }

        return finalPosition;
    }
}
