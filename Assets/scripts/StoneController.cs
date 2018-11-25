using System.Collections.Generic;
using GoogleARCore;
using TMPro;
using UnityEngine;

namespace Msw.Core.Controllers
{
    public class StoneController : MonoBehaviour
    {
        /// <summary>
        /// A list to hold all planes ARCore is tracking in the current frame. This object is used across
        /// the application to avoid per-frame allocations.
        /// </summary>
        private List<DetectedPlane> _allDetectedPlanes = new List<DetectedPlane>();

        /// <summary>
        /// A prefab for visualizing an environment.
        /// </summary>
        [SerializeField] private GameObject _environmentVisualizerPrefab;

        /// <summary>
        /// A textholder for navigation instructions (distance).
        /// </summary>
        [SerializeField] private TextMeshProUGUI _positionText;

        [SerializeField] private TextMeshProUGUI _directionText;

        [SerializeField] private TextMeshProUGUI _debugText;

        private bool positioned;

        private GameObject _environmentVisualizer;

        protected virtual void Update()
        {
            Session.GetTrackables(_allDetectedPlanes);

            bool tracking = false;
            for (var i = 0; i < _allDetectedPlanes.Count; i++)
            {
                if (_allDetectedPlanes[i].TrackingState == TrackingState.Tracking)
                {
                    tracking = true;
                    break;
                }
            }

            if (tracking)
            {
                _debugText.text = "Tracking";
                if (positioned == false)
                {
                    positionAugmentation();
                }
            }
            else
            {
                _debugText.text = "Not Tracking";
            }

            _positionText.text = Frame.Pose.position.ToString();
            _directionText.text = Frame.Pose.rotation.ToString();

            if (Input.touchCount < 1 || (Input.GetTouch(0)).phase != TouchPhase.Began)
            {
            }
            else
            {
                deployAugmentation();
            }
        }

        private void positionAugmentation()
        {
            var poseStart = new Vector3(0, 0, 0);
            var rotation = Frame.Pose.rotation;
            var rot = rotation.eulerAngles;
            rot.z = 0f;
            rot.x = 0f;
            var rot1 = Quaternion.Euler(rot);
            _environmentVisualizer =
                Instantiate(_environmentVisualizerPrefab, poseStart, rot1);
            positioned = true;
        }

        public void deployAugmentation()
        {
            if (_environmentVisualizer != null)
            {
                Destroy(_environmentVisualizer);
                _environmentVisualizer = null;
                positioned = false;
            }

            positionAugmentation();
        }
    }
}