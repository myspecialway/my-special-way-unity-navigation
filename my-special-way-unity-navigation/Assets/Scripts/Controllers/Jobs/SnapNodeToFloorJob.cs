using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

namespace Msw.Core.Controllers.Jobs
{
//    [ComputeJobOptimization]
    public struct SnapNodeToFloorJob : IJobParallelForTransform
    {
        public float FloorHeight;
        public float VecticalOffset;
        
        public void Execute(int index, TransformAccess transform)
        {
            var pos = transform.position;
            pos.y = FloorHeight + VecticalOffset;
            transform.position = pos;
        }
    }
}