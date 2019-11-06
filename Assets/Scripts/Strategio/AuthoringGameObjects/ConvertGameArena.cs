using System;
using Strategio.GameConfigs;
using Strategio.Systems;
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

        [SerializeField]
        private UnitCommonConfig[] side1Config;

        [SerializeField]
        private UnitCommonConfig[] side2Config;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.World.GetOrCreateSystem<InitDataSystem>()
                      .Init(mapSize, influenceResolution, side1Config, side2Config);
            UnitTypeConverter.InitArchetypes(dstManager);

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
}