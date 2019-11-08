using Strategio.Components.Physics;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Strategio.Systems.Physics
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class CircleCollisionResolveSystem : JobComponentSystem
    {
        public const float RepulsionPower = 0.08f;

        private EndSimulationEntityCommandBufferSystem _barrier;
        //private EntityQuery _query;

        protected override void OnCreate()
        {
            base.OnCreate();
            _barrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            //_query = EntityManager.CreateEntityQuery(ComponentType.ReadWrite<Translation>());
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var j1 = new ResolveCollisionEventsJob
            {
                commandBuffer = _barrier.CreateCommandBuffer().ToConcurrent(),
                entityTranslations = GetComponentDataFromEntity<Translation>(),
            };
            // TODO: fix this schedule single
            var h1 = j1.ScheduleSingle(this, inputDeps);
            _barrier.AddJobHandleForProducer(h1);
            return h1;
        }

        [BurstCompile]
        private struct ResolveCollisionEventsJob : IJobForEachWithEntity_EC<CollisionComponent>
        {
            public EntityCommandBuffer.Concurrent commandBuffer;
            public ComponentDataFromEntity<Translation> entityTranslations;

            public void Execute(Entity entity, int index, ref CollisionComponent c0)
            {
                var ent1 = entityTranslations[c0.ent1];
                var ent2 = entityTranslations[c0.ent2];
                ent1.Value -= math.float3(c0.ent1ToEnt2Dir, 0f) * RepulsionPower;
                ent2.Value += math.float3(c0.ent1ToEnt2Dir, 0f) * RepulsionPower;
                entityTranslations[c0.ent1] = ent1;
                entityTranslations[c0.ent2] = ent2;
                commandBuffer.DestroyEntity(index, entity);
            }
        }
    }
}