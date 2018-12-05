using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleGameObjectVisibility : MonoBehaviour
{
	//[SerializeField] private GameObject _pointCloud;

	public void TogglePointCloud(GameObject go)
	{
		go.SetActive(!go.activeSelf);
	}
}
