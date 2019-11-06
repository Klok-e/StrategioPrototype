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

            UnitTypeConverter.InitArchetypes(dstManager);
            var tmpArr = new NativeArray<EntityArchetype>(UnitTypeConverter.archetypes, Allocator.Temp);
            var arch = UnitType.Spawner.GetArchetype(tmpArr);
            tmpArr.Dispose();
            var ent = dstManager.CreateEntity(arch);

            //TODO: wait until Unity decides to implement PerRendererData and MaterialPropertyBlock in ECS or do it myself
            var mat = new Material(mesh.material) {mainTexture = mainTex};
            mat.SetColor("_TintColor", tint);
            mesh.material = mat;

            dstManager.SetComponentData(ent,
                new SpawnerComponent
                {
                    spawnProgress = 0,
                    unitType = UnitType.Simple,
                });
            dstManager.SetComponentData(ent, new UnitComponent {unitType = UnitType.Spawner});
            dstManager.SetComponentData(ent, new CircleColliderComponent {radius = colliderRadius});
            dstManager.SetComponentData(ent, influencerComponent);
            dstManager.SetComponentData(ent, side);
            dstManager.SetComponentData(ent, new PathfindingComponent {isOrderedToMove = 0});
            dstManager.AddComponentData(ent, new PlayerCanOrderToMoveComponentTag());

            var pos = transform.position;
            pos.z = z;
            dstManager.SetComponentData(ent, new Translation {Value = pos});
            dstManager.SetSharedComponentData(ent, mesh);
            dstManager.SetComponentData(ent, new Scale {Value = transform.localScale.x});

            dstManager.DestroyEntity(entity);
        }
    }
}