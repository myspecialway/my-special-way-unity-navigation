using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveZ : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (transform.localPosition.z < -1f)
        {
            transform.localPosition = Vector3.zero;
        }
        else
        {
            transform.Translate(0, 0, -Time.deltaTime);
        }
    }
}
