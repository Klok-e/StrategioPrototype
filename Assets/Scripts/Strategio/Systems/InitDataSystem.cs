using Strategio.AuthoringGameObjects;
using Strategio.Util;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
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