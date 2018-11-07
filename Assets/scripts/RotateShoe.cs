using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateShoe : MonoBehaviour {

    public int speed;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(0, speed , 0);
	}
}
