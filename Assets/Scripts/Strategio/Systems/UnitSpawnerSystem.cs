using System;
using Strategio.Components;
using Strategio.Components.Physics;
using Strategio.GameConfigs;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.WSA;
using Random = Unity.Mathematics.Random;

namespace Strategio.Systems
{
    public class UnitSpawnerSystem : ComponentSystem
    {
        private InitDataSystem _dataSystem;
        private Random _random;

        protected override void OnCreate()
        {
            base.OnCreate();
            _dataSystem = World.GetOrCreateSystem<InitDataSystem>();
            _random = new Random(42);
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((ref SpawnerComponent spawner, ref SideComponent sideComponent,
                              ref Translation translation, ref CircleColliderComponent collider) =>
            {
                if (spawner.spawnProgress > spawner.unitType.GetStats().requiredProgressToSpawn)
                {
                    spawner.spawnProgress = 0f;

                    var ent = PostUpdateCommands.CreateEntity(spawner.unitType.GetArchetype(_dataSystem.archetypes));
                    var config = _dataSystem.GetConfigForUnitType(spawner.unitType, sideComponent.side);
                    float minSpawnDist = config.colliderRadius + collider.radius;
                    float dist = _random.NextFloat(minSpawnDist, minSpawnDist + minSpawnDist * 0.5f);
                    float angle = _random.NextFloat(0f, math.PI * 2);
                    var offset = math.float2(math.cos(angle), math.sin(angle)) * dist;
                    var pos = translation.Value + math.float3(offset, 0f);
                    UnitTypeUtil.SetCommonConfigComponentsToEntity(PostUpdateCommands, ent, config, pos,
                        sideComponent, spawner.unitType);
                }

                spawner.spawnProgress += Time.deltaTime;
            });
        }
    }
}