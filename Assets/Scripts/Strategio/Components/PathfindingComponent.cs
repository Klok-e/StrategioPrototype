using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Strategio.Components
{
    [Serializable]
    public struct PathfindingComponent : IComponentData
    {
        public float2 goal;
        public byte isOrderedToMove;
    }
}