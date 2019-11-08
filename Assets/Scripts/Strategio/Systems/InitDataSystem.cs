using System;
using Strategio.AuthoringGameObjects;
using Strategio.Components;
using Strategio.Components.Physics;
using Strategio.GameConfigs;
using Strategio.Util;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace Strategio.Systems
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class InitDataSystem : ComponentSystem
    {
        // Don't change after initialization!
        public int2 MapSize { get; private set; }

        public int MapResolution { get; private set; }

        public Texture2D InfluenceTexture { get; private set; }

        public NativeArray2D<float> Influences { get; private set; }

        public UnitCommonConfig[] Side1Configs { get; private set; }

        public UnitCommonConfig[] Side2Configs { get; private set; }

        public NativeArray<EntityArchetype> archetypes;

        public void Init(int2 mapSize, int influenceResolution,
                         UnitCommonConfig[] side1Configs,
                         UnitCommonConfig[] side2Configs)
        {
            MapSize = mapSize;
            MapResolution = influenceResolution;
            Side1Configs = side1Configs;
            Side2Configs = side2Configs;

            InfluenceTexture = new Texture2D(MapSize.x * MapResolution, MapSize.y * MapResolution,
                GraphicsFormat.R32_SFloat,
                TextureCreationFlags.None);
            Influences = new NativeArray2D<float>(MapSize.x * MapResolution, MapSize.y * MapResolution,
                Allocator.Persistent);
            archetypes =
                new NativeArray<EntityArchetype>(UnitTypeUtil.InitArchetypes(EntityManager), Allocator.Persistent);
            foreach (var configs in new[] {side1Configs, side2Configs})
                for (int i = 0; i < configs.Length; i++)
                {
                    //TODO: wait until Unity decides to implement PerRendererData and MaterialPropertyBlock in ECS or do it myself
                    var mat = new Material(configs[i].mesh.material) {mainTexture = configs[i].mainTex};
                    mat.SetColor("_TintColor", configs[i].tint);
                    configs[i].mesh.material = mat;
                }
        }

        public UnitCommonConfig GetConfigForUnitType(UnitType unitType, Side side)
        {
            UnitCommonConfig config;
            switch (side)
            {
                case Side.Player1:
                    config = unitType.GetConfig(Side1Configs);
                    break;
                case Side.Player2:
                    config = unitType.GetConfig(Side1Configs);
                    break;
                case Side.Invalid:
                    throw new ArgumentOutOfRangeException(nameof(side), side, null);
                default:
                    throw new ArgumentOutOfRangeException(nameof(side), side, null);
            }

            return config;
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entToDel, ref NeedSpawnSpawnerComponent needSpawn) =>
            {
                var config = GetConfigForUnitType(UnitType.Spawner,needSpawn.side.side);

                var ent = EntityManager.CreateEntity(UnitType.Spawner.GetArchetype(archetypes));

                EntityManager.SetComponentData(ent,
                    new SpawnerComponent
                    {
                        spawnProgress = 0,
                        unitType = UnitType.Simple,
                    });
                EntityManager.AddComponentData(ent, new PlayerCanOrderToMoveComponentTag());
                UnitTypeUtil.SetCommonConfigComponentsToEntity(EntityManager, ent, config, needSpawn.position,
                    needSpawn.side, UnitType.Spawner);

                PostUpdateCommands.DestroyEntity(entToDel);
            });
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Influences.Dispose();
            archetypes.Dispose();
        }
    }
}