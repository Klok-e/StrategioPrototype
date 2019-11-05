using System;
using System.Collections.Generic;
using Strategio.Components;
using Strategio.Components.Physics;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace Strategio.GameConfigs
{
    public struct UnitStats
    {
        public float moveSpeed;
        public float attack;
        public float requiredProgress;
        public float health;
    }

    public static class UnitTypeConverter
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

        public static EntityArchetype[] archetypes;

        public static void InitArchetypes(EntityManager manager)
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
                    ComponentType.ReadWrite<Scale>()),

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
            archetypes = lst.ToArray();
        }

        public static EntityArchetype GetArchetype(this UnitType unit, NativeArray<EntityArchetype> archs)
        {
            return archs[(int) unit - 1];
        }
    }
}