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

        public NativeArray2D<int> Influences { get; private set; }

        public UnitCommonConfig[] side1Config;

        public UnitCommonConfig[] side2Config;

        public NativeArray<EntityArchetype> archetypes;

        public void Init(int2 mapSize, int influenceResolution,
                         UnitCommonConfig[] side1Config,
                         UnitCommonConfig[] side2Config)
        {
            MapSize = mapSize;
            MapResolution = influenceResolution;
            this.side1Config = side1Config;
            this.side2Config = side2Config;

            InfluenceTexture = new Texture2D(MapSize.x * MapResolution, MapSize.y * MapResolution,
                GraphicsFormat.R32_SFloat,
                TextureCreationFlags.None);
            Influences = new NativeArray2D<int>(MapSize.x * MapResolution, MapSize.y * MapResolution,
                Allocator.Persistent);
            archetypes =
                new NativeArray<EntityArchetype>(UnitTypeUtil.InitArchetypes(EntityManager), Allocator.Persistent);
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entToDel, ref NeedSpawnSpawnerComponent needSpawn) =>
            {
                UnitCommonConfig[] configs;
                switch (needSpawn.side.side)
                {
                    case Side.Invalid:
                        throw new ArgumentOutOfRangeException();
                    case Side.Player1:
                        configs = side1Config;
                        break;
                    case Side.Player2:
                        configs = side2Config;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                var config = UnitType.Spawner.GetConfig(configs);

                //TODO: wait until Unity decides to implement PerRendererData and MaterialPropertyBlock in ECS or do it myself
                var mat = new Material(config.mesh.material) {mainTexture = config.mainTex};
                mat.SetColor("_TintColor", config.tint);
                config.mesh.material = mat;

                var ent = EntityManager.CreateEntity(UnitType.Spawner.GetArchetype(archetypes));

                EntityManager.SetComponentData(ent,
                    new SpawnerComponent
                    {
                        spawnProgress = 0,
                        unitType = UnitType.Simple,
                    });
                EntityManager.SetComponentData(ent, new UnitComponent {unitType = UnitType.Spawner});
                EntityManager.SetComponentData(ent, new CircleColliderComponent {radius = config.colliderRadius});
                EntityManager.SetComponentData(ent, config.influencerComponent);
                EntityManager.SetComponentData(ent, needSpawn.side);
                EntityManager.SetComponentData(ent, new PathfindingComponent {isOrderedToMove = 0});
                var pos = needSpawn.position;
                pos.z = config.z;
                EntityManager.SetComponentData(ent, new Translation {Value = pos});
                EntityManager.SetSharedComponentData(ent, config.mesh);
                EntityManager.SetComponentData(ent, config.scale);

                EntityManager.AddComponentData(ent, new PlayerCanOrderToMoveComponentTag());

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