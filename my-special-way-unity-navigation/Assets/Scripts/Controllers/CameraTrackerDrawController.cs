using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using GoogleARCore;
using GoogleARCoreInternal;
using Msw.Core.Controllers.Jobs;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

namespace Msw.Core.Controllers
{
    public class CameraTrackerDrawController : MonoBehaviour
    {
        /// <summary>
        /// The first-person camera being used to render the passthrough camera image (i.e. AR background).
        /// </summary>
        [SerializeField] private Camera _firstPersonCamera;

        /// <summary>
        /// A model to place when a raycast from a user touch hits a plane.
        /// </summary>
        [SerializeField] private GameObject _cameraTrajectoryNodePrefab;

        [SerializeField] private GameObject _arrowPrefab;

        [SerializeField] private Transform _cameraTrajectoryRoot;

        [SerializeField] private float _distanceBetweenNodes;

        /// <summary>
        /// The rotation in degrees need to apply to model when the arrow model is placed.
        /// </summary>
        private const float k_ModelRotation = 180.0f;

        /// <summary>
        /// True if the app is in the process of quitting due to an ARCore connection error, otherwise false.
        /// </summary>
        private bool _isQuitting = false;

        protected virtual void Awake()
        {
            //
        }


        public static int NodeCounter = 0;

        public static Vector3 FloorPosition = Vector3.zero;
        
        // Jobs
        private TransformAccessArray _transforms;
        private SnapNodeToFloorJob _snapNodesToFloorJob;
        private JobHandle _snapNodesToFloorJobHandle;

        protected virtual void OnDisable()
        {
            _snapNodesToFloorJobHandle.Complete();
            _transforms.Dispose();
        }
        
        // Jobs
        
        protected virtual void Start()
        {
            //InvokeRepeating(nameof(CreateCameraTrajectoryNode), 1.0f, 1.0f);
            
            _transforms = new TransformAccessArray(0, -1);
        }
        
        protected virtual void Update()
        {
            // Jobs
            
            _snapNodesToFloorJobHandle.Complete();
            
            
            // Jobs
            
            
            _UpdateApplicationLifecycle();

            FloorPosition = TryFindFloorPlanePosition();
//            FloorPosition = TryFindLowestFeaturePointPosition();
//            CreateNodeOnAnyTouch();

            AutoGenerateRoute();
            
            // Jobs

            _snapNodesToFloorJob = new SnapNodeToFloorJob()
            {
                FloorHeight = FloorPosition.y,
                VecticalOffset = 0.01f
            };

            _snapNodesToFloorJobHandle = _snapNodesToFloorJob.Schedule(_transforms);
            JobHandle.ScheduleBatchedJobs();

            // Jobs
        }

        private List<Vector3> _nodes;
        private Vector3 _lastNode = Vector3.zero;

        private void AutoGenerateRoute()
        {
            // Jobs
            _snapNodesToFloorJobHandle.Complete();
            // Jobs
            
            var currentNode = Frame.Pose.position;

//            if (_lastNode == Vector3.zero)
//            {
//                _lastNode = currentNode;
//                return;
//            }

            if (Vector3.Distance(currentNode, _lastNode) >= _distanceBetweenNodes)
            {
                _lastNode = currentNode;
                CreateCameraTrajectoryNode();
            }
        }

        private void CreateNodeOnAnyTouch()//(Delegate @delegate)
        {
            // If the player has not touched the screen, we are done with this update.
            Touch touch;
            if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
            {
                return;
            }

            CreateCameraTrajectoryNode();
        }

//        private static Vector3 TryFindLowestFeaturePointPosition()
//        {
//            var featurePoints = new List<FeaturePoint>();
//            
//            Session.GetTrackables<FeaturePoint>(featurePoints, TrackableQueryFilter.All);
//
//            FeaturePoint lowestFeaturePoint = null;
//
//            var pos            = Frame.Pose.position;
//            var currentDeviceY = pos.y;
//
//            var minY = Mathf.Infinity;
//
//            for (int i = 0; i < featurePoints.Count; i++)
//            {
//                var featurePoint = featurePoints[i];
//                if (featurePoint.TrackingState == TrackingState.Tracking/* &&
//                    featurePoint.OrientationMode == FeaturePointOrientationMode.SurfaceNormal*/)
//                {
//                    var detectedFeaturePointY = featurePoint.Pose.position.y;
//
//                    if (detectedFeaturePointY <= minY)
//                    {
//                        minY = detectedFeaturePointY;
//                        lowestFeaturePoint  = featurePoint;
//                    }
//                }
//            }
//
//            if (lowestFeaturePoint != null)
//            {
//                return lowestFeaturePoint.Pose.position;
//            }
//
//            return Vector3.zero;
//        }

        private static Vector3 TryFindFloorPlanePosition()
        {
            List<DetectedPlane> allPlanes = new List<DetectedPlane>();

            // Hide snackbar when currently tracking at least one plane.
            Session.GetTrackables<DetectedPlane>(allPlanes);

            DetectedPlane floorDetectedPlane = null;

            var pos            = Frame.Pose.position;
            var currentDeviceY = pos.y;

            var minY = Mathf.Infinity;

//            bool showSearchingUI = true;
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

//                    showSearchingUI = false;
//                    break;
                }
            }

            if (floorDetectedPlane != null)
            {
                return floorDetectedPlane.CenterPose.position;
            }

            return Vector3.zero;
        }


        private void CreateCameraTrajectoryNode()
        {
//            var arCorePose = Frame.Pose;
//
//            var arCorePos = arCorePose.position;
//            var arCoreRot = arCorePose.rotation;
//            
//            
//
//            return;

            var camPos            = _firstPersonCamera.transform.position;
            var camRot            = _firstPersonCamera.transform.rotation;
            var camRotEulerAngles = camRot.eulerAngles;
            camRotEulerAngles.x = 0;
            camRotEulerAngles.z = 0;
            camRot              = Quaternion.Euler(camRotEulerAngles);

            // camera node
            //var camNodePos = camPos + _firstPersonCamera.transform.forward; // TODO think : const height
            var camNodePos = camPos; // + _firstPersonCamera.transform.forward; // TODO think : const height
//            camNodePos.y = .0f;      // TODO think : const height
            var trajectoryNode = Instantiate(_cameraTrajectoryNodePrefab, camNodePos, camRot);

            trajectoryNode.transform.parent = _cameraTrajectoryRoot;
            var trajectoryNodeController = trajectoryNode.GetComponent<TrajectoryNodeController>();
            trajectoryNodeController.NodeIdText.text = "Node " + NodeCounter++;
            var pos = trajectoryNode.transform.position;
            trajectoryNodeController.NodePositionText.text = $"x {pos.x}\ny {pos.y}\nz {pos.z}";

            // arrow node
//            var offsetFromCamera = new Vector3(.0f, -1.4f, .0f); // TODO think : const height

//            var arrowPos = new Vector3(camPos.x, camPos.y, camPos.z); // TODO think : const height
            var arrowPos = new Vector3(camPos.x, FloorPosition.y, camPos.z); // TODO think : floor Y
//            arrowPos += offsetFromCamera;                          // as an average height // TODO think : const height
//            arrowPos += _firstPersonCamera.transform.forward;      // to put it in front // TODO think : const height

            var arrowNode = Instantiate(_arrowPrefab, arrowPos, camRot);
            arrowNode.transform.parent = _cameraTrajectoryRoot;
            arrowNode.transform.Rotate(0, k_ModelRotation, 0, Space.Self);
            
            // Jobs
            _transforms.Add(trajectoryNode.transform);
            _transforms.Add(arrowNode.transform);
            // Jobs
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