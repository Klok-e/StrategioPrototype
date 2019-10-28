using System;
using Strategio.Components;
using Strategio.Components.Ui;
using Strategio.GameConfigs;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine.UIElements;

namespace Strategio.Systems
{
    public class UserClickEventProcessorEventSystem : JobComponentSystem
    {
        private EndSimulationEntityCommandBufferSystem _barrier;

        private NativeList<ClickEventComponent> _clickEvents;

        protected override void OnCreate()
        {
            base.OnCreate();
            _barrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            _clickEvents = new NativeList<ClickEventComponent>(Allocator.Persistent);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _clickEvents.Dispose();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var buffer = _barrier.CreateCommandBuffer();
            var j1 = new AddClicksToListJob
            {
                clickEvents = _clickEvents,
                commandBuffer = buffer.ToConcurrent()
            };
            var j2 = new ProcessLeftRightClicksJob
            {
                events = _clickEvents,
            };
            var j3 = new ClearListJob
            {
                list = _clickEvents,
            };
            var h1 = j1.ScheduleSingle(this, inputDeps);
            _barrier.AddJobHandleForProducer(h1);
            var h2 = j2.Schedule(this, h1);
            var h3 = j3.Schedule(h2);
            return h3;
        }

        [BurstCompile]
        private struct AddClicksToListJob : IJobForEachWithEntity_EC<ClickEventComponent>
        {
            public EntityCommandBuffer.Concurrent commandBuffer;
            public NativeList<ClickEventComponent> clickEvents;

            public void Execute(Entity entity, int index, ref ClickEventComponent c0)
            {
                clickEvents.Add(c0);
                commandBuffer.DestroyEntity(index, entity);
            }
        }

        [BurstCompile]
        private struct ProcessLeftRightClicksJob : IJobForEach_CC<PathfindingComponent, SideComponent>
        {
            [ReadOnly]
            public NativeList<ClickEventComponent> events;

            public void Execute(ref PathfindingComponent c0, ref SideComponent c1)
            {
                for (int i = 0; i < events.Length; i++)
                {
                    var item = events[i];
                    switch (item.button)
                    {
                        case MouseButton.LeftMouse:
                            if (c1.side == Side.Player1)
                            {
                                c0.goal = item.pos;
                                c0.isOrderedToMove = 1;
                            }

                            break;
                        case MouseButton.RightMouse:
                            if (c1.side == Side.Player2)
                            {
                                c0.goal = item.pos;
                                c0.isOrderedToMove = 1;
                            }

                            break;
                        case MouseButton.MiddleMouse:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

        [BurstCompile]
        private struct ClearListJob : IJob
        {
            public NativeList<ClickEventComponent> list;

            public void Execute()
            {
                list.Clear();
            }
        }
    }
}