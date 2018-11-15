using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveX : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (transform.localPosition.x > 1f)
        {
            transform.localPosition = Vector3.zero;
        }
        else
        {
            transform.Translate(Time.deltaTime, 0, 0);
        }
    }
}
