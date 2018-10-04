using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Navigation
{   
    public class Dijkstra : INavigation
    {
        public List<string> GetShortestPath(Dictionary<string, Node> graph, string from, string to)
        {
            var distances = CreateDistanceDict(graph, from);
            var previousNodes = new Dictionary<string, string>();
            var nodesToCheck = graph.Keys.ToList();
            var finalPath = new List<string>();
            
            while (nodesToCheck.Count > 0)
            {
                var currentMinNodeName = GetMinDistanceNodeName(nodesToCheck, distances);
                var currentMinNode = graph[currentMinNodeName];
                nodesToCheck.Remove(currentMinNodeName);

                if (currentMinNodeName == to)
                {
                    finalPath.Add(currentMinNodeName);
                    while (previousNodes.ContainsKey(currentMinNodeName))
                    {
                        currentMinNodeName = previousNodes[currentMinNodeName];
                        finalPath.Add(currentMinNodeName);
                    }

                    break;
                }
                
                foreach (var neighbor in currentMinNode.Neighbors)
                {
                    var newDistance = currentMinNode.DistanceTo(neighbor) + distances[currentMinNodeName];
                    if (newDistance < distances[neighbor.Name])
                    {
                        distances[neighbor.Name] = newDistance;
                        previousNodes[neighbor.Name] = currentMinNodeName;
                    }
                }
            }

            return finalPath;
        }

        private Dictionary<string, float> CreateDistanceDict(Dictionary<string, Node> graph, string from)
        {
            var distances = new Dictionary<string, float>();
            foreach (var key in graph.Keys)
            {
                distances.Add(key, float.MaxValue);
            }

            distances[from] = 0;

            return distances;
        }

        private string GetMinDistanceNodeName(List<string> nodes, Dictionary<string, float> distances)
        {
            var minDistance = distances[nodes[0]];
            var minKey = nodes[0];

            foreach (var node in nodes)
            {
                if (distances[node] < minDistance)
                {
                    minDistance = distances[node];
                    minKey = node;
                }
            }        

            return minKey;
        }
    }
}
