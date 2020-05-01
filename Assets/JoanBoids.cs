using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoanBoids : MonoBehaviour {
    [SerializeField] GameObject agentPrefab;

    [SerializeField] int numBoids = 10;

    JoanBoidsAgent[] agents;

    [SerializeField] float agentRadius = 5.0f;

    [SerializeField] float separationWeight = 1.0f, cohesionWeight = 1.0f, alignmentWeight = 1.0f;

    private void Awake() {
        List<JoanBoidsAgent> agentlist = new List<JoanBoidsAgent>();

        for (int i = 0; i < numBoids; i++) {
            Vector3 position = Vector3.up * Random.Range(0, 10)
                + Vector3.right * Random.Range(0, 10) + Vector3.forward * Random.Range(0, 10);

            JoanBoidsAgent newAgent = Instantiate(agentPrefab, position, Quaternion.identity).GetComponent<JoanBoidsAgent>();
            newAgent.SetRadius(agentRadius); 
            agentlist.Add(newAgent);

        }
        agents = agentlist.ToArray();
    }

    // Update is called once per frame
    void Update() {
        foreach (JoanBoidsAgent a in agents) {
            a.velocity = a.vel;
            a.neightbours.Clear();
            a.checkNeightbours();
            calculateCohesion(a);
            calculateSeparation(a);
            calculateAlignment(a);
            a.updateAgent();
        }
    }

    void calculateSeparation(JoanBoidsAgent a) {
        Vector3 separationVector = Vector3.zero;
        foreach (JoanBoidsAgent neightbour in a.neightbours) {
            if (!neightbour) return;
            float distance = Vector3.Distance(a.transform.position, neightbour.transform.position);
            distance /= a.radius;
            distance = 1 - distance;
            separationVector += distance * (a.transform.position - neightbour.transform.position).normalized * separationWeight;


        }
        a.addForce(separationVector, JoanBoidsAgent.DEBUGforceType.SEPARATION);
    }

    void calculateCohesion(JoanBoidsAgent a) {
        Vector3 centralPosition = new Vector3();

        foreach (JoanBoidsAgent neightbour in a.neightbours) {
            if (!neightbour) return;
            centralPosition += neightbour.transform.position; 
        }
        centralPosition += a.transform.position;
        centralPosition /= a.neightbours.Count + 1;
        a.addForce((centralPosition - a.transform.position) * cohesionWeight, JoanBoidsAgent.DEBUGforceType.COHESION);
    }

    void calculateAlignment(JoanBoidsAgent a) {
        Vector3 dirVec = new Vector3();

        foreach (JoanBoidsAgent neightbour in a.neightbours) {
            if (!neightbour) return;
            dirVec += neightbour.velocity; 
        }

        dirVec += a.velocity;
        dirVec /= a.neightbours.Count + 1;
        a.addForce(dirVec, JoanBoidsAgent.DEBUGforceType.ALIGNMENT);
    }
}
