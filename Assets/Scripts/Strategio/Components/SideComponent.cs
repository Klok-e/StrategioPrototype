using System;
using Strategio.GameConfigs;
using Unity.Entities;

namespace Strategio.Components
{
    [Serializable]
    public struct SideComponent : IComponentData
    {
        public Side side;
    }
}