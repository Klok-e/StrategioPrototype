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
        private InitDataSystem _dataSystem;

        protected override void OnCreate()
        {
            base.OnCreate();
            _barrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            //_query = EntityManager.CreateEntityQuery(ComponentType.ReadWrite<Translation>());
            _dataSystem = World.GetOrCreateSystem<InitDataSystem>();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var mapsize = math.float2(_dataSystem.MapSize);
            var max = math.float2(mapsize / 2f);
            var min = math.float2(mapsize / 2f - mapsize);
            var j1 = new ResolveCollisionEventsJob
            {
                commandBuffer = _barrier.CreateCommandBuffer().ToConcurrent(),
                entityTranslations = GetComponentDataFromEntity<Translation>(),
                maxPos = max,
                minPos = min,
            };
            // TODO: fix this schedule single
            var h1 = j1.ScheduleSingle(this, inputDeps);
            _barrier.AddJobHandleForProducer(h1);
            return h1;
        }

        [BurstCompile]
        private struct ResolveCollisionEventsJob : IJobForEachWithEntity_EC<CollisionComponent>
        {
            public float2 minPos;
            public float2 maxPos;

            public EntityCommandBuffer.Concurrent commandBuffer;
            public ComponentDataFromEntity<Translation> entityTranslations;

            public void Execute(Entity entity, int index, ref CollisionComponent c0)
            {
                var ent1 = entityTranslations[c0.ent1];
                var ent2 = entityTranslations[c0.ent2];
                ent1.Value -= math.float3(c0.ent1ToEnt2Dir, 0f) * RepulsionPower;
                ent2.Value += math.float3(c0.ent1ToEnt2Dir, 0f) * RepulsionPower;

                // check for map bounds
                // unsafe is not to make allocations and not to duplicate code
                unsafe
                {
                    //TODO: change type of ents to Span<Translation> when it's supported
                    var ents = stackalloc[] {ent1, ent2};
                    for (int i = 0; i < 2; i++)
                    {
                        var currEnt = ents + i;
                        if (currEnt->Value.x < minPos.x)
                            currEnt->Value.x = minPos.x;
                        else if (currEnt->Value.x > maxPos.x)
                            currEnt->Value.x = maxPos.x;
                        if (currEnt->Value.y < minPos.y)
                            currEnt->Value.y = minPos.y;
                        else if (currEnt->Value.y > maxPos.y)
                            currEnt->Value.y = maxPos.y;
                    }

                    // because unity crashes when using "stackalloc[] {&ent1, &ent2}"
                    ent1 = ents[0];
                    ent2 = ents[1];
                }

                entityTranslations[c0.ent1] = ent1;
                entityTranslations[c0.ent2] = ent2;
                commandBuffer.DestroyEntity(index, entity);
            }
        }
    }
}