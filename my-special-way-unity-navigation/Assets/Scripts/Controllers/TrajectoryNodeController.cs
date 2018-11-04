using TMPro;
using UnityEngine;

namespace Msw.Core.Controllers
{
    public class TrajectoryNodeController : MonoBehaviour
    {
        public TextMeshPro NodeIdText;
        public TextMeshPro NodePositionText;

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
//            transform.LookAt(_mainCamera.transform);
//            NodeIdText.transform.SetPositionAndRotation(NodeIdText.transform.position, _mainCamera.transform.rotation);
//            NodePositionText.transform.SetPositionAndRotation(NodePositionText.transform.position, _mainCamera.transform.rotation);
        }
    }
}