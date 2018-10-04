using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Navigation;
using UnityEngine;

public class NavUtils  
{
   
    public static Dictionary<string, Node> CreateGraph(GameObject[] waypoints, GameObject[] paths)
    {
        var relationships = new Dictionary<string, Node>();

        foreach (var waypoint in waypoints)
        {
            relationships[waypoint.name] = new Node(waypoint.transform);
        }

        foreach (var path in paths)
        {
            var waypointsInPath = path.name.Split(new string[] {"to"}, StringSplitOptions.RemoveEmptyEntries);
            // The origin is the second arg because Unity switched it when imported from Maya for some reason
            relationships[waypointsInPath[1]].AddNeighbor(relationships[waypointsInPath[0]]);
        }

        return relationships;
    }
}
