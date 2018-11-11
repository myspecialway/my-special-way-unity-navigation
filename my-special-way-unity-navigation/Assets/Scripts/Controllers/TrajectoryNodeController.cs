#region Usings

using GoogleARCore;
using TMPro;
using UnityEngine;

#endregion

namespace Msw.Core.Controllers
{
    public class TrajectoryNodeController : MonoBehaviour
    {
        [SerializeField] private GameObject  _constheightWayPoint;
        [SerializeField] private GameObject  _cameraHeightWayPoint;
        [SerializeField] private float       _verticalOffset;
        public                   TextMeshPro NodeIdText;
        public                   TextMeshPro NodePositionText;

        private Camera _mainCamera;

        protected virtual void Awake()
        {
            _mainCamera = Camera.main;
        }

        protected virtual void Start()
        {
            if (_cameraHeightWayPoint != null)
            {
                _cameraHeightWayPoint.SetActive(false);
            }

            if (NodeIdText != null)
            {
                NodeIdText.gameObject.SetActive(false);
            }

            if (NodePositionText != null)
            {
                NodePositionText.gameObject.SetActive(false);
            }
        }

        protected virtual void Update()
        {
            // tracking is not stable
            if (Session.Status != SessionStatus.Tracking)
            {
                return;
            }

            var wayPoint = _constheightWayPoint.transform.position;

            wayPoint.y =
                CameraTrackerDrawController.FloorPosition.y + _verticalOffset;
            _constheightWayPoint.transform.position = wayPoint;

//            transform.LookAt(_mainCamera.transform);
//            NodeIdText.transform.SetPositionAndRotation(NodeIdText.transform.position, _mainCamera.transform.rotation);
//            NodePositionText.transform.SetPositionAndRotation(NodePositionText.transform.position, _mainCamera.transform.rotation);
        }
    }
}