using System.Collections.Generic;
using GoogleARCore;
using TMPro;
using UnityEngine;

namespace Msw.Core.Controllers
{
    public class StoneController : MonoBehaviour
    {
        [SerializeField] private Camera _firstPersonCamera;

        [SerializeField] private TextMeshProUGUI _distanceText;
        [SerializeField] private GameObject _environmentVisualizerPrefab;
        [SerializeField] private GameObject _navigationMenu;
        [SerializeField] private GameObject _tapToPlaceModel;
        [SerializeField] private GameObject _startUpScan;
        [SerializeField] private GameObject _afterScanApp;
        [SerializeField] private GameObject _duringAr;
        [SerializeField] private GameObject _coupons;

        private bool _positioned;
        private bool _tracking;
        private bool _goingToAdidas;
        private bool _goingToNike = true;
        private string _destinationName;
        private GameObject _destination;
        private GameObject _environmentVisualizer;
        private Vector3 _floorPosition = Vector3.zero;
       
        protected void Update()
        {
            _floorPosition = TryFindFloorPlanePosition();

            if (_environmentVisualizerPrefab != null)
            {
                var pos = _environmentVisualizerPrefab.transform.position;
                pos.y = _floorPosition.y;
                _environmentVisualizerPrefab.transform.position = pos;
            }

            if (_floorPosition != Vector3.zero)
            {
                _tracking = true;
            }
            else
            {
                return;
            }

            if (_tracking && !_positioned)
            {
                if (!_startUpScan.activeSelf || !_tapToPlaceModel.activeSelf)
                {
                    _startUpScan.SetActive(true); 
                    _tapToPlaceModel.SetActive(true);
                }
            }

            if (_environmentVisualizerPrefab != null)
            {
                CalculateDistance();
            }

            Vector3 touchPosition = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(touchPosition);
            var h = 108;
            var w = 216;
            if (Input.mousePosition.x > w && Input.mousePosition.x < 1080 - w && Input.mousePosition.y > h && Input.mousePosition.y < 2160 - h)
            {
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.tag.Equals("Coupon"))
                    {
                        _coupons.SetActive(true);
                        _duringAr.SetActive(false);
                    }
                }
            }
        }

        public void MoveToDemo()
        {
            _afterScanApp.SetActive(false);
            _duringAr.SetActive(true);
            _navigationMenu.SetActive(true);
        }
        
        public void TapToScan()
        {
            _startUpScan.SetActive(false);
            _afterScanApp.SetActive(true);
            DeployAugmentation();
        }
        
        public void CancelNavigationMenu()
        {
            SetPath1Visibility(false);
            SetPath2Visibility(false);
            _navigationMenu.SetActive(false);
            _destination = null;
        }
        
        public void ToggleNavigationMenu()
        {
            SetPath1Visibility(false);
            SetPath2Visibility(false);
            if (_navigationMenu.activeSelf)
            {
                _navigationMenu.SetActive(false);
            }
            else
            {
                _navigationMenu.SetActive(true);
            }
        }

        public void CloseCoupons()
        {
            _coupons.SetActive(false);
            _duringAr.SetActive(true);
        }

        public void BackToMainScreen()
        {
            _duringAr.SetActive(false);
            _afterScanApp.SetActive(true);
        }
        
        public void NavigateToNike()
        {
            SetPath1Visibility(true);
            SetPath2Visibility(false);
            _navigationMenu.SetActive(false);
        }
        
        public void NavigateToAdidas()
        {
            SetPath1Visibility(false);
            SetPath2Visibility(true);
            _navigationMenu.SetActive(false);
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
                        _goingToNike = false;
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

        private void PositionAugmentation()
        {
            var poseStart = Frame.Pose.position;
            var rotation = Frame.Pose.rotation;
            var rot = rotation.eulerAngles;
            rot.z = 0f;
            rot.x = 0f;
            poseStart.y = _floorPosition.y;
            
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
        
                
        private Vector3 TryFindFloorPlanePosition()
        {
            List<DetectedPlane> allPlanes = new List<DetectedPlane>();

            Session.GetTrackables(allPlanes);

            DetectedPlane floorDetectedPlane = null;

            var minY = Mathf.Infinity;

            for (int i = 0; i < allPlanes.Count; i++)
            {
                var detectedPlane = allPlanes[i];
                if (detectedPlane.TrackingState == TrackingState.Tracking &&
                    detectedPlane.PlaneType     == DetectedPlaneType.HorizontalUpwardFacing)
                {
                    var detectedPlaneY = detectedPlane.CenterPose.position.y;

                    if (detectedPlaneY <= minY)
                    {
                        minY = detectedPlaneY;
                        floorDetectedPlane  = detectedPlane;
                    }
                }
            }

            if (floorDetectedPlane != null)
            {
                _tracking = true;
                return floorDetectedPlane.CenterPose.position;
            }

            return Vector3.zero;
        }

    }
}