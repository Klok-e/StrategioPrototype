using System;
using Strategio.Components;
using Strategio.GameConfigs;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.WSA;

namespace Strategio.Systems
{
    public class UnitSpawnerSystem : ComponentSystem
    {
        private InitDataSystem _dataSystem;

        protected override void OnCreate()
        {
            base.OnCreate();
            _dataSystem = World.GetOrCreateSystem<InitDataSystem>();
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((ref SpawnerComponent spawner, ref SideComponent sideComponent,
                              ref Translation translation) =>
            {
                if (spawner.spawnProgress > spawner.unitType.GetStats().requiredProgressToSpawn)
                {
                    spawner.spawnProgress = 0f;

                    var ent = PostUpdateCommands.CreateEntity(spawner.unitType.GetArchetype(_dataSystem.archetypes));
                    var config = _dataSystem.GetConfigForUnitType(spawner.unitType, sideComponent.side);
                    UnitTypeUtil.SetCommonConfigComponentsToEntity(PostUpdateCommands, ent, config, translation.Value,
                        sideComponent, spawner.unitType);
                }

                spawner.spawnProgress += Time.deltaTime;
            });
        }
    }
}