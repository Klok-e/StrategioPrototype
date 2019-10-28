using System;
using Strategio.Util;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace Strategio.AuthoringGameObjects
{
    public struct GameArenaComponent : IComponentData
    {
        public int2 mapSize;
        public int influenceResolution;
    }

    public struct GameArenaTag : IComponentData
    {
    }

    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    public class ConvertGameArena : MonoBehaviour, IConvertGameObjectToEntity
    {
        [SerializeField]
        private Texture2D mainTex;

        [SerializeField]
        private RenderMesh arenaMesh;

        [SerializeField]
        private float z;

        [SerializeField]
        private DragCamera2D camera2D;

        [SerializeField]
        private int2 mapSize = new int2(50, 50);

        [SerializeField]
        private int influenceResolution;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var ent = dstManager.CreateEntity();
            dstManager.AddComponentData(ent, new GameArenaComponent
            {
                influenceResolution = influenceResolution,
                mapSize = mapSize,
            });

            camera2D.maxZoom = math.min(mapSize.x / 3, mapSize.y / 3);
            camera2D.cameraMaxX = mapSize.x - mapSize.x / 2f;
            camera2D.cameraMaxY = mapSize.y - mapSize.y / 2f;
            camera2D.cameraMinX = -mapSize.x + mapSize.x / 2f;
            camera2D.cameraMinY = -mapSize.x + mapSize.x / 2f;

            arenaMesh.material.SetTexture("_MainTex", mainTex);

            var pos = transform.position;
            pos.z = z;
            dstManager.SetComponentData(entity, new Translation {Value = pos});
            dstManager.AddComponentData(entity, new NonUniformScale {Value = math.float3(mapSize.xy, 1f)});
            dstManager.AddSharedComponentData(entity, arenaMesh);
            dstManager.AddComponentData(entity, new GameArenaTag());
        }
    }

    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class InitDataSystem : ComponentSystem
    {
        // Don't change after initialization!
        public int2 MapSize { get; private set; }

        public int MapResolution { get; private set; }

        public Texture2D InfluenceTexture { get; private set; }

        public NativeArray2D<int> Influences { get; private set; }

        protected override void OnUpdate()
        {
            Entities.ForEach((Entity ent, ref GameArenaComponent gameArena) =>
            {
                PostUpdateCommands.DestroyEntity(ent);
                MapSize = gameArena.mapSize;
                MapResolution = gameArena.influenceResolution;

                InfluenceTexture = new Texture2D(MapSize.x * MapResolution, MapSize.y * MapResolution,
                    GraphicsFormat.R32_SFloat,
                    TextureCreationFlags.None);
                Influences = new NativeArray2D<int>(MapSize.x * MapResolution, MapSize.y * MapResolution,
                    Allocator.Persistent);
            });
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Influences.Dispose();
        }
    }
}