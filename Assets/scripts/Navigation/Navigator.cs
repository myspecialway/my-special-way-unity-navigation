using System;
using System.Collections;
using System.Collections.Generic;
using System.Media;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

namespace Navigation
{
	public class Navigator : MonoBehaviour
	{
		private static readonly string[] TO_B2 = { "AtoA0", "A1toA", "BtoA1", "B2toB"};
		private static readonly string[] TO_C1 = { "AtoA0", "B12toA", "B10toB12", "B8toB10", "B6toB8", "B4toB6", "B3toB4", "CtoB3", "C1toC"};

		private static readonly string[] TO_B2_NODES = {"A0", "A", "A1", "B", "B2"};
		private static readonly string[] TO_C1_NODES = {"A0", "A", "B12", "B10", "B8", "B6", "B4", "B3", "C", "C1"};

		[SerializeField] private NavMeshAgent agent;

		[SerializeField] private float timeScale = 1.0f;

		private string goalName;

		private readonly List<string> nodes = new List<string>();

		private bool inProgress;

		private GameObject[] paths;

		[SerializeField]
		private AudioSource startNav;

		// Use this for initialization
		private void Start()
		{
			Time.timeScale = timeScale;
//			paths = GameObject.FindGameObjectsWithTag("Path");
//			HideAllPaths();
		}
	
		private void OnTriggerEnter(Collider other)
		{
			if (!inProgress) return;
			
			var colliderName = other.gameObject.name;

			Debug.Log("TriggerEnter " + colliderName);
			nodes.Add(colliderName);

			if (colliderName == goalName) // Goal reached
			{
				DestinationReached();
			}
		}
		
		public IEnumerator NavigateTo(Transform goal)
		{
			paths = GameObject.FindGameObjectsWithTag("Path");
			HideAllPaths();
			nodes.Clear();
			
			switch (goal.name)
			{
				case "B2":
					nodes.AddRange(TO_B2_NODES);
					break;
				case "C1":
					nodes.AddRange(TO_C1_NODES);
					break;
			}
			
			DestinationReached();

//			if (inProgress)
//			{
//				throw new Exception("Already in progress");
//			}
//
//			inProgress = true;
//
//			this.goalName = goal.gameObject.name;
//
//			var success = agent.SetDestination(goal.position);
//
//			if (!success)
//			{
				yield break;
//			}
//
//			while (inProgress)
//			{
//				yield return null;
//			}
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
			Time.timeScale = 1;
			ShowPath();
			inProgress = false;
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
