using TMPro;

namespace Msw.Core.Controllers
{
    using GoogleARCore;
    using UnityEngine;
    using System.Collections.Generic;

    public class MswController : MonoBehaviour
    {
        /// <summary>
        /// The first-person camera being used to render the passthrough camera image (i.e. AR background).
        /// </summary>
        [SerializeField] private Camera _firstPersonCamera;

        /// <summary>
        /// The overlay containing the fit to scan user guide.
        /// </summary>
        [SerializeField] private GameObject _fitToScanOverlay;

        /// <summary>
        /// A list to hold all planes ARCore is tracking in the current frame. This object is used across
        /// the application to avoid per-frame allocations.
        /// </summary>
        private List<DetectedPlane> _allDetectedPlanes = new List<DetectedPlane>();

        private List<AugmentedImage> _tempAugmentedImages = new List<AugmentedImage>();

        /// <summary>
        /// A prefab for visualizing an environment.
        /// </summary>
        [SerializeField] private GameObject _environmentVisualizerPrefab;

        /// <summary>
        /// A textholder for navigation instructions (distance).
        /// </summary>
        [SerializeField] private TextMeshProUGUI _distanceText;

        private GameObject destination;

        private GameObject _environmentVisualizer = null;

        [SerializeField] private TextMeshProUGUI _sampleCounterText;

        private List<Vector3> _positionAggregator = new List<Vector3>();
        private List<Vector3> _rotationAggregator = new List<Vector3>();

        private const int RequiredSampleCount = 10;
        private int _sampleCount = 0;
        private bool _didCollectEnoughSamples = false;
        private string destinationName;

        protected virtual void Awake() { }

        protected virtual void Start() { }

        protected virtual void Update()
        {
            _sampleCounterText.text = $"{_sampleCount} samples";

            Session.GetTrackables<DetectedPlane>(_allDetectedPlanes, TrackableQueryFilter.All);

            var showSearchingUI = true;

            for (var i = 0; i < _allDetectedPlanes.Count; i++)
            {
                if (_allDetectedPlanes[i].TrackingState == TrackingState.Tracking)
                {
                    showSearchingUI = false;
                    break;
                }
            }

            if (!showSearchingUI)
            {
                Session.GetTrackables(_tempAugmentedImages, TrackableQueryFilter.Updated);

                foreach (var augmentedImage in _tempAugmentedImages)
                {
                    if (augmentedImage.TrackingState == TrackingState.Tracking &&
                        _environmentVisualizer == null)
                    {

                        if (!_didCollectEnoughSamples)
                        {
                            _positionAggregator.Add(augmentedImage.CenterPose.position);
                            _rotationAggregator.Add(augmentedImage.CenterPose.rotation.eulerAngles);

                            _sampleCount++;

                            if (_sampleCount >= RequiredSampleCount)
                            {
                                _didCollectEnoughSamples = true;
                            }

                            break;
                        }
                        else
                        {
                            _sampleCounterText.gameObject.SetActive(false);
                        }

                        var trackableHits = new List<TrackableHit>();

                        const TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon |
                                                                TrackableHitFlags.FeaturePointWithSurfaceNormal;

                        var didHitSomething = Frame.RaycastAll(_firstPersonCamera.transform.position,
                            _firstPersonCamera.transform.forward, trackableHits, Mathf.Infinity, raycastFilter);

                        if (didHitSomething)
                        {
                            var minY = Mathf.Infinity;

                            var hitToAssociateWith = new TrackableHit(); // with minimal 'y'
                            var foundSuchTrackableHit = false;

                            foreach (var trackableHit in trackableHits)
                            {
                                // Use hit pose and camera pose to check if hittest is from the
                                // back of the plane, if it is, no need to create the anchor.
                                if ((trackableHit.Trackable is DetectedPlane) &&
                                    Vector3.Dot(_firstPersonCamera.transform.position - trackableHit.Pose.position,
                                        trackableHit.Pose.rotation * Vector3.up) < 0)
                                {
                                    Debug.Log("Hit at back of the current DetectedPlane");
                                }
                                else
                                {
                                    if (trackableHit.Pose.position.y < minY)
                                    {
                                        minY = trackableHit.Pose.position.y;
                                        hitToAssociateWith = trackableHit;
                                        foundSuchTrackableHit = true;
                                    }
                                }
                            }

                            if (foundSuchTrackableHit)
                            {
                                // calculate median value for augmented image position and rotation
                                var midIndex = (int)(_positionAggregator.Count / 2);

                                var poseMedian = _positionAggregator[midIndex];
                                var rotMedian = _rotationAggregator[midIndex];

                                var rot = Quaternion.Euler(rotMedian);
                                rot.SetLookRotation(new Vector3(-90f, 0f, 0f), Vector3.up);
                                _environmentVisualizer = Instantiate(_environmentVisualizerPrefab, poseMedian, rot);
                                setPath1Visibility(false);
                                setPath2Visibility(false);
                                var initialAnchor = hitToAssociateWith.Trackable.CreateAnchor(hitToAssociateWith.Pose);
                                _environmentVisualizer.transform.parent = initialAnchor.transform;

                                _fitToScanOverlay.SetActive(false);
                            }
                        }
                    }
                }

                if (_environmentVisualizer == null)
                {
                    _fitToScanOverlay.SetActive(true);
                }
            }
            // in case of lost tracking
            else
            {
                _fitToScanOverlay.SetActive(true);

                if (_environmentVisualizer != null)
                {
                    Destroy(_environmentVisualizer);
                    _environmentVisualizer = null;
                }
            }

            CalculateDistance();
            // If the player has not touched the screen, we are done with this update.

            if (Input.touchCount < 1 || (Input.GetTouch(0)).phase != TouchPhase.Began)
            {
                return;
            }

            // Raycast against the location the user touched to search for objects.
            RaycastHit hit;
            Ray ray = _firstPersonCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                Transform objectHit = hit.transform;
                if (objectHit.gameObject.name.Contains("close shop"))
                {
                    setPath1Visibility(true);
                }
                if (objectHit.gameObject.name.Contains("Far Destination Shop"))
                {
                    setPath1Visibility(false);
                    setPath2Visibility(true);
                }
            }
        }

        private void setPath1Visibility(bool visible){
            Transform navigation = _environmentVisualizer.transform.Find("Navigation");
            if (navigation != null)
            {
                Transform path = navigation.Find("Navigate from start to D");

                path.gameObject.SetActive(visible);
                destinationName = "Nike";
                Transform hall = _environmentVisualizer.transform.Find("Entrance hall");
                if (hall != null)
                {
                    destination = hall.Find("NikeStore").gameObject;
                }
            }
        }

        private void setPath2Visibility(bool visible)
        {
            Transform navigation = _environmentVisualizer.transform.Find("Navigation");
            if (navigation != null)
            {
                Transform path = navigation.Find("Navigate from D to B");

                path.gameObject.SetActive(visible);
                destinationName = "Adidas";

                Transform hall = _environmentVisualizer.transform.Find("Entrance hall");
                if (hall != null)
                {
                    destination = hall.Find("AdidasStore").gameObject;
                }
            }
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

            if (destination != null && _firstPersonCamera != null)
            {
                float dist = Vector3.Distance(destination.transform.position, _firstPersonCamera.transform.position);
                int _distance = (int)(dist * 3.37f);
                _distanceText.text = $"{destinationName} Store - {_distance}ft";
            }
        }
    }
}