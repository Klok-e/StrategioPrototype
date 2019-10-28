using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.UIElements;

namespace Strategio.Components.Ui
{
    public struct ClickEventComponent : IComponentData
    {
        public MouseButton button;
        public float2 pos;
    }
}