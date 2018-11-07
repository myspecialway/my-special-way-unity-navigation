#region Usings

using GoogleARCore;
using Msw.Core.Controllers.DebugInfo;
using UnityEngine;

#endregion

public class PointCloudDebugInfo : DebugInfoBase
{
    protected virtual void Update()
    {
        var pointCount = Frame.PointCloud.PointCount;

//        DebugInfo.color = Frame.PointCloud.IsUpdatedThisFrame ? Color.green : Color.yellow;

        DebugInfo.text = $"Frame.PointCloud.PointCount = {pointCount}";
    }
}