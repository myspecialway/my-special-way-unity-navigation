using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

namespace Navigation
{
	public class Navigator
	{
		private string goalName;

		private readonly GameObject[] _paths;

		private readonly INavigation _navigation;
		
		private readonly AudioSource _startNav;

		private readonly Dictionary<string, Node> _graph;
		
		public Navigator(INavigation navigation, AudioSource startNav)
		{
			_paths = GameObject.FindGameObjectsWithTag("Path");
			HideAllPaths();
			_graph = NavUtils.CreateGraph(GameObject.FindGameObjectsWithTag("Waypoint"), _paths);
			_navigation = navigation;
			_startNav = startNav;
		}
	
		public void Navigate(string from, string to)
		{
			var path = _navigation.GetShortestPath(_graph, from, to);
			Debug.Log("Path found - " + path.ToString());
			ShowPath(path);
			_startNav.PlayOneShot(_startNav.clip);
		}

		private void HideAllPaths()
		{
			foreach (var path in _paths)
			{
				SetPathRenderersEnable(path, false);
			}
		}		

		private void ShowPath(List<string> path)
		{
			for (var i = path.Count - 1; i >= 0; i--)
			{				
				if (i == 0) break; // last

				var temp = GameObject.Find(path[i - 1] + "to" + path[i]);
				if (temp == null) temp = GameObject.Find(path[i] + "to" + path[i-1]);
				
				if(temp != null) SetPathRenderersEnable(temp, true);
			}
		}

		private void EnablePath(string pathName)
		{		
			foreach (var path in _paths)
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
