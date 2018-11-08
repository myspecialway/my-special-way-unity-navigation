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
        public                   TextMeshPro NodeIdText;
        public                   TextMeshPro NodePositionText;

        private Camera _mainCamera;

        protected virtual void Awake()
        {
            _mainCamera = Camera.main;
        }

        protected virtual void Start()
        {
        }

        protected virtual void Update()
        {
            // tracking is not stable
            if (Session.Status != SessionStatus.Tracking)
            {
                return;
            }

            var wayPoint = _constheightWayPoint.transform.position;

            wayPoint.y                              = Frame.Pose.position.y - 1.0f;
            _constheightWayPoint.transform.position = wayPoint;

//            transform.LookAt(_mainCamera.transform);
//            NodeIdText.transform.SetPositionAndRotation(NodeIdText.transform.position, _mainCamera.transform.rotation);
//            NodePositionText.transform.SetPositionAndRotation(NodePositionText.transform.position, _mainCamera.transform.rotation);
        }
    }
}