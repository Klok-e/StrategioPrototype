using System;
using Unity.Entities;

namespace Strategio.Components.Physics
{
    [Serializable]
    public struct CircleColliderComponent : IComponentData
    {
        public float radius;
    }
}