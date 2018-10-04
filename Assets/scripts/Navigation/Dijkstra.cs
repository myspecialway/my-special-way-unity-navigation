using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Navigation
{   
    public class Dijkstra 
    {
        private class ExtendedNode
        {
            public Node Node { private set; get; }

            public float Distance { set; get; }
            
            public ExtendedNode PreviousSmallest { set; get; }            

            public bool IsChecked { set; get; }
            
            public ExtendedNode(Node node)
            {
                Node = node;
                Distance = float.MaxValue;
                IsChecked = false;
            }
        }
        
        private readonly List<ExtendedNode> _nodes;

        public Dijkstra(Node[] nodes)
        {
            _nodes = new List<ExtendedNode>(nodes.Length);
            foreach (var node in nodes)
            {
                _nodes.Add(new ExtendedNode(node));
            }
        }

        public List<Node> GetShortestPath(Node from, Node to)
        {
         
            ResetPathsList(from);
            var path = new List<Node>();

            while (_nodes.Count > 0)
            {
                var currentMinNode = GetCurrentShortestPath();

                if (currentMinNode == null)
                {
                    break;
                }

                currentMinNode.IsChecked = true;

                if (currentMinNode.Node == to)
                {
                    var insert = currentMinNode;
                    do
                    {
                        path.Add(insert.Node);
                    } while ((insert = insert.PreviousSmallest) != null);

                    break;
                }
                
                foreach (var neighbor in currentMinNode.Node.Neighbors)
                {
                    var newDistance = currentMinNode.Node.DistanceTo(neighbor) + currentMinNode.Distance;
                    var neighborExtendedNode = FindNode(neighbor);
                    if (newDistance < neighborExtendedNode.Distance)
                    {
                        neighborExtendedNode.Distance = newDistance;
                        neighborExtendedNode.PreviousSmallest = currentMinNode;
                    }
                }
            }

            return path;
        }

        private void ResetPathsList(Node start)
        {
            foreach (var node in _nodes)
            {
                node.Distance = (node.Node == start) ? 0 : float.MaxValue;
                node.IsChecked = false;
            }
        }

        private ExtendedNode GetCurrentShortestPath()
        {
            ExtendedNode minNode = null;
            
            foreach (var node in _nodes)
            {
                if (!node.IsChecked && (minNode == null || node.Distance.CompareTo() node.Distance < minNode.Distance))
                {
                    minNode = node;
                } 
            }           

            return minNode;
        }

        private ExtendedNode FindNode(Node node)
        {
            foreach (var extendedNode in _nodes)
            {
                if (extendedNode.Node == node)
                {
                    return extendedNode;
                }
            }

            return null;
        }
    }
}
