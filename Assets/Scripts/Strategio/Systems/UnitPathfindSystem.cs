using Strategio.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Strategio.Systems
{
    public class UnitPathfindSystem : JobComponentSystem
    {
        public const float Margin = 0.1f;

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var j1 = new PathfindJob
            {
            };
            var h1 = j1.Schedule(this, inputDeps);
            return h1;
        }

        [BurstCompile]
        private struct PathfindJob : IJobForEach_CC<Translation, PathfindingComponent>
        {
            public void Execute(ref Translation c0, ref PathfindingComponent c1)
            {
                if (c1.isOrderedToMove == 0) return;
                if (math.distance(c1.goal, c0.Value.xy) < Margin)
                {
                    c1.isOrderedToMove = 0;
                    return;
                }

                var dir = math.normalize(c1.goal - c0.Value.xy);
                var move = dir * 0.1f;
                c0.Value += math.float3(move, 0f);
            }
        }
    }
}