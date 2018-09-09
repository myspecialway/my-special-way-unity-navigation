using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Navigation : MonoBehaviour {
	
	[SerializeField]
	private NavMeshAgent agent;
	
	[SerializeField]
	private float timeScale = 1.0f;

	private string goalName;
	
	private readonly List<string> nodes = new List<string>();

	private bool inProgress;

	private GameObject[] paths;
	
	// Use this for initialization
	private void Start () {
		Time.timeScale = timeScale;
		paths = GameObject.FindGameObjectsWithTag("paths");
		HidePaths();
	}

	public IEnumerator CalculateNavAndShowPath(Transform goal)
	{
		if (inProgress)
		{
			throw new Exception("Already in progress");	
		}
		
		inProgress = true;

		this.goalName = goal.gameObject.name;

		var success = agent.SetDestination(goal.position);

		if (!success)
		{
			yield break;
		}
		
		while (inProgress)
		{
			yield return null;
		}
	}

	private void HidePaths()
	{
		foreach (var path in paths)
		{
			path.GetComponent<Renderer>().enabled = false;
		}
	}
	
	private void OnTriggerEnter(Collider other)
	{
		var colliderName = other.gameObject.name;
		
		Debug.Log("TriggerEnter " + colliderName);
		nodes.Add(colliderName);

		if (colliderName == goalName) // Goal reached
		{
			DestinationReached();
		}
	}

	private void DestinationReached()
	{
		Time.timeScale = 1;
		ShowPath();
		inProgress = false;
	}

	private void ShowPath()
	{		
		for (var i = 0; i < nodes.Count; i++)
		{
			if (i == nodes.Count - 1) // last
			{
				break;
			}

			var pathName = nodes[i] + nodes[i + 1];
			
			EnablePath(pathName);
		}
	}

	private void EnablePath(string pathName)
	{
		
		foreach (var path in paths)
		{
			if (path.name == pathName)
			{
				path.GetComponent<Renderer>().enabled = true;
			}			
		}
	}
}
