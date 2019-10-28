using Unity.Entities;
using Unity.Mathematics;

namespace Strategio.Components.Physics
{
    public struct CollisionComponent : IComponentData
    {
        public Entity ent1;
        public Entity ent2;
        public float2 ent1ToEnt2Dir;
    }
}