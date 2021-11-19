using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class BoidAvoidanceBehaviour : SteeringBehaviour
{ 
    public float separationDistance = 2f;

    protected override Vector3 CalculateDirection(List<GameObject> neighbours)
    {
        var boidAvoid = new Vector3(0, 0, 0);

        if (neighbours.Count == 0) return boidAvoid;

        neighbours = neighbours
            .Where(x => (x.transform.position - transform.position).sqrMagnitude <=
                        separationDistance * separationDistance).ToList();
        
        foreach (GameObject neigh in neighbours)
        {
            if (neigh == null) continue;
            var dir = neigh.transform.position - transform.position;
            boidAvoid -= (dir.normalized * 2) / dir.sqrMagnitude;
        }

        return boidAvoid;
    }
}