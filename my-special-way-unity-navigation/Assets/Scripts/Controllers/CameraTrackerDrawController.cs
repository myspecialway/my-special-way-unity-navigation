#region Usings

using System.Collections.Generic;
using GoogleARCore;
using Msw.Core.Controllers.Jobs;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

#endregion

namespace Msw.Core.Controllers
{
#if UNITY_EDITOR
    // Set up touch input propagation while using Instant Preview in the editor.
    using Input = InstantPreviewInput;

#endif

    public class CameraTrackerDrawController : MonoBehaviour
    {
        /// <summary>
        /// The first-person camera being used to render the passthrough camera image (i.e. AR background).
        /// </summary>
        [SerializeField] private Camera _firstPersonCamera;

        [SerializeField] private Transform _cameraTrajectoryRoot;
        
        [SerializeField] private GameObject _cameraTrajectoryNodePrefab;

        [SerializeField] private GameObject _arrowPrefab;

        [SerializeField] private float _arrowVerticalOffset;
        
        [SerializeField] private float _trajectoryNodeVerticalOffset;


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
        private TransformAccessArray _arrowTransforms;
        private TransformAccessArray _trajectoryTransforms;
        
        private SnapNodeToFloorJob _snapArrowsToFloorJob;
        private SnapNodeToFloorJob _snapTrajectoriesToFloorJob;
        
        private JobHandle _snapArrowsToFloorJobHandle;
        private JobHandle _snapTrajectoriesToFloorJobHandle;

        private bool _autoGenerateMap = true;

        public void ToggleAutoGenerate()
        {
            _autoGenerateMap = !_autoGenerateMap;
        }

        protected virtual void OnDisable()
        {
            _snapArrowsToFloorJobHandle.Complete();
            _snapTrajectoriesToFloorJobHandle.Complete();
            _arrowTransforms.Dispose();
            _trajectoryTransforms.Dispose();
        }
        // Jobs

        protected virtual void Start()
        {
            _arrowTransforms = new TransformAccessArray(0, -1);
            _trajectoryTransforms = new TransformAccessArray(0, -1);
        }

        protected virtual void Update()
        {
            // Jobs
            _snapArrowsToFloorJobHandle.Complete();
            _snapTrajectoriesToFloorJobHandle.Complete();
            // Jobs

            _UpdateApplicationLifecycle();

            FloorPosition = TryFindFloorPlanePosition();
//            FloorPosition = TryFindLowestFeaturePointPosition();
//            CreateNodeOnAnyTouch();

            if (_autoGenerateMap)
            {
                AutoGenerateRoute();
            }

            // Jobs

            _snapArrowsToFloorJob = new SnapNodeToFloorJob()
            {
                FloorHeight = FloorPosition.y,
                VecticalOffset = _arrowVerticalOffset
            };
            
            _snapTrajectoriesToFloorJob = new SnapNodeToFloorJob()
            {
                FloorHeight = FloorPosition.y,
                VecticalOffset = _trajectoryNodeVerticalOffset
            };

            _snapArrowsToFloorJobHandle = _snapArrowsToFloorJob.Schedule(_arrowTransforms);
            _snapTrajectoriesToFloorJobHandle = _snapTrajectoriesToFloorJob.Schedule(_trajectoryTransforms);
            JobHandle.ScheduleBatchedJobs();

            // Jobs
        }

        private List<Vector3> _nodes;
        private Vector3 _lastNode = Vector3.zero;

        private void AutoGenerateRoute()
        {
            // Jobs
            _snapArrowsToFloorJobHandle.Complete();
            _snapTrajectoriesToFloorJobHandle.Complete();
            // Jobs

            var currentNode = Frame.Pose.position;

            if (Vector3.Distance(currentNode, _lastNode) >= _distanceBetweenNodes)
            {
                _lastNode = currentNode;
                CreateCameraTrajectoryNode();
            }
        }

        private void CreateNodeOnAnyTouch() //(Delegate @delegate)
        {
            // If the player has not touched the screen, we are done with this update.
            Touch touch;
            if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
            {
                return;
            }

            CreateCameraTrajectoryNode();
        }

        private static Vector3 TryFindFloorPlanePosition()
        {
            List<DetectedPlane> allPlanes = new List<DetectedPlane>();

            // Hide snackbar when currently tracking at least one plane.
            Session.GetTrackables<DetectedPlane>(allPlanes);

            DetectedPlane floorDetectedPlane = null;

            var pos = Frame.Pose.position;
            var currentDeviceY = pos.y;

            var minY = Mathf.Infinity;

//            bool showSearchingUI = true;
            for (int i = 0; i < allPlanes.Count; i++)
            {
                var detectedPlane = allPlanes[i];
                if (detectedPlane.TrackingState == TrackingState.Tracking &&
                    detectedPlane.PlaneType == DetectedPlaneType.HorizontalUpwardFacing)
                {
                    var detectedPlaneY = detectedPlane.CenterPose.position.y;

                    if (detectedPlaneY <= minY)
                    {
                        minY = detectedPlaneY;
                        floorDetectedPlane = detectedPlane;
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
            var camPos = _firstPersonCamera.transform.position;
            var camRot = _firstPersonCamera.transform.rotation;
            var camRotEulerAngles = camRot.eulerAngles;
            camRotEulerAngles.x = 0;
            camRotEulerAngles.z = 0;
            camRot = Quaternion.Euler(camRotEulerAngles);
    
            var trajectoryNode = Instantiate(_cameraTrajectoryNodePrefab, camPos, camRot);
            trajectoryNode.transform.parent = _cameraTrajectoryRoot;

            var arrowPos = new Vector3(camPos.x, FloorPosition.y, camPos.z);

            var arrowNode = Instantiate(_arrowPrefab, arrowPos, camRot);
            arrowNode.transform.parent = _cameraTrajectoryRoot;
            arrowNode.transform.Rotate(0, k_ModelRotation, 0, Space.Self);

            // Jobs
            _trajectoryTransforms.Add(trajectoryNode.transform);
            _arrowTransforms.Add(arrowNode.transform);
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
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
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
                                message, 0);
                    toastObject.Call("show");
                }));
            }
        }
    }
}