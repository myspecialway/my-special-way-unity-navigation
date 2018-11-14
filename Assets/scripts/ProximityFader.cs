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

        updateArrowVisibility(_distanceFromArrow);
        updateText(_distanceFromNavigationTarget);
    }

    private void updateArrowVisibility(int dis){

        if (dis < 6)
        {
            hideArrow();
        }
    }

    private void updateText(int dis){
        _distanceText.text = $"{dis}ft";
    }

    private void hideArrow(){
        var objects = this.GetComponentsInChildren(typeof(GameObject));
        foreach (Component curGameObject in objects)
        {
            curGameObject.gameObject.SetActive(false);
        }
    }
}
