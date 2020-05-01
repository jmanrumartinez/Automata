using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class patrol : AI_Agent {
    enum Side {
        LEFT,
        RIGHT
    }

    Vector3[] waypoints;
    public Transform target;
    public int maxWaypoints = 10;
    int actualWaypoint = 0;

    float angularVelocity = 5f;

    float halfAngle = 30.0f;
    float coneDistance = 10.0f;
    float idleAttackDistance = 5.0f;

    void initPositions() {
        List<Vector3> waypointsList = new List<Vector3>();
        float anglePartition = 360.0f / (float)maxWaypoints;
        for (int i = 0; i < maxWaypoints; ++i) {
            Vector3 v = transform.position + 5 * Vector3.forward * Mathf.Cos(i * anglePartition)
                + 5 * Vector3.right * Mathf.Sin(i * anglePartition);
            waypointsList.Add(v);

        }
        waypoints = waypointsList.ToArray();
    }

    private void OnDrawGizmos() {

        if (Application.isPlaying) {
            if (waypoints.Length > 0) {
                for (int i = 0; i < maxWaypoints; i++) {
                    Gizmos.DrawSphere(waypoints[i], 1.0f);
                }
            }

            Gizmos.color = Color.red;
            Vector3 rightSide = Quaternion.Euler(Vector3.up * halfAngle) * transform.forward * coneDistance;
            Vector3 leftSide = Quaternion.Euler(Vector3.up * -halfAngle) * transform.forward * coneDistance;

            Gizmos.DrawLine(transform.position, transform.position + transform.forward * coneDistance);
            Gizmos.DrawLine(transform.position, transform.position + rightSide);
            Gizmos.DrawLine(transform.position, transform.position + leftSide);

            Gizmos.DrawLine(transform.position + leftSide, transform.position + transform.forward * coneDistance);
            Gizmos.DrawLine(transform.position + rightSide, transform.position + transform.forward * coneDistance);

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(target.position, transform.position);
        }
    }

    void idle() {
        setState(getState("goto"));
    }

    void goToWaypoint() {
        // Waypoint Logic
        float signedAngle = Vector3.SignedAngle(transform.forward, waypoints[actualWaypoint] - transform.position, transform.up);
        float angleToGo = Mathf.Min(angularVelocity, Mathf.Abs(signedAngle));
        angleToGo *= Mathf.Sign(signedAngle);
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + angleToGo, transform.rotation.eulerAngles.z);
        transform.position += transform.forward * angularVelocity * Time.deltaTime;

        if (Vector3.Distance(waypoints[actualWaypoint], transform.position) <= 1.0f) {
            setState(getState("nextwp"));
        }

        // Check Player inside range
        if (Vector3.Distance(target.position, transform.position) < coneDistance &&
            Vector3.Angle(transform.forward, target.position - transform.position) <= halfAngle) {
            setState(getState("goToPlayer"));
        }
    }

    void calculateNextWaypoint() {
        actualWaypoint = (++actualWaypoint) % waypoints.Length;
        setState(getState("goto"));
    }

    Side side;
    float angleToRotate;
    float myAngle;

    void Orbit() {
        if (side.Equals(Side.LEFT)) {
            float angleToGo = Mathf.Min(angularVelocity, Mathf.Abs(angleToRotate));
            float dist = Vector3.Distance(transform.position, target.transform.position);
            myAngle += Mathf.Min(angleToGo, angularVelocity);
            transform.position += dist * transform.forward;
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x,
                transform.rotation.eulerAngles.y + angleToGo, transform.rotation.eulerAngles.z);
            transform.position += dist * -transform.forward;
            if (myAngle > angleToRotate) {
                setState(getState("idleAttack"));
            }

        }

        if (side.Equals(Side.RIGHT)) {
            float angleToGo = Mathf.Min(angularVelocity, Mathf.Abs(angleToRotate * -1));
            float dist = Vector3.Distance(transform.position, target.transform.position);

            myAngle -= Mathf.Min(angleToGo, angularVelocity);
            transform.position += dist * transform.forward;
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x,
                transform.rotation.eulerAngles.y - angleToGo, transform.rotation.eulerAngles.z);
            transform.position += dist * -transform.forward;
            if (Mathf.Abs(myAngle) > angleToRotate) {
                setState(getState("idleAttack"));
            }

        }
    }

    void idleAttack() {
        int randSelection = (int)Random.Range(0f, 2f);

        switch (randSelection) {
            case 0:
                // Repeat idleAttack
                setState(getState("idleAttack"));
                break;
            case 1:
                // Orbit
                side = (Side)Random.Range(0, 2);
                angleToRotate = Random.Range(0, 181);
                myAngle = 0;
                setState(getState("orbit"));
                break;
            case 2:
                // Fight
                Debug.Log("Fight");
                break;
        }
    }

    void goToPlayer() {
        float signedAngle = Vector3.SignedAngle(transform.forward, target.transform.position - transform.position, transform.up);
        float angleToGo = Mathf.Min(angularVelocity, Mathf.Abs(signedAngle));
        angleToGo *= Mathf.Sign(signedAngle);
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + angleToGo, transform.rotation.eulerAngles.z);
        transform.position += transform.forward * angularVelocity * 2.0f * Time.deltaTime;

        if (Vector3.Distance(target.position, transform.position) < idleAttackDistance &&
            Vector3.Angle(transform.forward, target.position - transform.position) <= halfAngle) {
            setState(getState("idleAttack"));
        }
    }

    void Start() {
        initPositions();
        actualWaypoint = 0;
        initState("idle", idle);
        initState("goto", goToWaypoint);
        initState("nextwp", calculateNextWaypoint);
        initState("goToPlayer", goToPlayer);
        initState("idleAttack", idleAttack);
        initState("orbit", Orbit);

        setState(getState("idle"));
    }
}
