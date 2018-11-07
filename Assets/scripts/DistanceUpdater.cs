using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceUpdater : MonoBehaviour
{
    [SerializeField] private TextMesh _distanceText;
    private Camera _firstPersonCamera;

    // Use this for initialization
    void Start()
    {

    }

    private void Awake()
    {
        _firstPersonCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        CalculateDistance();
    }

    private void CalculateDistance()
    {
        if (_firstPersonCamera == null || _firstPersonCamera.transform == null || _firstPersonCamera.transform.position == null)
        {
            _distanceText.text = "no camera";
            return;
        }
        if (_distanceText == null || _distanceText.transform == null || _distanceText.transform.position == null)
        {
            _distanceText.text = "no text prefab";
            return;
        }
        float dist = Vector3.Distance(_distanceText.transform.position, _firstPersonCamera.transform.position);
        int _dis = (int)dist;
        _distanceText.text = $"{_dis} m";
    }
}
