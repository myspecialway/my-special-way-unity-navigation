using System.Net.NetworkInformation;
using GoogleARCore;
using UnityEngine;

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

        protected virtual void Start()
        {
            //InvokeRepeating(nameof(CreateCameraTrajectoryNode), 1.0f, 1.0f);
        }

        public static int NodeCounter = 0;

        protected virtual void Update()
        {
            _UpdateApplicationLifecycle();
            
            // If the player has not touched the screen, we are done with this update.
            Touch touch;
            if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
            {
                return;
            }
            
            CreateCameraTrajectoryNode();
        }

        private void CreateCameraTrajectoryNode()
        {
            var camPos = _firstPersonCamera.transform.position;
            var camRot = _firstPersonCamera.transform.rotation;
            
            // camera node
            var camNodePos = camPos + _firstPersonCamera.transform.forward;
            var trajectoryNode = Instantiate(_cameraTrajectoryNodePrefab, camNodePos, camRot);

            trajectoryNode.transform.parent = _cameraTrajectoryRoot;
            var trajectoryNodeController = trajectoryNode.GetComponent<TrajectoryNodeController>();
            trajectoryNodeController.NodeIdText.text = "Node " + NodeCounter++;
            var pos = trajectoryNode.transform.position;
            trajectoryNodeController.NodePositionText.text = $"x {pos.x}\ny {pos.y}\nz {pos.z}";

            // arrow node
            var offsetFromCamera = new Vector3(.0f, -1.4f, .0f);

            var arrowPos = new Vector3(camPos.x, camPos.y, camPos.z);
            arrowPos += offsetFromCamera; // as an average height
            arrowPos += _firstPersonCamera.transform.forward; // to put it in front
            
            var arrowNode = Instantiate(_arrowPrefab, arrowPos, camRot);
            arrowNode.transform.parent = _cameraTrajectoryRoot;
            arrowNode.transform.Rotate(0, k_ModelRotation, 0, Space.Self);
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