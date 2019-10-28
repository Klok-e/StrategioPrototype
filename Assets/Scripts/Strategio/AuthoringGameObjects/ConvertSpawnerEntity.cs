using Strategio.Components;
using Strategio.GameConfigs;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Serialization;

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
        private InfluencerComponent influencerComponent;

        [SerializeField]
        private SideComponent side;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            Debug.Assert(side.side != Side.Invalid);
            Debug.Assert(influencerComponent.num > 0);

            dstManager.AddComponentData(entity,
                new SpawnerComponent
                {
                    spawnProgress = 0,
                    unitType = UnitType.Simple,
                });
            dstManager.AddComponentData(entity, influencerComponent);
            dstManager.AddComponentData(entity, side);
            dstManager.AddComponentData(entity, new PathfindingComponent {isOrderedToMove = 0});
            var pos = transform.position;
            pos.z = z;
            dstManager.SetComponentData(entity, new Translation {Value = pos});
            dstManager.AddSharedComponentData(entity, mesh);
        }
    }
}