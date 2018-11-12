using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityFader : MonoBehaviour {

    [SerializeField] private TextMesh _distanceText;
    [SerializeField] private GameObject _fromGameObject;
    private Camera _firstPersonCamera;

    // Use this for initialization
    void Start()
    {

    }

    private void Awake()
    {
        _firstPersonCamera = Camera.main;
    }

    void Update()
    {
        CalculateDistance();
    }

    private void CalculateDistance()
    {
        float distanceFromNavigationTarget = Vector3.Distance(_fromGameObject.transform.position, _firstPersonCamera.transform.position);
        float distanceFromArrow = Vector3.Distance(this.transform.position, _firstPersonCamera.transform.position);

        int _distanceFromNavigationTarget = (int)(distanceFromNavigationTarget * 3.37f);
        int _distanceFromArrow = (int)(distanceFromArrow * 3.37f);
        if (_distanceFromArrow > 10) {
            updateArrow(1.0f);
        } 
        if (_distanceFromArrow < 10 && _distanceFromArrow > 6) {
            updateArrow(0.7f);
        }
        if (_distanceFromArrow < 6 && _distanceFromArrow > 3)
        {
            updateArrow(0.4f);
        }
        if (_distanceFromArrow < 3 && _distanceFromArrow > 0)
        {
            updateArrow(0.2f);
        }
        if (_distanceFromArrow == 0)
        {
            updateArrow(0.0f);
        }
        updateText(_distanceFromNavigationTarget);
    }

    private void updateText(int dis){
        _distanceText.text = $"{dis}ft";
    }

    private void updateArrow(float f){
        Component[] renderers = this.GetComponentsInChildren(typeof(Renderer));
        foreach (Renderer curRenderer in renderers)
        {
            Color color = new Color(1.0f, 1.0f, 1.0f, f);
            foreach (Material material in curRenderer.materials)
            {
                material.color = color;
            }
        }
        // this.GetComponent<MeshRenderer>().material.color = new Color(f, f, f, f);
    }
}
