using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveTo : MonoBehaviour {
	
	public Transform goal;
	public NavMeshAgent agent;
	public float timeScale = 1.0f;
	
	private readonly List<string> nodes = new List<string>();
	
	// Use this for initialization
	void Start () {
		Debug.Log("Start");
		agent = GetComponent<NavMeshAgent>();
		agent.destination = goal.position;
		Time.timeScale = timeScale;

		var paths = GameObject.FindGameObjectsWithTag("paths");
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

		if (colliderName == goal.gameObject.name) // Goal reached
		{
			Time.timeScale = 1;
			ShowPath();
		}
	}

	private void ShowPath()
	{
		var paths = new List<string>();
		
		for (var i = 0; i < nodes.Count; i++)
		{
			if (i == nodes.Count - 1) // last
			{
				break;
			}

			var pathName = nodes[i] + nodes[i + 1];

			GameObject.Find(pathName).GetComponent<Renderer>().enabled = true;		
		}			
	}
}
