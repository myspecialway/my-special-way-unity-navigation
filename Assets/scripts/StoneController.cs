using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Msw.Core.Controllers
{
    public class StoneController : MonoBehaviour
    {
        /// <summary>
        /// The first-person camera being used to render the passthrough camera image (i.e. AR background).
        /// </summary>
        [SerializeField] private Camera _firstPersonCamera;
        /// <summary>
        /// A list to hold all planes ARCore is tracking in the current frame. This object is used across
        /// the application to avoid per-frame allocations.
        /// </summary>
        private readonly List<DetectedPlane> _allDetectedPlanes = new List<DetectedPlane>();
        /// <summary>
        /// A prefab for visualizing an environment.
        /// </summary>
        ///
        ///         /// <summary>
        /// A textholder for navigation instructions (distance).
        /// </summary>
        ///    /// <summary>
        /// The overlay containing the footer managing the navigation.
        /// </summary>
        [SerializeField] private TextMeshProUGUI _distanceText;
        [SerializeField] private GameObject _environmentVisualizerPrefab;
        [SerializeField] private GameObject _navigationMenu;
        [SerializeField] private GameObject _guidingLineYellow;
        [SerializeField] private GameObject _guidingLineGreen;
        [SerializeField] private int _delayBeforeNavigationCompleted;

        private bool _positioned;
        private bool _completedNavigation;
        private bool _tracking;
        private bool _firstUpdate = true;
        private bool _goingToAdidas;
        private bool _goingToNike = true;
        private string _destinationName;
        private GameObject _destination;
        private GameObject _environmentVisualizer;

        private void Init()
        {
            _guidingLineYellow.SetActive(true);
            _firstUpdate = false;
        }
        
        protected void Update()
        {
            if (_firstUpdate)
            {
                Init();
            }
            
            Session.GetTrackables(_allDetectedPlanes);

            foreach (var plane in _allDetectedPlanes)
            {
                if (plane.TrackingState == TrackingState.Tracking)
                {
                    _tracking = true;
                    break;
                }
            }

            if (_tracking)
            {
                if (_guidingLineYellow.activeSelf)
                {
                    _guidingLineYellow.SetActive(false);
                    _guidingLineGreen.SetActive(true);
                }
            }
            
            if (!_positioned)
            {
                // if user did not touched the display, finish the update
                if (Input.touchCount < 1 || Input.GetTouch(0).phase != TouchPhase.Began)
                {
                    return;
                }

                if (_tracking)
                {
                    DeployAugmentation();
                    RemoveGuidingLine();
                }
            }
            else
            {
                // if user did not touched the display, finish the update
                if (Input.touchCount < 1 || Input.GetTouch(0).phase != TouchPhase.Began)
                {
                    CalculateDistance();
                    return;
                }

                if (_navigationMenu.activeSelf)
                {
                    _navigationMenu.SetActive(false);
                    if (_goingToNike)
                    {
                        SetPath1Visibility(true);
                        SetPath2Visibility(false);
                    }

                    if (_goingToAdidas)
                    {
                        SetPath1Visibility(false);
                        SetPath2Visibility(true);
                        StartCoroutine(CompletedNavigationToAdidas());
                    }
                }
                else
                {
                    if (!_completedNavigation)
                    {
                        _navigationMenu.SetActive(true);
                    }
                    else
                    {
                        SceneManager.LoadScene(2);
                    }
                }
            }
        }

        IEnumerator CompletedNavigationToAdidas()
        {
            yield return new WaitForSeconds(_delayBeforeNavigationCompleted);
            _completedNavigation = true;
            _goingToAdidas = false;
            _goingToNike = false;
        }

        private void CalculateDistance()
        {
            if (_firstPersonCamera == null || _firstPersonCamera.transform == null)
            {
                _distanceText.text = "no camera";
                return;
            }

            if (_distanceText == null)
            {
                _distanceText.text = "Select destination";
                return;
            }

            if (_destination != null && _firstPersonCamera != null)
            {
                float dist = Vector3.Distance(_destination.transform.position, _firstPersonCamera.transform.position);
                int distance = (int)(dist * 3.37f);
                _distanceText.text = $"{_destinationName} Store - {distance}ft";

                if (dist < 4)
                {
                    SetPath1Visibility(false);
                    SetPath2Visibility(false);
                    _distanceText.text = "Click to Navigate";

                    if (_goingToAdidas)
                    {
                        _goingToAdidas = false;
                        _destination = null;
                    }

                    if (_goingToNike)
                    {
                        _goingToNike = false;
                        _goingToAdidas = true;
                        _destination = null;
                    }
                }
            }
        }

        private void RemoveGuidingLine()
        {
            _guidingLineYellow.SetActive(false);
            _guidingLineGreen.SetActive(false);
        }

        private void PositionAugmentation()
        {
            var poseStart = new Vector3(0, 0, 0);
            var rotation = Frame.Pose.rotation;
            var rot = rotation.eulerAngles;
            rot.z = 0f;
            rot.x = 0f;
            var rot1 = Quaternion.Euler(rot);
            _environmentVisualizer =
                Instantiate(_environmentVisualizerPrefab, poseStart, rot1);
           SetPath1Visibility(false);
           SetPath2Visibility(false);
            _positioned = true;
        }

        public void DeployAugmentation()
        {
            if (_environmentVisualizer != null)
            {
                Destroy(_environmentVisualizer);
                _environmentVisualizer = null;
                _positioned = false;
            }

            PositionAugmentation();
        }
        
        public void SetPath1Visibility(bool visible){
            Transform navigation = _environmentVisualizer.transform.Find("Navigation");
            if (navigation != null)
            {
                Transform path = navigation.Find("Navigate from start to D");

                path.gameObject.SetActive(visible);
                if (visible)
                {
                    _destinationName = "Nike";
                    Transform hall = _environmentVisualizer.transform.Find("Entrance hall");
                    if (hall != null)
                    {
                        _destination = hall.Find("NikeStore").gameObject;
                    }
                }
            }
        }
        
        public void SetPath2Visibility(bool visible)
        {
            Transform navigation = _environmentVisualizer.transform.Find("Navigation");
            if (navigation != null)
            {
                Transform path = navigation.Find("Navigate from D to B");

                path.gameObject.SetActive(visible);
                if (visible)
                {
                    _destinationName = "Adidas";

                    Transform hall = _environmentVisualizer.transform.Find("Entrance hall");
                    if (hall != null)
                    {
                        _destination = hall.Find("AdidasStore").gameObject;
                    }
                }
            }
        }
    }
}