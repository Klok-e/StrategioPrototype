using System;
using Strategio.GameConfigs;
using Unity.Entities;

namespace Strategio.Components
{
    [Serializable]
    public struct InfluencerComponent : IComponentData
    {
        public int num;
    }
}