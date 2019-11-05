using System;
using Strategio.Components;
using Strategio.GameConfigs;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

namespace Strategio.Systems
{
    public class UnitSpawnerSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return inputDeps;
        }

        private struct SpawnStuff : IJobForEach_CC<SpawnerComponent, Translation>
        {
            public EntityCommandBuffer.Concurrent buffer;

            public void Execute(ref SpawnerComponent c0, ref Translation c1)
            {
                switch (c0.unitType)
                {
                    case UnitType.Invalid:
                        break;
                    case UnitType.Simple:
                        break;
                    case UnitType.Spawner:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}