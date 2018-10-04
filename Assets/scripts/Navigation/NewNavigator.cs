using System;
using System.Collections;
using System.Collections.Generic;
using System.Media;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

namespace Navigation
{
	public class NewNavigator : MonoBehaviour
	{
		private string goalName;

		private readonly List<string> nodes = new List<string>();

		private GameObject[] paths;

		private Dijkstra _dijkstra;
		
		[SerializeField]
		private AudioSource startNav;

		// Use this for initialization
		private void Start()
		{
			paths = GameObject.FindGameObjectsWithTag("Path");
			HideAllPaths();
			var graph = NavUtils.CreateGraph(GameObject.FindGameObjectsWithTag("Waypoint"), paths);
			_dijkstra = new Dijkstra(graph);
			var path = _dijkstra.GetShortestPath(graph[21], graph[2]);
			for (var i = path.Count - 1; i >= 0; i--)
			{				
				if (i == 0) break; // last

				var temp = GameObject.Find(path[i - 1].Name + "to" + path[i].Name);
				SetPathRenderersEnable(temp, true);
			}
		}
	
		public void NavigateTo(Transform goal)
		{
		}

		private void HideAllPaths()
		{
			foreach (var path in paths)
			{
				SetPathRenderersEnable(path, false);
			}
		}		

		private void DestinationReached()
		{
			Debug.Log("DestinationReached");
			startNav.PlayOneShot(startNav.clip);
			ShowPath();
		}

		private void ShowPath()
		{
			for (var i = 0; i < nodes.Count; i++)
			{
				if (i == nodes.Count - 1) break; // last

				var pathName = nodes[i + 1] + "to" + nodes[i];

				EnablePath(pathName);
			}
		}

		private void EnablePath(string pathName)
		{		
			foreach (var path in paths)
			{
				if (path.name == pathName)
					SetPathRenderersEnable(path, true);
			}
		}
		
		private void SetPathRenderersEnable(GameObject path, bool enable)
		{
			var renderers = path.GetComponentsInChildren<MeshRenderer>();
			foreach (var rend in renderers)
				rend.enabled = enable;
		}
	}
}
