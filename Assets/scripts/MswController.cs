using TMPro;

namespace Msw.Core.Controllers
{
    using GoogleARCore;
    using UnityEngine;
    using System.Collections.Generic;
    using System;

#if UNITY_EDITOR
    // Set up touch input propagation while using Instant Preview in the editor.
    using Input = GoogleARCore.InstantPreviewInput;
    using UnityEngine.UI;

#endif

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
        /// A gameobject parenting UI for displaying the "searching for planes" snackbar.
        /// </summary>
        [SerializeField] private GameObject _searchingForPlaneUI;

        /// <summary>
        /// True if the app is in the process of quitting due to an ARCore connection error, otherwise false.
        /// </summary>
        private bool _isQuitting = false;

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

        private GameObject _environmentVisualizer = null;

        /// <summary>
        /// A model to place when a raycast from a user touch hits a plane.
        /// </summary>
        [SerializeField] private GameObject _vitualAnchorPlanePrefab;

        [SerializeField] private TextMeshProUGUI _sampleCounterText;

        [SerializeField] private TextMesh _distance;
        
        private List<Vector3> _positionAggregator = new List<Vector3>();
        private List<Vector3> _rotationAggregator = new List<Vector3>();

        private const int RequiredSampleCount = 10;
        private int _sampleCount = 0;
        private bool _didCollectEnoughSamples = false;

        protected virtual void Awake(){}

        protected virtual void Start(){}

        protected virtual void Update()
        {
            _UpdateApplicationLifecycle();
           
            // update sample counter text
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

            _searchingForPlaneUI.SetActive(showSearchingUI);

            if (!showSearchingUI)
            {
                // Get updated augmented images for this frame.
                Session.GetTrackables(_tempAugmentedImages, TrackableQueryFilter.Updated);

                // for the case we have already found initial position and instantiated/attached environment
                // TODO : we should handle here creation of virtual anchors on the go 
//                if (_environmentVisualizer != null)
//                {
//                    
//                }

                foreach (var augmentedImage in _tempAugmentedImages)
                {
                    if (augmentedImage.TrackingState == TrackingState.Tracking &&
                        _environmentVisualizer       == null)
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

//                      const TrackableHitFlags filter =
//                      TrackableHitFlags.PlaneWithinBounds | TrackableHitFlags.PlaneWithinPolygon;

                        // TODO think : like in HelloAR example
                        const TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon |
                                                                TrackableHitFlags.FeaturePointWithSurfaceNormal;

                        var didHitSomething = Frame.RaycastAll(_firstPersonCamera.transform.position,
                            _firstPersonCamera.transform.forward, trackableHits, Mathf.Infinity, raycastFilter);

                        if (didHitSomething)
                        {
                            var minY = Mathf.Infinity;

                            var hitToAssociateWith    = new TrackableHit(); // with minimal 'y'
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
                                        minY                  = trackableHit.Pose.position.y;
                                        hitToAssociateWith    = trackableHit;
                                        foundSuchTrackableHit = true;
                                    }
                                }
                            }

                            if (foundSuchTrackableHit)
                            {
//                                _environmentVisualizer = Instantiate(_environmentVisualizerPrefab, hitToAssociateWith.Pose.position, hitToAssociateWith.Pose.rotation);

//                                var pose = augmentedImage.CenterPose;
//                                var pos  = pose.position;
//                                var rot  = pose.rotation;
//                                
//                                _environmentVisualizer = Instantiate(_environmentVisualizerPrefab, pos, rot);

                                // calculate median value for augmented image position and rotation
                                var midIndex = (int)(_positionAggregator.Count / 2);

                                var poseMedian = _positionAggregator[midIndex];
                                var rotMedian = _rotationAggregator[midIndex];
                                
                                _environmentVisualizer = Instantiate(_environmentVisualizerPrefab, poseMedian, Quaternion.Euler(rotMedian));

                                var initialAnchor = hitToAssociateWith.Trackable.CreateAnchor(hitToAssociateWith.Pose);
                                _environmentVisualizer.transform.parent = initialAnchor.transform;

                                _fitToScanOverlay.SetActive(false);

                                InvokeRepeating(nameof(CreateVirtualAnchor), 1.0f, 1.0f);

                            }
                        }
                    }
                }

                if (_environmentVisualizer == null)
                {
                    _fitToScanOverlay.SetActive(true);
                }
            }
            // probably lost tracking at all
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
        }

        private static int count = 0;

        private void CalculateDistance()
        {
            count++;
            if (_environmentVisualizer == null)
            {
                _distance.text = $"Unknown {count}";
                return;
            }
            float dist = Vector3.Distance(_distance.transform.position, _firstPersonCamera.transform.position);
            int _dis = (int)dist;
            _distance.text = $"{_dis} m";
        }

        private void CreateVirtualAnchor()
        {
            if (_environmentVisualizer == null)
            {
                return;
            }

            var trackableHits = new List<TrackableHit>();

//                        const TrackableHitFlags filter =
//                            TrackableHitFlags.PlaneWithinBounds | TrackableHitFlags.PlaneWithinPolygon;

            // TODO think : like in HelloAR example
            const TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon |
                                                    TrackableHitFlags.FeaturePointWithSurfaceNormal;

            var didHitSomething = Frame.RaycastAll(_firstPersonCamera.transform.position,
                _firstPersonCamera.transform.forward, trackableHits, Mathf.Infinity, raycastFilter);

            if (didHitSomething)
            {
                var minY = Mathf.Infinity;

                var hitToAssociateWith    = new TrackableHit(); // with minimal 'y'
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
                            minY                  = trackableHit.Pose.position.y;
                            hitToAssociateWith    = trackableHit;
                            foundSuchTrackableHit = true;
                        }
                    }
                }

                if (foundSuchTrackableHit)
                {
                    var virtualAnchor = hitToAssociateWith.Trackable.CreateAnchor(hitToAssociateWith.Pose);
                    _environmentVisualizer.transform.parent = virtualAnchor.transform;

                    // create a model for this virtual anchor
                    var virtualAnchorModel = Instantiate(_vitualAnchorPlanePrefab, hitToAssociateWith.Pose.position,
                        hitToAssociateWith.Pose.rotation);

                    virtualAnchorModel.transform.parent = virtualAnchor.transform;
                }
            }
        }

        /// <summary>
        /// Check and update the application lifecycle.
        /// </summary>
        private void _UpdateApplicationLifecycle()
        {
            // Exit the app when the 'back' button is pressed.
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }

            // Only allow the screen to sleep when not tracking.
            if (Session.Status != SessionStatus.Tracking)
            {
                const int lostTrackingSleepTimeout = 15;
                Screen.sleepTimeout = lostTrackingSleepTimeout;
            }
            else
            {
                Screen.sleepTimeout = SleepTimeout.NeverSleep;
            }

            if (_isQuitting)
            {
                return;
            }

            // Quit if ARCore was unable to connect and give Unity some time for the toast to appear.
            if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
            {
                _ShowAndroidToastMessage("Camera permission is needed to run this application.");
                _isQuitting = true;
                Invoke("_DoQuit", 0.5f);
            }
            else if (Session.Status.IsError())
            {
                _ShowAndroidToastMessage("ARCore encountered a problem connecting.  Please start the app again.");
                _isQuitting = true;
                Invoke("_DoQuit", 0.5f);
            }
        }

        /// <summary>
        /// Actually quit the application.
        /// </summary>
        private void _DoQuit()
        {
            Application.Quit();
        }

        /// <summary>
        /// Show an Android toast message.
        /// </summary>
        /// <param name="message">Message string to show in the toast.</param>
        private void _ShowAndroidToastMessage(string message)
        {
            AndroidJavaClass  unityPlayer   = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            if (unityActivity != null)
            {
                AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
                unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                                                                            {
                                                                                AndroidJavaObject toastObject =
                                                                                    toastClass
                                                                                       .CallStatic<AndroidJavaObject>(
                                                                                            "makeText", unityActivity,
                                                                                            message,    0);
                                                                                toastObject.Call("show");
                                                                            }));
            }
        }
    }
}