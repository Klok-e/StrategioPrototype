using System;
using Unity.Entities;

namespace Strategio.Components
{
    [Serializable]
    public enum Side
    {
        Invalid = 0,
        Friend = 1,
        Enemy = 2,
    }

    [Serializable]
    public struct Influencer : IComponentData
    {
        public int num;
        public Side side;
    }
}