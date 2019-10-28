using Strategio.Components;
using Strategio.Components.Physics;
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
        private float colliderRadius;

        [SerializeField]
        private Texture2D mainTex;

        [SerializeField]
        private Color tint;

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

            //TODO: wait until Unity decides to implement PerRendererData and MaterialPropertyBlock in ECS or do it myself
            var mat = new Material(mesh.material) {mainTexture = mainTex};
            mat.SetColor("_TintColor", tint);
            mesh.material = mat;

            dstManager.AddComponentData(entity,
                new SpawnerComponent
                {
                    spawnProgress = 0,
                    unitType = UnitType.Simple,
                });
            dstManager.AddComponentData(entity, new UnitComponent {unitType = UnitType.Spawner});
            dstManager.AddComponentData(entity, new CircleColliderComponent {radius = colliderRadius});
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