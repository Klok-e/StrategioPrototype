using System.Collections.Generic;
using Strategio.Components;
using Strategio.Components.Physics;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace Strategio.GameConfigs
{
    public static class UnitTypeUtil
    {
        public static UnitStats GetStats(this UnitType unit)
        {
            Debug.Assert(unit != UnitType.Invalid);
            var x = new UnitStats();
            switch (unit)
            {
                case UnitType.Invalid:
                    break;
                case UnitType.Simple:
                    x = new UnitStats
                    {
                        attack = 1f,
                        health = 5f,
                        moveSpeed = 0.5f,
                        requiredProgress = 0.5f,
                    };
                    break;
                case UnitType.Spawner:
                    x = new UnitStats
                    {
                        attack = 1f,
                        health = 100f,
                        moveSpeed = 0.1f,
                        requiredProgress = 50f,
                    };
                    break;
            }

            return x;
        }

        public static EntityArchetype[] InitArchetypes(EntityManager manager)
        {
            var lst = new List<EntityArchetype>
            {
                // Simple = 1
                manager.CreateArchetype(
                    ComponentType.ReadWrite<UnitComponent>(),
                    ComponentType.ReadWrite<CircleColliderComponent>(),
                    ComponentType.ReadWrite<InfluencerComponent>(),
                    ComponentType.ReadWrite<SideComponent>(),
                    ComponentType.ReadWrite<PathfindingComponent>(),
                    ComponentType.ReadWrite<Translation>(),
                    ComponentType.ReadWrite<RenderMesh>(),
                    ComponentType.ReadWrite<Scale>(),
                    ComponentType.ReadWrite<LocalToWorld>(),
                    ComponentType.ReadWrite<Rotation>()),

                // Spawner = 2
                manager.CreateArchetype(
                    ComponentType.ReadWrite<SpawnerComponent>(),
                    ComponentType.ReadWrite<UnitComponent>(),
                    ComponentType.ReadWrite<CircleColliderComponent>(),
                    ComponentType.ReadWrite<InfluencerComponent>(),
                    ComponentType.ReadWrite<SideComponent>(),
                    ComponentType.ReadWrite<PathfindingComponent>(),
                    ComponentType.ReadWrite<Translation>(),
                    ComponentType.ReadWrite<RenderMesh>(),
                    ComponentType.ReadWrite<Scale>(),
                    ComponentType.ReadWrite<LocalToWorld>(),
                    ComponentType.ReadWrite<Rotation>())
            };
            return lst.ToArray();
        }

        public static void SetCommonConfigComponentsToEntity(EntityManager manager, Entity entity,
                                                             UnitCommonConfig config, float3 position,
                                                             SideComponent side)
        {
            manager.SetComponentData(entity, new UnitComponent {unitType = UnitType.Spawner});
            manager.SetComponentData(entity, new CircleColliderComponent {radius = config.colliderRadius});
            manager.SetComponentData(entity, config.influencerComponent);
            manager.SetComponentData(entity, side);
            manager.SetComponentData(entity, new PathfindingComponent {isOrderedToMove = 0});
            var pos = position;
            pos.z = config.z;
            manager.SetComponentData(entity, new Translation {Value = pos});
            manager.SetSharedComponentData(entity, config.mesh);
            manager.SetComponentData(entity, config.scale);
        }

        public static EntityArchetype GetArchetype(this UnitType unit, NativeArray<EntityArchetype> archs)
        {
            return archs[(int) unit - 1];
        }

        public static UnitCommonConfig GetConfig(this UnitType unit, UnitCommonConfig[] configs)
        {
            return configs[(int) unit - 1];
        }
    }
}