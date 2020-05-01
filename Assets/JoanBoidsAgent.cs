using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoanBoidsAgent : MonoBehaviour
{
    public Vector3 velocity;

    public Vector3 separationForce, cohesionForce, alignmentForce;
    [SerializeField]
    public float radius = 1.0f;

    public Vector3 vel = Vector3.zero;
    public List<JoanBoidsAgent> neightbours;

    public void SetRadius(float newRadious) {
        radius = newRadious;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + separationForce);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + cohesionForce);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + alignmentForce);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + velocity);
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    private void Awake()
    {
        neightbours = new List<JoanBoidsAgent>();
    }
    
    public enum DEBUGforceType
    {
        SEPARATION,
        COHESION,
        ALIGNMENT
    }
    public void addForce(Vector3 f, DEBUGforceType type)
    {
        switch (type) {
            case DEBUGforceType.SEPARATION:
                separationForce = f;
                break; 
            case DEBUGforceType.COHESION:
                cohesionForce = f;
                break; 
            case DEBUGforceType.ALIGNMENT: 
                alignmentForce = f;
                break;
            default:
                Debug.LogError("NO TYPE ASSIGNED");
                break; 
        }

        velocity += f;
    }
    public void addForce(Vector3 f)
    {
        velocity += f;
    }
    public void updateAgent()
    {
        transform.position += velocity * Time.deltaTime;
    }
    public void checkNeightbours()
    {
        Collider[] checks = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider c in checks)
        {
            neightbours.Add(c.GetComponent<JoanBoidsAgent>());
        }
    }
}
