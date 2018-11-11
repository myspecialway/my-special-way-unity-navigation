using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using UnityEngine;

namespace Msw.Core.Controllers
{
    public class FloorMapController : MonoBehaviour
    {
        [SerializeField] private GameObject _mapPrefab;

        private GameObject map = null;

        private Vector3 _previousPos;
        private Vector3 _currentPos;

        private void Update()
        {
            // tracking is not stable
            if (Session.Status != SessionStatus.Tracking)
            {
                return;
            }

            if (map == null)
            {
                return;
            }

            var mapPosition = map.transform.position;

            mapPosition.y = CameraTrackerDrawController.FloorPosition.y;
            map.transform.position = mapPosition;
        }

        public void ShowFloorMap()
        {
            if (map != null)
            {
                map.SetActive(!map.activeSelf);
                return;
            }
            
            // tracking is not stable
            if (Session.Status != SessionStatus.Tracking)
            {
                return;
            }

            var pos = Frame.Pose.position;
            var rot = Frame.Pose.rotation;

            var eulerRot = rot.eulerAngles;
            eulerRot.x = 0;
            eulerRot.z = 0;
            rot = Quaternion.Euler(eulerRot);

            map = Instantiate(_mapPrefab, new Vector3(pos.x, CameraTrackerDrawController.FloorPosition.y, pos.z), rot);
        }
    }
}