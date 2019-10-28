using System;
using Strategio.GameConfigs;
using Unity.Entities;

namespace Strategio.Components
{
    [Serializable]
    public struct UnitComponent : IComponentData
    {
        public UnitType unitType;
    }
}