using System;
using Strategio.Util;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace Strategio.AuthoringGameObjects
{
    public struct GameArenaComponent : ISharedComponentData, IEquatable<GameArenaComponent>
    {
        public GameObject arena;

        // Don't change after initialization!
        public int2 mapSize;

        public SpriteRenderer spriteRenderer;

        public bool Equals(GameArenaComponent other)
        {
            return Equals(arena, other.arena) && mapSize.Equals(other.mapSize) &&
                   Equals(spriteRenderer, other.spriteRenderer);
        }

        public override bool Equals(object obj)
        {
            return obj is GameArenaComponent other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (arena != null ? arena.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ mapSize.GetHashCode();
                hashCode = (hashCode * 397) ^ (spriteRenderer != null ? spriteRenderer.GetHashCode() : 0);
                return hashCode;
            }
        }
    }

    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    public class ConvertGameArena : MonoBehaviour, IConvertGameObjectToEntity
    {
        [SerializeField]
        private Material arenaMat;

        [SerializeField]
        private Sprite arenaSprite;

        [SerializeField]
        private DragCamera2D camera2D;

        [SerializeField]
        public int2 mapSize = new int2(50, 50);

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var arena = new GameObject("ArenaSprite");

            var spriteRenderer = arena.AddComponent<SpriteRenderer>();
            spriteRenderer.material = arenaMat;
            spriteRenderer.sprite = arenaSprite;
            spriteRenderer.drawMode = SpriteDrawMode.Tiled;
            spriteRenderer.size = new Vector2(mapSize.x, mapSize.y);
            camera2D.cameraMaxX = 25f;
            camera2D.cameraMaxY = 25f;
            camera2D.cameraMinX = -25f;
            camera2D.cameraMinY = -25f;

            dstManager.AddSharedComponentData(entity, new GameArenaComponent
            {
                arena = arena,
                mapSize = mapSize,
                spriteRenderer = spriteRenderer,
            });
        }
    }

    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class InitDataSystem : ComponentSystem
    {
        public GameObject Arena { get; private set; }

        // Don't change after initialization!
        public int2 MapSize { get; private set; }

        public SpriteRenderer SpriteRenderer { get; private set; }

        public Texture2D InfluenceTexture { get; private set; }

        public NativeArray2D<int> Influences { get; private set; }

        protected override void OnUpdate()
        {
            Entities.ForEach((Entity ent, GameArenaComponent gameArena) =>
            {
                Arena = gameArena.arena;
                MapSize = gameArena.mapSize;
                SpriteRenderer = gameArena.spriteRenderer;
                EntityManager.DestroyEntity(ent);

                InfluenceTexture = new Texture2D(MapSize.x, MapSize.y, GraphicsFormat.R32_SFloat,
                    TextureCreationFlags.None);
                Influences = new NativeArray2D<int>(MapSize.x, MapSize.y, Allocator.Persistent);
            });
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Influences.Dispose();
        }
    }
}