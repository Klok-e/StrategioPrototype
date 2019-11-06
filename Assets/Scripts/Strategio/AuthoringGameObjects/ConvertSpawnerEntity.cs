using System;
using Strategio.Components;
using Strategio.Components.Physics;
using Strategio.GameConfigs;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Serialization;

namespace Strategio.AuthoringGameObjects
{
    public struct NeedSpawnSpawnerComponent : IComponentData
    {
        public float3 position;
        public SideComponent side;
    }

    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    public class ConvertSpawnerEntity : MonoBehaviour, IConvertGameObjectToEntity
    {
        [SerializeField]
        private SideComponent side;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            Debug.Assert(side.side != Side.Invalid);
            
            var ent = dstManager.CreateEntity();

            dstManager.AddComponentData(ent, new NeedSpawnSpawnerComponent
            {
                position = transform.position,
                side = side,
            });

            dstManager.DestroyEntity(entity);
        }
    }
}