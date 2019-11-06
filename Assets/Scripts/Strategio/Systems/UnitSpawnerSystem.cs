using System;
using Strategio.Components;
using Strategio.GameConfigs;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

namespace Strategio.Systems
{
    public class UnitSpawnerSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((ref SpawnerComponent spawner, ref Translation translation) =>
            {
                
            });
        }
    }
}