using System;
using Strategio.GameConfigs;
using Unity.Entities;
using UnityEngine.Serialization;

namespace Strategio.Components
{
    [Serializable]
    public struct SpawnerComponent : IComponentData
    {
        public float spawnProgress;
        public UnitType unitType;
    }
}