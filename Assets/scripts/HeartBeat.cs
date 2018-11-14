using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartBeat : MonoBehaviour {

    private Vector3 tmp;
    private bool expanding = true;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        tmp = transform.localScale;
        if (expanding)
        {
            tmp.x += 0.01f;
            tmp.y += 0.01f;
            tmp.z += 0.01f;
        } else {
            tmp.x -= 0.01f;
            tmp.y -= 0.01f;
            tmp.z -= 0.01f;
        }

        if (tmp.x > 0.4){
            expanding = false;
        }
        if (tmp.x < 0.2){
            expanding = true;
        }
        transform.localScale = tmp;
	}
}
