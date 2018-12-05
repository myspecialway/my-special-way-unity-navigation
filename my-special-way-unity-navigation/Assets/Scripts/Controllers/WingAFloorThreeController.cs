#region Usings

using System.Collections.Generic;
using GoogleARCore;
using UnityEngine;

#endregion

namespace Msw.Core.Controllers
{
#if UNITY_EDITOR
    // Set up touch input propagation while using Instant Preview in the editor.
    using Input = InstantPreviewInput;

#endif

    public class WingAFloorThreeController : MonoBehaviour
    {
        [SerializeField] private GameObject _environmentPrefab;
        [SerializeField] private Transform _puppyImageTransform;

        [SerializeField] private bool _manualEnvironmentInitialization;

        private GameObject _environment;
        
        private List<AugmentedImage> _tempAugmentedImages = new List<AugmentedImage>();

        /// <summary>
        /// True if the app is in the process of quitting due to an ARCore connection error, otherwise false.
        /// </summary>
        private bool _isQuitting = false;

        public static Vector3 FloorPosition = Vector3.zero;

        protected virtual void Start()
        {
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            _UpdateApplicationLifecycle();

            // no reason to continue
            if (Session.Status != SessionStatus.Tracking)
            {
                return;
            }

            FloorPosition = TryFindFloorPlanePosition();

            // snap to floor
            if (_environment != null)
            {
                var pos = _environment.transform.position;
                pos.y = FloorPosition.y;
                _environment.transform.position = pos;
            }

            if (_manualEnvironmentInitialization)
            {
                // if the player has not touched the screen, we are done with this update.
                Touch touch;
                if (Input.touchCount > 0 &&
                    (touch = Input.GetTouch(0)).phase == TouchPhase.Began)
                {
                    var pos = Frame.Pose.position;
                    pos.y = FloorPosition.y;
                
                    var rot = Frame.Pose.rotation;
                    rot.x = .0f;
                    rot.z = .0f;
                
                    if (_environment == null)
                    {
                        _environment = Instantiate(_environmentPrefab);
                    }

                    _environment.transform.SetPositionAndRotation(pos, rot);
                }
            }
            else
            {
                // Get updated augmented images for this frame.
                Session.GetTrackables<AugmentedImage>(_tempAugmentedImages, TrackableQueryFilter.Updated);

                foreach (var augmentedImage in _tempAugmentedImages)
                {
                    if (augmentedImage.Name == "Puppy")
                    {
                        if (_environment == null)
                        {
                            _environment = Instantiate(_environmentPrefab);
                        }

                        var pose = Frame.Pose;
                        
                        var pos = pose.position - _puppyImageTransform.position;
                        pos.y = FloorPosition.y;
                        
                        var rot = pose.rotation * _puppyImageTransform.rotation;
                        
                        _environment.transform.SetPositionAndRotation(pos, rot);
                    }
                }
            }
        }

        private static Vector3 TryFindFloorPlanePosition()
        {
            List<DetectedPlane> allPlanes = new List<DetectedPlane>();

            // Hide snackbar when currently tracking at least one plane.
            Session.GetTrackables<DetectedPlane>(allPlanes);

            DetectedPlane floorDetectedPlane = null;

            var minY = Mathf.Infinity;

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
                }
            }

            if (floorDetectedPlane != null)
            {
                return floorDetectedPlane.CenterPose.position;
            }

            return Vector3.zero;
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