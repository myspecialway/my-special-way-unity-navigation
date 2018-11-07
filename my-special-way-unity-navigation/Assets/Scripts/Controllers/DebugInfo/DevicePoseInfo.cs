#region Usings

using GoogleARCore;
using TMPro;
using UnityEngine;

#endregion

namespace Msw.Core.Controllers.DebugInfo
{
    public class DevicePoseInfo : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _debugInfo;

        protected virtual void Update()
        {
            var pose = Frame.Pose;

            var pos = pose.position;
            var rot = pose.rotation;

            _debugInfo.text = $"Pos x {pos.x} y {pos.y} z {pos.z}\n" +
                              $"Rot x {rot.x} y {rot.y} z {rot.z}\n";

//            _debugInfo.text = $"Pos x {pos.x} y {pos.y} z {pos.z}\n" +
//                              $"Rot x {rot.x} y {rot.y} z {rot.z}\n" +
//                              $"Height {pos.y}";
        }
    }
}