using System;
using Unity.Entities;

namespace Strategio.Components
{
    public enum SpawnType
    {
        Invalid = 0,
        Simple = 1,
    }

    [Serializable]
    public struct Spawner : IComponentData
    {
        public float spawnProgress;
        public float spawnProgressRequired;
        public SpawnType spawnType;
    }
}