using System.Collections.Generic;
using Strategio.Components;
using Strategio.Components.Physics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Strategio.Systems.Physics
{
    public class CircleCollideSystem : JobComponentSystem
    {
        private EntityQuery _colliders;
        private EndSimulationEntityCommandBufferSystem _barrier;

        // cached
        private NativeQueue<CollisionComponent> _queue;

        protected override void OnCreate()
        {
            base.OnCreate();
            _colliders = EntityManager.CreateEntityQuery(ComponentType.ReadOnly<CircleColliderComponent>(),
                ComponentType.ReadOnly<Translation>());
            _barrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            _queue = new NativeQueue<CollisionComponent>(Allocator.Persistent);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _queue.Dispose();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var transl = _colliders.ToComponentDataArray<Translation>(Allocator.TempJob);
            var colls = _colliders.ToComponentDataArray<CircleColliderComponent>(Allocator.TempJob);
            var j1 = new CollideJob
            {
                entities = _colliders.ToEntityArray(Allocator.TempJob),
                count = transl.Length,
                translations = transl,
                colliders = colls,
                collisions = _queue.AsParallelWriter()
            };
            var j2 = new CreateEntityJob
            {
                toCreate = _queue,
                commands = _barrier.CreateCommandBuffer(),
            };
            var h1 = j1.Schedule(transl.Length, 64, inputDeps);
            var h2 = j2.Schedule(h1);
            _barrier.AddJobHandleForProducer(h2);
            return h2;
        }

        [BurstCompile]
        private struct CollideJob : IJobParallelFor
        {
            public int count;

            [DeallocateOnJobCompletion]
            public NativeArray<Entity> entities;

            [DeallocateOnJobCompletion]
            public NativeArray<Translation> translations;

            [DeallocateOnJobCompletion]
            public NativeArray<CircleColliderComponent> colliders;

            public NativeQueue<CollisionComponent>.ParallelWriter collisions;

            public void Execute(int i1)
            {
                //TODO: optimize naive method
                for (int i2 = i1 + 1; i2 < count; i2++)
                {
                    var i1t = translations[i1].Value;
                    var i2t = translations[i2].Value;
                    if (math.distance(i1t, i2t) >= colliders[i1].radius + colliders[i2].radius) continue;

                    collisions.Enqueue(new CollisionComponent
                    {
                        ent1ToEnt2Dir = (i2t - i1t).xy,
                        ent1 = entities[i1],
                        ent2 = entities[i2],
                    });
                }
            }
        }

        private struct CreateEntityJob : IJob
        {
            public NativeQueue<CollisionComponent> toCreate;
            public EntityCommandBuffer commands;

            public void Execute()
            {
                while (toCreate.TryDequeue(out var item))
                {
                    var ent = commands.CreateEntity();
                    commands.AddComponent(ent, item);
                }
            }
        }
    }
}