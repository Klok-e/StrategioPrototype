using Strategio.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace Strategio.AuthoringGameObjects
{
    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    public class ConvertSpawnerEntity : MonoBehaviour, IConvertGameObjectToEntity
    {
        [SerializeField]
        private RenderMesh mesh;

        [SerializeField]
        private float z;

        [SerializeField]
        private Influencer influencer;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity,
                new Spawner
                {
                    spawnProgress = 0,
                    spawnType = SpawnType.Simple,
                    spawnProgressRequired = 0.5f
                });
            dstManager.AddComponentData(entity, influencer);
            var pos = transform.position;
            pos.z = z;
            dstManager.SetComponentData(entity, new Translation {Value = pos});
            dstManager.AddSharedComponentData(entity, mesh);
        }
    }
}