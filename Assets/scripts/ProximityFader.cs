using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProximityFader : MonoBehaviour {

    [SerializeField] private TextMeshPro _distanceText;
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

        updateArrowTransparency(_distanceFromArrow);
        updateText(_distanceFromNavigationTarget);
    }

    private void updateArrowTransparency(int dis){
        if (dis > 10)
        {
            updateArrow(0.7f);
        }
        if (dis < 10 && dis > 6)
        {
            updateArrow(0.4f);
        }
        if (dis < 6 && dis > 3)
        {
            updateArrow(0.1f);
        }
        if (dis < 3 && dis > 0)
        {
            updateArrow(0.0f);
        }
    }

    private void updateText(int dis){
        _distanceText.text = $"{dis}ft";
    }

    private void updateArrow(float f){
        Component[] renderers = this.GetComponentsInChildren(typeof(Renderer));
        foreach (Renderer curRenderer in renderers)
        {
            Color color = new Color(1.0f, 0.0f, 0.0f, f);
            foreach (Material material in curRenderer.materials)
            {
                material.color = color;
            }
        }
    }
}
