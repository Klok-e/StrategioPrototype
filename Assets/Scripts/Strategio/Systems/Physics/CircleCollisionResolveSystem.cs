using Unity.Entities;
using Unity.Jobs;

namespace Strategio.Systems.Physics
{
    public class CircleCollisionResolveSystem:JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return inputDeps;
        }
    }
}