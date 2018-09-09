using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Init : MonoBehaviour
{

	[SerializeField]
	private Navigation nav;

	[SerializeField]
	private string destName;
	
	private void Start ()
	{
		var dest = GameObject.Find(destName);

		if (dest != null)
		{
			StartCoroutine(nav.CalculateNavAndShowPath(dest.transform));
		}
	}
}
