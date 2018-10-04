using System.Collections.Generic;
using UnityEngine;

namespace Navigation
{
	public class Node
	{
		public string Name => Transform.gameObject.name;
		public Transform Transform { get; private set; }
		public List<Node> Neighbors { get; } = new List<Node>();

		public Node(Transform transform)
		{
			Transform = transform;
		}

		public void AddNeighbor(Node node)
		{
			Neighbors.Add(node);
		}

		public float DistanceTo(Node other)
		{
			return Vector3.Distance(Transform.position, other.Transform.position);
		}
	}
}
